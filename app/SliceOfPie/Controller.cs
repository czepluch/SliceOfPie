using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SliceOfPie.ApmHelpers;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SliceOfPieTests")]

namespace SliceOfPie {

    /// <summary>
    /// Controller of SliceOfPie.
    /// </summary>
    public class Controller {

        private static Controller _instance = null;
        private static bool isWebController = false;

        /// <summary>
        /// Get the single Controller instance.
        /// </summary>
        public static Controller Instance {
            get {
                if (_instance == null) _instance = new Controller();
                return _instance;
            }
        }

        /// <summary>
        /// Denotes whether the controller used is a web controller or a local controller. Set to true before
        /// using the controller (calling Instance) to change the type of it.
        /// </summary>
        /// <seealso cref="Controller.Instance"/>
        public static bool IsWebController {
            get {
                return isWebController;
            }
            set {
                if (value != isWebController) {
                    if (value == false) {
                        _instance = new Controller(new LocalFileModel());
                    }
                    else {
                        _instance = new Controller(new WebFileModel());
                    }
                    isWebController = value;
                }
            }
        }

        private IFileModel fileModel;
        private UserModel userModel;

        /// <summary>
        /// Create a new instance of a controller. Made private as to only allow one instance
        /// to be created. External sources should get the controller from <c>Controller.Instance;</c>
        /// </summary>
        /// <param name="model">FileModel to use (local or web?)</param>
        private Controller(IFileModel model = null) {
            if (model == null) model = new LocalFileModel();
            fileModel = model;
            userModel = new UserModel();
        }

        /// <summary>
        /// Helper reducing code duplication
        /// </summary>
        private Action<IAsyncResult> apmNoResultHelper = ApmMethodFactory.CreateNoResultEndMethod();

        #region Project

        #region Create Project

        /// <summary>
        /// Create a new project synchronously.
        /// </summary>
        /// <param name="name">Name of project</param>
        /// <param name="userMail">Email of user</param>
        /// <returns>The new project</returns>
        /// <seealso cref="BeginCreateProject"/>
        public Project CreateProject(string name, string userMail) {
            return fileModel.AddProject(name, userMail);
        }

        /// <summary>
        /// Helper for async project creation
        /// </summary>
        /// <param name="asyncResult"><c>AsyncResult&lt;Project,string,string&gt;</c></param>
        private void CreateProjectAsyncHelper(object asyncResult) {
            AsyncResult<Project,string,string> ar = (AsyncResult<Project,string,string>)asyncResult;
            try {
                Project result = CreateProject(ar.Parameter1, ar.Parameter2);
                ar.SetAsCompleted(result, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Create a project asynchronously, according to the Asynchronous Programming Model (seealso 
        /// http://msdn.microsoft.com/en-us/library/ms228963.aspx)
        /// </summary>
        /// <param name="name">Name of the project to create</param>
        /// <param name="userMail">Email of the user who owns the project</param>
        /// <param name="callback">Callback delegate, called on completion</param>
        /// <param name="stateObject">State object (passed to callback)</param>
        /// <returns><c>IAsyncResult</c> to be used with <c>EndCreateProject</c></returns>
        /// <seealso cref="EndCreateProject"/>
        public IAsyncResult BeginCreateProject(string name, string userMail, AsyncCallback callback, object stateObject) {
            AsyncResult<Project,string,string> ar = new AsyncResult<Project,string,string>(callback, stateObject, name, userMail);
            ThreadPool.QueueUserWorkItem(CreateProjectAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Wait for async creation of project to end, then return project.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from <c>BeginCreateProject</c></param>
        /// <returns>Newly created project</returns>
        public Project EndCreateProject(IAsyncResult asyncResult) {
            AsyncResult<Project, string, string> ar = (AsyncResult<Project, string, string>)asyncResult;
            return ar.EndInvoke();
        }

        #endregion

        #region Remove Project

        /// <summary>
        /// Remove the specified project from the system.
        /// </summary>
        /// <param name="p">Project to remove.</param>
        /// <seealso cref="BeginRemoveProject"/>
        public void RemoveProject(Project p) {
            fileModel.RemoveProject(p);
        }

        /// <summary>
        /// Helper to remove project asynchronously
        /// </summary>
        /// <param name="asyncResult"><c>AsyncResultNoResult&lt;Project&gt;</c></param>
        private void RemoveProjectAsyncHelper(object asyncResult) {
            AsyncResultNoResult<Project> ar = (AsyncResultNoResult<Project>)asyncResult;
            try {
                RemoveProject(ar.Parameter1);
                ar.SetAsCompleted(null, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Remove a project asynchronously, according to the APM (Asynchrnonous Programming Model).
        /// </summary>
        /// <param name="p">Project to remove</param>
        /// <param name="callback">Callback called upon finish</param>
        /// <param name="state">State object, passed to callback.</param>
        /// <returns>IAsyncResult used by EndRemoveProject</returns>
        /// <seealso cref="EndRemoveProject"/>
        public IAsyncResult BeginRemoveProject(Project p, AsyncCallback callback, object state) {
            AsyncResultNoResult<Project> ar = new AsyncResultNoResult<Project>(callback, state, p);
            ThreadPool.QueueUserWorkItem(RemoveProjectAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Block until asynchronous removal of project is underway.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult made by BeginRemoveProject</param>
        /// <seealso cref="BeginRemoveProject"/>
        public void EndRemoveProject(IAsyncResult asyncResult) {
            apmNoResultHelper(asyncResult);
        }

        #endregion

        #region Share Project

        /// <summary>
        /// Share the project with other users (per email).
        /// </summary>
        /// <param name="p">Project to share</param>
        /// <param name="commaSeperatedEmailList">Emails as a comma-seperated string.</param>
        public void ShareProject(Project p, string commaSeparatedEmailList) {
            ShareProject(p, commaSeparatedEmailList.Split(','));
        }

        /// <summary>
        /// Share the project with other users (per email).
        /// </summary>
        /// <param name="p">Project to share</param>
        /// <param name="emails">Emails as an iterable of strings</param>
        public void ShareProject(Project p, IEnumerable<string> emails) {
            foreach (string email in emails) {
                userModel.ShareProject(p.Id, email);
            }
        }

        /// <summary>
        /// Helper facilitating asynchronous sharing of projects
        /// </summary>
        /// <param name="asyncResult">AsyncResultNoResult&lt;Project, IEnumerable&lt;string&gt;&gt;</param>
        private void ShareProjectAsyncHelper(object asyncResult) {
            AsyncResultNoResult<Project, IEnumerable<string>> ar = (AsyncResultNoResult<Project, IEnumerable<string>>)asyncResult;
            try {
                ShareProject(ar.Parameter1, ar.Parameter2);
                ar.SetAsCompleted(null, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Starts sharing a project using APM.
        /// </summary>
        /// <param name="p">Project to share</param>
        /// <param name="emails">Emails to share the project with</param>
        /// <param name="callback">Callback called upon conclusion of sharing</param>
        /// <param name="stateObject">state object woopdashoopfloop</param>
        /// <returns>IAsyncResult for EnShareProject</returns>
        /// <seealso cref="EndShareProject"/>
        public IAsyncResult BeginShareProject(Project p, IEnumerable<string> emails, AsyncCallback callback, object stateObject) {
            AsyncResultNoResult<Project, IEnumerable<string>> ar = new AsyncResultNoResult<Project, IEnumerable<string>>(callback, stateObject, p, emails);
            ThreadPool.QueueUserWorkItem(ShareProjectAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Wait for asynchronous project sharing to finish, then return it
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginShareProject</param>
        /// <seealso cref="BeginShareProject"/>
        public void EndShareProject(IAsyncResult asyncResult) {
            AsyncResultNoResult<Project, IEnumerable<string>> ar = (AsyncResultNoResult<Project, IEnumerable<string>>)asyncResult;
            ar.EndInvoke();
        }

        #endregion

        #endregion

        #region Document

        #region Create Document

        /// <summary>
        /// Create a new document
        /// </summary>
        /// <param name="name">Name of document</param>
        /// <param name="userMail">Email of owning user</param>
        /// <param name="parent">Parent of document. Must be placed in a project or folder</param>
        /// <returns>Newly created document</returns>
        /// <seealso cref="BeginCreateDocument"/>
        public Document CreateDocument(string name, string userMail, IItemContainer parent) {
            return fileModel.AddDocument(parent, name);
        }

        /// <summary>
        /// Helper facilitating asynchronous creation of documents
        /// </summary>
        /// <param name="asyncResult">AsyncResult&lt;Document, string, string, IItemContainer&gt;</param>
        private void CreateDocumentAsyncHelper(object asyncResult) {
            AsyncResult<Document, string, string, IItemContainer> ar = (AsyncResult<Document, string, string, IItemContainer>)asyncResult;
            try {
                Document result = CreateDocument(ar.Parameter1, ar.Parameter2, ar.Parameter3);
                ar.SetAsCompleted(result, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Begin creation of a document using the Asynchronous Programming Model.
        /// </summary>
        /// <param name="name">Name of document</param>
        /// <param name="userMail">Email of owning user</param>
        /// <param name="parent">Folder or project to place the document in</param>
        /// <param name="callback">Callback called on completion of creation</param>
        /// <param name="stateObject">state object (passed to callback)</param>
        /// <returns>IAsyncResult to be used in EndCreateDocument</returns>
        /// <seealso cref="EndCreateDocument"/>
        public IAsyncResult BeginCreateDocument(string name, string userMail, IItemContainer parent, AsyncCallback callback, object stateObject) {
            AsyncResult<Document, string, string, IItemContainer> ar = new AsyncResult<Document, string, string, IItemContainer>(callback, stateObject, name, userMail, parent);
            ThreadPool.QueueUserWorkItem(CreateDocumentAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Wait for asynchronous document creation to finish, then return it
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginCreateDocument</param>
        /// <returns>Newly created document</returns>
        /// <seealso cref="BeginCreateDocument"/>
        public Document EndCreateDocument(IAsyncResult asyncResult) {
            AsyncResult<Document, string, string, IItemContainer> ar = (AsyncResult<Document, string, string, IItemContainer>)asyncResult;
            return ar.EndInvoke();
        }

        #endregion

        #region Save Document

        /// <summary>
        /// Save a document
        /// </summary>
        /// <param name="document">Document to save</param>
        public void SaveDocument(Document document) {
            fileModel.SaveDocument(document);
        }

        /// <summary>
        /// Helper method to asynchronously save documents
        /// </summary>
        /// <param name="asyncResult">AsyncResultNoResult&lt;Document&gt;</param>
        private void SaveDocumentAsyncHelper(object asyncResult) {
            AsyncResultNoResult<Document> ar = (AsyncResultNoResult<Document>)asyncResult;
            try {
                SaveDocument(ar.Parameter1);
                ar.SetAsCompleted(null, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Start saving a document asynchronously in accordance with the Asynchronous Programming Model.
        /// </summary>
        /// <param name="d">Document to save</param>
        /// <param name="callback">Callback called when document saving finishes</param>
        /// <param name="state">State object, passed to callback</param>
        /// <returns>IAsyncResult for EndSaveDocument</returns>
        /// <seealso cref="EndSaveDocument"/>
        public IAsyncResult BeginSaveDocument(Document d, AsyncCallback callback, object state) {
            AsyncResultNoResult<Document> ar = new AsyncResultNoResult<Document>(callback, state, d);
            ThreadPool.QueueUserWorkItem(SaveDocumentAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Block until the document saving concludes, then continue.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginSaveDocument</param>
        /// <seealso cref="BeginSaveDocument"/>
        public void EndSaveDocument(IAsyncResult asyncResult) {
            apmNoResultHelper(asyncResult);
        }

        #endregion

        #region Remove Document

        /// <summary>
        /// Remove a document from the system.
        /// </summary>
        /// <param name="d">Document to remove</param>
        /// <seealso cref="BeginRemoveDocument"/>
        public void RemoveDocument(Document d) {
            fileModel.RemoveDocument(d);
        }

        /// <summary>
        /// Helper for asynchronously removing documents.
        /// </summary>
        /// <param name="asyncResult">AsyncResultNoResult&lt;Document&gt;</param>
        private void RemoveDocumentAsyncHelper(object asyncResult) {
            AsyncResultNoResult<Document> ar = (AsyncResultNoResult<Document>)asyncResult;
            try {
                RemoveDocument(ar.Parameter1);
                ar.SetAsCompleted(null, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Start removing a document asynchronously in accordance with the Asynchronous Programming Model.
        /// </summary>
        /// <param name="d">Document to remove</param>
        /// <param name="callback">Callback called on removal completion</param>
        /// <param name="state">State object (passed to callback)</param>
        /// <returns>IAsyncResult for EndRemoveDocument</returns>
        /// <seealso cref="EndRemoveDocument"/>
        public IAsyncResult BeginRemoveDocument(Document d, AsyncCallback callback, object state) {
            AsyncResultNoResult<Document> ar = new AsyncResultNoResult<Document>(callback, state, d);
            ThreadPool.QueueUserWorkItem(RemoveDocumentAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Wait for removal of document to finish, then continue (block while waiting).
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginRemoveDocument</param>
        /// <seealso cref="BeginRemoveDocument"/>
        public void EndRemoveDocument(IAsyncResult asyncResult) {
            apmNoResultHelper(asyncResult);
        }

        #endregion

        #region Download Revisions

        /// <summary>
        /// Download revisions for a specific Document and add them to internal collection.
        /// </summary>
        /// <param name="d"></param>
        public IEnumerable<Revision> DownloadRevisions(Document d) {
            foreach (Revision r in fileModel.DownloadRevisions(d)) {
                yield return r;
            }
        }

        /// <summary>
        /// Helper method to asynchronously download revisions
        /// </summary>
        /// <param name="asyncResult">AsyncResult&lt;IEnumerable&lt;Revision&gt;, Document&gt;</param>
        private void DownloadRevisionsAsyncHelper(object asyncResult) {
            AsyncResult<IEnumerable<Revision>, Document> ar = (AsyncResult<IEnumerable<Revision>, Document>)asyncResult;
            try {
                var result = DownloadRevisions(ar.Parameter1);
                ar.SetAsCompleted(result, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Start downloading revisions asynchronously in accordance with the Asynchronous Programming Model.
        /// </summary>
        /// <param name="d">Document whose revisions to get</param>
        /// <param name="callback">Callback called when download of revisions finishes</param>
        /// <param name="state">State object, passed to callback</param>
        /// <returns>IAsyncResult for EndDownloadRevisions</returns>
        /// <seealso cref="EndDownloadRevisions"/>
        public IAsyncResult BeginDownloadRevisions(Document d, AsyncCallback callback, object state) {
            AsyncResult <IEnumerable<Revision>, Document> ar = new AsyncResult<IEnumerable<Revision>, Document>(callback, state, d);
            ThreadPool.QueueUserWorkItem(DownloadRevisionsAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Block until the download of revisions concludes, then continue.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginDownloadRevisions</param>
        /// <seealso cref="BeginDownloadRevisions"/>
        public IEnumerable<Revision> EndDownloadRevisions(IAsyncResult asyncResult) {
            AsyncResult<IEnumerable<Revision>, Document> ar = (AsyncResult<IEnumerable<Revision>, Document>)asyncResult;
            return ar.EndInvoke();
        }

        #endregion

        #endregion

        #region Folder

        #region Create Folder

        /// <summary>
        /// Create a new folder
        /// </summary>
        /// <param name="name">Name of folder</param>
        /// <param name="userMail">Email of owning user</param>
        /// <param name="parent">Parent of folder, folders must be placed in folders or projects</param>
        /// <returns>The newly created folder</returns>
        public Folder CreateFolder(String name, string userMail, IItemContainer parent) {
            return fileModel.AddFolder(parent, name);
        }

        /// <summary>
        /// Helper for asynchronously creating folder
        /// </summary>
        /// <param name="asyncResult">AsyncResult&lt;Folder,string,string,IItemContainer&gt;</param>
        private void CreateFolderAsyncHelper(object asyncResult) {
            AsyncResult<Folder, string, string, IItemContainer> ar = (AsyncResult<Folder, string, string, IItemContainer>)asyncResult;
            try {
                Folder result = CreateFolder(ar.Parameter1, ar.Parameter2, ar.Parameter3);
                ar.SetAsCompleted(result, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Start asynchronously creating a folder in accordance with the Asynchronous Programming Model.
        /// </summary>
        /// <param name="name">Name of folder</param>
        /// <param name="userMail">Email of owner of folder</param>
        /// <param name="parent">Parent of folder; folders must be placed in folders or projects</param>
        /// <param name="callback">Callback called upon folder creation</param>
        /// <param name="stateObject">State object (passed to callback)</param>
        /// <returns>IAsyncResult for EndCreateFolder</returns>
        /// <seealso cref="EndCreateFolder"/>
        public IAsyncResult BeginCreateFolder(string name, string userMail, IItemContainer parent, AsyncCallback callback, object stateObject) {
            AsyncResult<Folder, string, string, IItemContainer> ar = new AsyncResult<Folder, string, string, IItemContainer>(callback, stateObject, name, userMail, parent);
            ThreadPool.QueueUserWorkItem(CreateFolderAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Wait for folder creation to finish, then return the folder
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginCreateFolder</param>
        /// <returns>Newly created folder</returns>
        public Folder EndCreateFolder(IAsyncResult asyncResult) {
            AsyncResult<Folder, string, string, IItemContainer> ar = (AsyncResult<Folder, string, string, IItemContainer>)asyncResult;
            return ar.EndInvoke();
        }

        #endregion

        #region Remove Folder

        /// <summary>
        /// Remove a folder
        /// </summary>
        /// <param name="f">Folder to remove</param>
        public void RemoveFolder(Folder f) {
            fileModel.RemoveFolder(f);
        }

        /// <summary>
        /// Helper for asynchronously removing a folder.
        /// </summary>
        /// <param name="asyncResult">AsyncResultNoResult&lt;Folder&gt;</param>
        private void RemoveFolderAsyncHelper(object asyncResult) {
            AsyncResultNoResult<Folder> ar = (AsyncResultNoResult<Folder>)asyncResult;
            try {
                RemoveFolder(ar.Parameter1);
                ar.SetAsCompleted(null, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Start removing a folder asynchronously in accordance to the Asynchronous Programming Model.
        /// </summary>
        /// <param name="f">Folder to remove</param>
        /// <param name="callback">Callback called when folder has been removed</param>
        /// <param name="state">State object (passed to callback)</param>
        /// <returns>IAsyncResult for EndRemoveFolder</returns>
        /// <seealso cref="EndRemoveFolder"/>
        public IAsyncResult BeginRemoveFolder(Folder f, AsyncCallback callback, object state) {
            AsyncResultNoResult<Folder> ar = new AsyncResultNoResult<Folder>(callback, state, f);
            ThreadPool.QueueUserWorkItem(RemoveFolderAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Block while folder is removed, then continue
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginRemoveFolder</param>
        /// <seealso cref="BeginRemoveFolder"/>
        public void EndRemoveFolder(IAsyncResult asyncResult) {
            apmNoResultHelper(asyncResult);
        }

        #endregion

        #region Get Directly

        /// <summary>
        /// Get a project directly by its id
        /// </summary>
        /// <param name="projectId">Id of project to get</param>
        /// <returns>Project corresponding to id</returns>
        public Project GetProjectDirectly(int projectId) {
            return fileModel.GetProject(projectId);
        }

        /// <summary>
        /// Get a folder directly by its id
        /// </summary>
        /// <param name="folderId">Id of folder</param>
        /// <returns>Folder corresponding to id</returns>
        public Folder GetFolderDirectly(int folderId) {
            return fileModel.GetFolder(folderId);
        }

        /// <summary>
        /// Get a document directly by its id
        /// </summary>
        /// <param name="documentId">Id of document</param>
        /// <returns>Document corresponding to id</returns>
        public Document GetDocumentDirectly(int documentId) {
            return fileModel.GetDocument(documentId);
        }

        #endregion

        #endregion

        #region All

        #region Sync Projects

        /// <summary>
        /// Synchronize all files of a user with the remote version.
        /// </summary>
        /// <param name="userMail">Email of user whose projects to sync</param>
        /// <param name="password">Password of user</param>
        /// <returns>Updated collection of projects.</returns>
        /// <seealso cref="BeginSyncProjects"/>
        public IEnumerable<Project> SyncProjects(string userMail, string password) {
            if (!userModel.ValidateLogin(userMail, password)) throw new ArgumentException("Invalid login provided for SynProjects");
            fileModel.SyncFiles(userMail);
            return GetProjects(userMail);
        }

        /// <summary>
        /// Helper for synchronizing projects asynchronously (funny as that sounds).
        /// </summary>
        /// <param name="asyncResult">AsyncResult&lt;IEnumerable&ltProject&gt;,string&gt;</param>
        private void SyncProjectsAsyncHelper(object asyncResult) {
            AsyncResult<IEnumerable<Project>, string, string> ar = (AsyncResult<IEnumerable<Project>, string, string>)asyncResult;
            try {
                IEnumerable<Project> result = SyncProjects(ar.Parameter1, ar.Parameter2);
                ar.SetAsCompleted(result, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Start synchronizing all projects for a user asynchronously, in accordance with the Asynchronous
        /// Programming Model pattern.
        /// </summary>
        /// <param name="userMail">Email of user</param>
        /// <param name="password">Password of user</param>
        /// <param name="callback">Callback called when synchronization finishes</param>
        /// <param name="state">State object, passed to callback</param>
        /// <returns>IAsyncResult for EndSyncProjects</returns>
        /// <seealso cref="EndSyncProjects"/>
        public IAsyncResult BeginSyncProjects(string userMail, string password, AsyncCallback callback, object state) {
            AsyncResult<IEnumerable<Project>, string, string> ar = new AsyncResult<IEnumerable<Project>, string, string>(callback, state, userMail, password);
            ThreadPool.QueueUserWorkItem(SyncProjectsAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Blocks until synchronization of projects has completed, then returns the result.
        /// </summary>
        /// <param name="asyncResult">ASyncResult from BeginSyncProjects</param>
        /// <returns>Updated enumerable of projects</returns>
        /// <seealso cref="BeginSyncProjects"/>
        public IEnumerable<Project> EndSyncProjects(IAsyncResult asyncResult) {
            AsyncResult<IEnumerable<Project>, string, string> ar = (AsyncResult<IEnumerable<Project>, string, string>)asyncResult;
            return ar.EndInvoke();
        }

        #endregion

        #region Get Projects

        /// <summary>
        /// Get An IEnumerable of the project associated with the user denoted by userId
        /// (defaults to User.Local as a convenience for the local client)
        /// </summary>
        /// <returns>returns project related to user.</returns>
        public IEnumerable<Project> GetProjects(string userMail) {
            foreach (Project p in fileModel.GetProjects(userMail)) {
                yield return p;
            }
        }

        /// <summary>
        /// Helper for asynchronously getting projects
        /// </summary>
        /// <param name="asyncResult">AsyncResult&ltIEnumerable&lt;Project&gt;,string&gt;</param>
        private void GetProjectsAsyncHelper(object asyncResult) { //TODO: Use AsyncResultNoResult instead (generic this!)
            AsyncResult<IEnumerable<Project>, string> ar = (AsyncResult<IEnumerable<Project>, string>)asyncResult;
            try {
                IEnumerable<Project> result = GetProjects(ar.Parameter1);
                ar.SetAsCompleted(result, false);
            }
            catch (Exception e) {
                ar.SetAsCompleted(e, false);
            }
        }

        /// <summary>
        /// Start getting all projects from the model asynchronously, in accordance with the APM pattern.
        /// </summary>
        /// <param name="userMail">Email of user whose projects are retrieved</param>
        /// <param name="callback">Callback called when all projects have been retrieved</param>
        /// <param name="state">Passed to callback</param>
        /// <returns>IAsynResult for EndGetProjects</returns>
        /// <seealso cref="EndGetProjects"/>
        public IAsyncResult BeginGetProjects(string userMail, AsyncCallback callback, object state) {
            AsyncResult<IEnumerable<Project>, string> ar = new AsyncResult<IEnumerable<Project>, string>(callback, state, userMail);
            ThreadPool.QueueUserWorkItem(GetProjectsAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Block until all projects have been retrieved, then return them.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginGetProjects</param>
        /// <returns>Enumerable containing all projects</returns>
        /// <seealso cref="BeginGetProjects"/>
        public IEnumerable<Project> EndGetProjects(IAsyncResult asyncResult) {
            AsyncResult<IEnumerable<Project>, string> ar = (AsyncResult<IEnumerable<Project>, string>)asyncResult;
            return ar.EndInvoke();
        }

        #endregion

        #endregion
    }
}