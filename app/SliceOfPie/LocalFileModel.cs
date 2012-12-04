using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SliceOfPie {
    public class LocalFileModel : IFileModel {
        private string AppPath;
        private string DefaultProjectPath;
        private List<Project> Projects = new List<Project>();

        public LocalFileModel() {
            AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SliceOfPie");
            //SyncFiles("me@michaelstorgaard.com");
            CreateStructure();
            FindProjects();
        }

        /// <summary>
        /// Add project to system. If db is set to true, only create folder.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public override Project AddProject(string title, int id = 0, bool db = false) {
            string projectPath = Path.Combine(AppPath, Helper.GenerateName(id, title));
            if (Directory.Exists(projectPath) || File.Exists(projectPath)) {
                if (db) return null;
                throw new ArgumentException("Name is already in use (" + projectPath + ")");
            }
            Directory.CreateDirectory(projectPath);
            if (db) return null;
            Project project = new Project();
            project.Title = title;
            project.AppPath = AppPath;
            Projects.Add(project);
            return project;
        }

        /// <summary>
        /// Rename project both in file system and internal system.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="title"></param>
        public void RenameProject(Project project, string title) {
            string projectPath = Path.Combine(AppPath, Helper.GenerateName(project.Id, title));
            if (Directory.Exists(projectPath) || File.Exists(projectPath)) {
                throw new ArgumentException("Name is already in use (" + projectPath + ")");
            }
            Directory.Move(Path.Combine(AppPath, project.Title), projectPath);
            project.Title = title;
        }

        public override void RemoveProject(Project project) {
            if (!Directory.Exists(project.GetPath())) {
                throw new ArgumentException("Project folder does not exist (" + project.GetPath() + ")");
            }
            Document[] removeDocuments = project.Documents.ToArray();
            foreach (Document document in removeDocuments) {
                RemoveDocument(document);
            }
            Folder[] removeFolders = project.Folders.ToArray();
            foreach (Folder subFolder in removeFolders) {
                RemoveFolder(subFolder);
            }
            Directory.Delete(project.GetPath());
            Projects.Remove(project);
        }

        /// <summary>
        /// Lazily return all projects for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public override IEnumerable<Project> GetProjects(string userMail) {
            foreach (Project project in Projects) {
                yield return project;
            }
        }

        /// <summary>
        /// Add folder to system. If db is set to true, only create folder.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="title"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public override Folder AddFolder(IItemContainer parent, string title, int id = 0, bool db = false) {
            string folderPath = Path.Combine(parent.GetPath(), Helper.GenerateName(id, title));
            if (Directory.Exists(folderPath) || File.Exists(folderPath)) {
                if (db) return null;
                throw new ArgumentException("Name is already in use (" + folderPath + ")");
            }
            Directory.CreateDirectory(folderPath);
            if (db) return null;
            Folder folder = new Folder();
            folder.Title = title;
            folder.Parent = parent;
            parent.Folders.Add(folder);
            return folder;
        }

        /// <summary>
        /// Rename folder both in file system and internal system.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="title"></param>
        public void RenameFolder(Folder folder, string title) {
            string folderPath = Path.Combine(folder.Parent.GetPath(), Helper.GenerateName(folder.Id, title));
            if (Directory.Exists(folderPath) || File.Exists(folderPath)) {
                throw new ArgumentException("Name is already in use (" + folderPath + ")");
            }
            Directory.Move(folder.GetPath(), folderPath);
            folder.Title = title;
        }

        public override void RemoveFolder(Folder folder) {
            if (!Directory.Exists(folder.GetPath())) {
                throw new ArgumentException("Folder does not exist (" + folder.GetPath() + ")");
            }
            Document[] removeDocuments = folder.Documents.ToArray();
            foreach (Document document in removeDocuments) {
                RemoveDocument(document);
            }
            Folder[] removeFolders = folder.Folders.ToArray();
            foreach (Folder subFolder in removeFolders) {
                RemoveFolder(subFolder);
            }
            Directory.Delete(folder.GetPath());
            folder.Parent.Folders.Remove(folder);
        }

        /// <summary>
        /// Add document to system. If db is set to true, only create file.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="title"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public override Document AddDocument(IItemContainer parent, string title, int id = 0, bool db = false) {
            string documentPath = Path.Combine(parent.GetPath(), Helper.GenerateName(id, title) + ".txt");
            if (Directory.Exists(documentPath) || File.Exists(documentPath)) {
                if (db) return null;
                throw new ArgumentException("Name is already in use (" + documentPath + ")");
            }
            if (db) return null;
            Document document = new Document();
            document.Title = title;
            document.Parent = parent;
            parent.Documents.Add(document);
            FileStream fileStream = File.Create(documentPath);
            fileStream.Close();
            return document;
        }

        /// <summary>
        /// Rename file both in file system and internal system.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="title"></param>
        public void RenameDocument(Document document, string title) {
            string documentPath = Path.Combine(document.Parent.GetPath(), Helper.GenerateName(document.Id, title) + ".txt");
            if (Directory.Exists(documentPath) || File.Exists(documentPath)) {
                throw new ArgumentException("Name is already in use(" + documentPath + ")");
            }
            File.Move(Path.Combine(document.Parent.GetPath(), document.Title), document.GetPath());
            document.Title = title;
        }

        /// <summary>
        /// Save document to file. Take CurrentRevision from Document and overwrite existing file.
        /// </summary>
        /// <param name="document"></param>
        public override void SaveDocument(Document document) {
            if (!File.Exists(document.GetPath())) {
                throw new ArgumentException("File does not exist (" + document.GetPath() + ")");
            }
            FileStream fileStream = new FileStream(document.GetPath(), FileMode.Open, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(document.CurrentRevision);
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        public override void RemoveDocument(Document document) {
            if (!File.Exists(document.GetPath())) {
                throw new ArgumentException("File does not exist (" + document.GetPath() + ")");
            }
            File.Delete(document.GetPath());
            document.Parent.Documents.Remove(document);
        }

        /// <summary>
        /// Synchronize all files and folders with db. Upload first then download.
        /// </summary>
        /// <param name="email"></param>
        public void SyncFiles(string email) {
            UploadStructure(email);
            DownloadStructure(email);
        }

        /// <summary>
        /// Upload all files and folders to db for specific user.
        /// </summary>
        /// <param name="email"></param>
        public void UploadStructure(string email) {

        }

        /// <summary>
        /// Download all files and folders from db for specific user.
        /// </summary>
        /// <param name="email"></param>
        public void DownloadStructure(string email) {
            List<Project> projectsContainer = new List<Project>();
            using (var dbContext = new sliceofpieEntities2()) {
                var projects = from projectUser in dbContext.ProjectUsers
                               from project in dbContext.Projects
                               where projectUser.UserEmail == email && projectUser.ProjectId == project.Id
                               select project;
                foreach (Project project in projects) {
                    project.AppPath = AppPath;
                    projectsContainer.Add(project);
                    AddProject(project.Title, project.Id, true);
                }
            }
            foreach (Project project in projectsContainer) {
                List<Folder> foldersContainer = new List<Folder>();
                using (var dbContext = new sliceofpieEntities2()) {
                    var folders = from folder in dbContext.Folders
                                  where folder.ProjectId == project.Id
                                  select folder;
                    foreach (Folder folder in folders) {
                        folder.Parent = project;
                        AddFolder(project, folder.Title, folder.Id, true);
                        foldersContainer.Add(folder);
                    }
                }
                foreach (Folder folder in foldersContainer) {
                    DownloadFolders(folder);
                    DownloadDocuments(folder);
                }
                using (var dbContext = new sliceofpieEntities2()) {
                    var documents = from document in dbContext.Documents
                                    where document.ProjectId == project.Id
                                    select document;
                    foreach (Document document in documents) {
                        document.Parent = project;
                        AddDocument(project, document.Title, document.Id, true);
                    }
                }
            }
        }

        /// <summary>
        /// Download all folders for a specific parent folder. Do this recursively.
        /// </summary>
        /// <param name="parent"></param>
        public void DownloadFolders(Folder parent) {
            List<Folder> foldersContainer = new List<Folder>();
            using (var dbContext = new sliceofpieEntities2()) {
                var folders = from folder in dbContext.Folders
                              where folder.FolderId == parent.Id
                              select folder;
                foreach (Folder folder in folders) {
                    folder.Parent = parent;
                    AddFolder(parent, folder.Title, folder.Id, true);
                    foldersContainer.Add(folder);
                }
            }
            foreach (Folder folder in foldersContainer) {
                DownloadFolders(folder);
                DownloadDocuments(folder);
            }
        }

        /// <summary>
        /// Download all documents for a specific parent folder.
        /// </summary>
        /// <param name="parent"></param>
        public void DownloadDocuments(Folder parent) {
            using (var dbContext = new sliceofpieEntities2()) {
                var documents = from document in dbContext.Documents
                                where document.FolderId == parent.Id
                                select document;
                foreach (Document document in documents) {
                    document.Parent = parent;
                    AddDocument(parent, document.Title, document.Id, true);
                }
            }
        }

        /// <summary>
        /// Create basic structure if not found already. This includes SliceOfPie-folder and default project folder.
        /// </summary>
        public void CreateStructure() {
            if (!Directory.Exists(AppPath)) {
                Directory.CreateDirectory(AppPath);
            }

            if (Directory.GetDirectories(AppPath).Count() == 0) {
                DefaultProjectPath = Path.Combine(AppPath, "0-default");
                if (!Directory.Exists(DefaultProjectPath)) {
                    Directory.CreateDirectory(DefaultProjectPath);
                }
            }
        }

        /// <summary>
        /// Find all projects in file system and create them in internal system.
        /// </summary>
        public void FindProjects() {
            Projects = new List<Project>();

            string[] folders = Directory.GetDirectories(AppPath);
            foreach (string folderName in folders) {
                string pathName = Path.GetFileName(folderName);
                Project project = new Project();
                string[] parts = pathName.Split('-');
                project.Id = int.Parse(parts[0]);
                project.Title = pathName.Replace(parts[0] + "-", "");
                project.AppPath = AppPath;
                Projects.Add(project);

                FindFolders(project);
                FindDocuments(project);
            }
        }

        /// <summary>
        /// Find all folders in project folder and subfolders in file system and create them in internal system.
        /// </summary>
        /// <param name="parent"></param>
        public void FindFolders(IItemContainer parent) {
            string[] folders = Directory.GetDirectories(parent.GetPath());
            foreach (string folderName in folders) {
                string pathName = Path.GetFileName(folderName);
                Folder folder = new Folder();
                string[] parts = pathName.Split('-');
                folder.Id = int.Parse(parts[0]);
                folder.Title = pathName.Replace(parts[0] + "-", "");
                folder.Parent = parent;
                parent.Folders.Add(folder);

                FindFolders(folder);
                FindDocuments(folder);
            }
        }

        /// <summary>
        /// Find all documents in project/folder in file system and create them in internal system.
        /// </summary>
        /// <param name="parent"></param>
        public void FindDocuments(IItemContainer parent) {
            string[] documentPaths = Directory.GetFiles(parent.GetPath());
            foreach (string documentName in documentPaths) {
                string pathName = Path.GetFileName(documentName);
                Document document = new Document();
                string[] parts = pathName.Split('-');
                document.Id = int.Parse(parts[0]);
                document.Title = pathName.Replace(parts[0] + "-", "");
                document.Parent = parent;
                parent.Documents.Add(document);
            }
        }
    }
}
