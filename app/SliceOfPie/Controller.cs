﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SliceOfPie.ApmHelpers;

namespace SliceOfPie {

    /// <summary>
    /// Controller of SliceOfPie.
    /// </summary>
    public class Controller {

        private static Controller _instance = new Controller();

        /// <summary>
        /// Get the single Controller instance.
        /// </summary>
        public static Controller Instance {
            get {
                return _instance;
            }
        }

        private IFileModel fileModel;

        /// <summary>
        /// Create a new instance of a controller. Made private as to only allow one instance
        /// to be created. External sources should get the controller from <c>Controller.Instance;</c>
        /// </summary>
        /// <param name="model">FileModel to use (local or web?)</param>
        private Controller(IFileModel model = null) {
            if (model == null) model = new LocalFileModel();
            fileModel = model;
        }

        #region Project

        #region Create Project

        /// <summary>
        /// Create a new project synchronously.
        /// </summary>
        /// <param name="name">Name of project</param>
        /// <param name="userMail">Email of user</param>
        /// <returns>The new project</returns>
        /// <see cref="BeginCreateProject"/>
        public Project CreateProject(string name, string userMail) {
            return fileModel.AddProject(name);
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
        /// Create a project asynchronously, according to the Asynchronous Programming Model (see 
        /// http://msdn.microsoft.com/en-us/library/ms228963.aspx)
        /// </summary>
        /// <param name="name">Name of the project to create</param>
        /// <param name="userMail">Email of the user who owns the project</param>
        /// <param name="callback">Callback delegate, called on completion</param>
        /// <param name="stateObject">State object (not used)</param>
        /// <returns><c>IAsyncResult</c> to be used with <c>EndCreateProject</c></returns>
        /// <see cref="EndCreateProject"/>
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
        /// <see cref="BeginRemoveProject"/>
        public void RemoveProject(Project p) {
            fileModel.RemoveProject(p);
        }

        /// <summary>
        /// Helper to remove project asynchronously
        /// </summary>
        /// <param name="asyncResult"><c>AsyncResult&lt;object,Project&gt;</c></param>
        private void RemoveProjectAsyncHelper(object asyncResult) { //TODO: Use AsyncResultNoResult instead (generic this!)
            AsyncResult<object,Project> ar = (AsyncResult<object,Project>)asyncResult;
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
        /// <param name="state">State object, not used.</param>
        /// <returns>IAsyncResult used by EndRemoveProject</returns>
        /// <see cref="EndRemoveProject"/>
        public IAsyncResult BeginRemoveProject(Project p, AsyncCallback callback, object state) {
            AsyncResult<object, Project> ar = new AsyncResult<object, Project>(callback, state, p);
            ThreadPool.QueueUserWorkItem(RemoveProjectAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Block until asynchronous removal of project is underway.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult made by BeginRemoveProject</param>
        /// <see cref="BeginRemoveProject"/>
        public void EndRemoveProject(IAsyncResult asyncResult) {
            AsyncResult<object, Project> ar = (AsyncResult<object, Project>)asyncResult;
            ar.EndInvoke();
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
            throw new NotImplementedException();
        }

        #endregion

        #region SyncProject

        /// <summary>
        /// Synchronize the project with the online version
        /// </summary>
        /// <param name="p">Project to synchronize</param>
        public void SyncProject(Project p) {
            //story cool, yes
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
        /// <see cref="BeginCreateDocument"/>
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
        /// <param name="stateObject">state object (not used)</param>
        /// <returns>IAsyncResult to be used in EndCreateDocument</returns>
        /// <see cref="EndCreateDocument"/>
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
        /// <see cref="BeginCreateDocument"/>
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
        /// <param name="asyncResult">AsyncResult&lt;object,Document&gt;</param>
        private void SaveDocumentAsyncHelper(object asyncResult) { //TODO: Use AsyncResultNoResult instead (generic this!)
            AsyncResult<object, Document> ar = (AsyncResult<object, Document>)asyncResult;
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
        /// <param name="state">State object, not used</param>
        /// <returns>IAsyncResult for EndSaveDocument</returns>
        /// <see cref="EndSaveDocument"/>
        public IAsyncResult BeginSaveDocument(Document d, AsyncCallback callback, object state) {
            AsyncResult<object, Document> ar = new AsyncResult<object, Document>(callback, state, d);
            ThreadPool.QueueUserWorkItem(RemoveDocumentAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Block until the document saving concludes, then continue.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginSaveDocument</param>
        /// <see cref="BeginSaveDocument"/>
        public void EndSaveDocument(IAsyncResult asyncResult) {
            AsyncResult<object, Document> ar = (AsyncResult<object, Document>)asyncResult;
            ar.EndInvoke();
        }

        #endregion

        #region Remove Document

        /// <summary>
        /// Remove a document from the system.
        /// </summary>
        /// <param name="d">Document to remove</param>
        /// <see cref="BeginRemoveDocument"/>
        public void RemoveDocument(Document d) {
            fileModel.RemoveDocument(d);
        }

        /// <summary>
        /// Helper for asynchronously removing documents.
        /// </summary>
        /// <param name="asyncResult">AsyncResult&lt;object,Document&gt;</param>
        private void RemoveDocumentAsyncHelper(object asyncResult) { //TODO: Use AsyncResultNoResult instead (generic this!)
            AsyncResult<object, Document> ar = (AsyncResult<object, Document>)asyncResult;
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
        /// <see cref="EndRemoveDocument"/>
        public IAsyncResult BeginRemoveDocument(Document d, AsyncCallback callback, object state) {
            AsyncResult<object, Document> ar = new AsyncResult<object, Document>(callback, state, p);
            ThreadPool.QueueUserWorkItem(RemoveDocumentAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Wait for removal of document to finish, then continue (block while waiting).
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginRemoveDocument</param>
        /// <see cref="BeginRemoveDocument"/>
        public void EndRemoveDocument(IAsyncResult asyncResult) {
            AsyncResult<object, Document> ar = (AsyncResult<object, Document>)asyncResult;
            ar.EndInvoke();
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
        /// <see cref="EndCreateFolder"/>
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
        /// <param name="asyncResult">AsyncResult&lt;object,Folder&gt;</param>
        private void RemoveFolderAsyncHelper(object asyncResult) { //TODO: Use AsyncResultNoResult instead (generic this!)
            AsyncResult<object, Folder> ar = (AsyncResult<object, Folder>)asyncResult;
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
        /// <see cref="EndRemoveFolder"/>
        public IAsyncResult BeginRemoveFolder(Folder f, AsyncCallback callback, object state) {
            AsyncResult<object, Folder> ar = new AsyncResult<object, Folder>(callback, state, f);
            ThreadPool.QueueUserWorkItem(RemoveFolderAsyncHelper, ar);

            return ar;
        }

        /// <summary>
        /// Block while project is removed, then continue
        /// </summary>
        /// <param name="asyncResult">IAsyncResult from BeginRemoveFolder</param>
        /// <see cref="BeginRemoveFolder"/>
        public void EndRemoveProject(IAsyncResult asyncResult) {
            AsyncResult<object, Folder> ar = (AsyncResult<object, Folder>)asyncResult;
            ar.EndInvoke();
        }

        #endregion

        #endregion


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
    }
}