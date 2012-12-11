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
        private List<Document> ReviewDocuments = new List<Document>();

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
        public override Project AddProject(string title, string userMail, int id = 0, bool db = false) {
            if (!db) title = GetAvailableName(title, id, AppPath);
            string projectPath = Path.Combine(AppPath, Helper.GenerateName(id, title));
            try {
                Directory.CreateDirectory(projectPath);
            } catch (IOException e) {
                // Should not be accesible
                Console.WriteLine(e.Message);
            }
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
            string projectPath = Path.Combine(AppPath, Helper.GenerateName(project.Id, GetAvailableName(title, project.Id, AppPath)));
            try {
                Directory.Move(project.GetPath(), projectPath);
            } catch (IOException e) {
                // Should not be accesible
                Console.WriteLine(e.Message);
            }
            project.Title = title;
        }

        public override void RemoveProject(Project project) {
            if (Projects.Count() == 1) {
                throw new ArgumentException("You cannot delete all projects");
            }
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
            if (!db) title = GetAvailableName(title, id, parent.GetPath());
            string folderPath = Path.Combine(parent.GetPath(), Helper.GenerateName(id, title));
            try {
                Directory.CreateDirectory(folderPath);
            } catch (IOException e) {
                // Should not be accesible
                Console.WriteLine(e.Message);
            }
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
            string folderPath = Path.Combine(folder.Parent.GetPath(), Helper.GenerateName(folder.Id, GetAvailableName(title, folder.Id, folder.Parent.GetPath())));
            try {
                Directory.Move(folder.GetPath(), folderPath);
            } catch (IOException e) {
                // Should not be accesible
                Console.WriteLine(e.Message);
            }
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
        public override Document AddDocument(IItemContainer parent, string title, string revision = "", int id = 0, bool db = false) {
            if (!db) title = GetAvailableName(title, id, parent.GetPath(), ".txt");
            string documentPath = Path.Combine(parent.GetPath(), Helper.GenerateName(id, title));
            documentPath += ".txt";
            try {
                FileStream fileStream = File.Create(documentPath);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                string[] lines = revision.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (string line in lines) {
                    streamWriter.WriteLine(line);
                }
                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();
            } catch (IOException e) {
                // Should not be accesible
                Console.WriteLine(e.Message);
            }
            if (db) return null;
            Document document = new Document();
            document.Title = title;
            document.Parent = parent;
            parent.Documents.Add(document);
            return document;
        }

        /// <summary>
        /// Rename file both in file system and internal system.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="title"></param>
        public void RenameDocument(Document document, string title) {
            string documentPath = Path.Combine(document.Parent.GetPath(), Helper.GenerateName(document.Id, GetAvailableName(title, document.Id, document.Parent.GetPath(), ".txt"))) + ".txt";
            try {
                File.Move(document.GetPath(), documentPath);
            } catch (IOException e) {
                // Should not be accesible
                Console.WriteLine(e.Message);
            }
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
            FileStream fileStream = new FileStream(document.GetPath(), FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            string[] lines = document.CurrentRevision.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            streamWriter.WriteLine(document.CurrentHash);
            foreach (string line in lines) {
                streamWriter.WriteLine(line);
            }
            streamWriter.Flush();
            streamWriter.Close();
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
        public override void SyncFiles(string email) {
            UploadStructure(email);
            DownloadStructure(email);
            Projects = new List<Project>();
            FindProjects();
        }

        /// <summary>
        /// Upload all files and folders to db for specific user.
        /// </summary>
        /// <param name="email"></param>
        public void UploadStructure(string email) {
            // Projects
            string[] folders = Directory.GetDirectories(AppPath);
            foreach (string folderName in folders) {
                Project dbProject = null;
                using (var dbContext = new sliceofpieEntities2()) {
                    string pathName = Path.GetFileName(folderName);
                    string[] parts = pathName.Split('-');
                    int id = int.Parse(parts[0]);
                    string title = pathName.Replace(parts[0] + "-", "");

                    var dbProjects = from dProject in dbContext.Projects
                                     where dProject.Id == id
                                     select dProject;
                    if (id > 0 && dbProjects.Count() == 0) {
                        if (Directory.Exists(Path.Combine(AppPath, Helper.GenerateName(id, title)))) {
                            Directory.Move(Path.Combine(AppPath, Helper.GenerateName(id, title)), Path.Combine(AppPath, Helper.GenerateName(0, title)));
                        }
                        id = 0;
                    }
                    if (id > 0) {
                        // Updating project
                        dbProject = dbProjects.First();
                        dbProject.Title = title;
                    } else {
                        // Creating project
                        dbProject = new Project {
                            Title = title
                        };
                        dbContext.Projects.AddObject(dbProject);
                    }
                    dbContext.SaveChanges();
                }
                // Rename project directory
                string projectPath = Path.Combine(AppPath, Helper.GenerateName(dbProject.Id, dbProject.Title));
                if (Directory.Exists(Path.Combine(AppPath, Helper.GenerateName(0, dbProject.Title)))) {
                    Directory.Move(Path.Combine(AppPath, Helper.GenerateName(0, dbProject.Title)), projectPath);
                }
                // Create project user
                using (var dbContext = new sliceofpieEntities2()) {
                    var dbProjectUsers = from dProjectUser in dbContext.ProjectUsers
                                         where dProjectUser.ProjectId == dbProject.Id && dProjectUser.UserEmail == email
                                         select dProjectUser;
                    if (dbProjectUsers.Count() == 0) {
                        dbContext.ProjectUsers.AddObject(new ProjectUser {
                            ProjectId = dbProject.Id,
                            UserEmail = email
                        });
                        dbContext.SaveChanges();
                    }
                }

                // Upload folders and documents
                var path = Path.Combine(AppPath, Helper.GenerateName(dbProject.Id, dbProject.Title));
                UploadFolders(path, dbProject.Id, Container.Project);
                UploadDocuments(path, dbProject.Id, Container.Project);
            }
        }

        /// <summary>
        /// Upload folders to db for project or folder.
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="parentId"></param>
        /// <param name="container"></param>
        public void UploadFolders(string parentPath, int parentId, Container container = Container.Folder) {
            string[] folders = Directory.GetDirectories(parentPath);
            foreach (string folderName in folders) {
                Folder dbFolder = null;
                using (var dbContext = new sliceofpieEntities2()) {
                    string pathName = Path.GetFileName(folderName);
                    string[] parts = pathName.Split('-');
                    int id = int.Parse(parts[0]);
                    string title = pathName.Replace(parts[0] + "-", "");

                    var dbFolders = from dFolder in dbContext.Folders
                                    where dFolder.Id == id
                                    select dFolder;
                    if (id > 0 && dbFolders.Count() == 0) {
                        if (Directory.Exists(Path.Combine(parentPath, Helper.GenerateName(id, title)))) {
                            Directory.Move(Path.Combine(parentPath, Helper.GenerateName(id, title)), Path.Combine(parentPath, Helper.GenerateName(0, title)));
                        }
                        id = 0;
                    }
                    if (id > 0) {
                        // Updating folder
                        dbFolder = dbFolders.First();
                        dbFolder.Title = title;
                        if (container == Container.Project) {
                            dbFolder.ProjectId = parentId;
                            dbFolder.FolderId = null;
                        } else {
                            dbFolder.ProjectId = null;
                            dbFolder.FolderId = parentId;
                        }
                    } else {
                        // Creating folder
                        dbFolder = new Folder {
                            Title = title
                        };
                        if (container == Container.Project) {
                            dbFolder.ProjectId = parentId;
                            dbFolder.FolderId = null;
                        } else {
                            dbFolder.ProjectId = null;
                            dbFolder.FolderId = parentId;
                        }
                        dbContext.Folders.AddObject(dbFolder);
                    }
                    dbContext.SaveChanges();
                }
                // Rename folder directory
                string folderPath = Path.Combine(parentPath, Helper.GenerateName(dbFolder.Id, dbFolder.Title));
                if (Directory.Exists(Path.Combine(parentPath, Helper.GenerateName(0, dbFolder.Title)))) {
                    Directory.Move(Path.Combine(parentPath, Helper.GenerateName(0, dbFolder.Title)), folderPath);
                }
                // Recursively
                UploadFolders(folderPath, dbFolder.Id);
                UploadDocuments(folderPath, dbFolder.Id);
            }
        }

        /// <summary>
        /// Upload documents to db for project or folder.
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="parentId"></param>
        /// <param name="container"></param>
        public void UploadDocuments(string parentPath, int parentId, Container container = Container.Folder) {
            string[] files = Directory.GetFiles(parentPath);
            foreach (string fileName in files) {
                Document dbDocument = null;
                using (var dbContext = new sliceofpieEntities2()) {
                    string pathName = Path.GetFileName(fileName);
                    string[] parts = pathName.Split('-');
                    int id = int.Parse(parts[0]);
                    string title = pathName.Replace(parts[0] + "-", "").Replace(".txt", "");
                    int hash = "".GetHashCode();
                    string revision = "";
                    bool isRevision = false;
                    FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    StreamReader streamReader = new StreamReader(fileStream);
                    string line;
                    int i = 0;
                    while ((line = streamReader.ReadLine()) != null) {
                        if (i == 0) {
                            if (line.Length > 0) {
                                if (line.Substring(0, 3).Equals("rev")) {
                                    isRevision = true;
                                    line = line.Substring(3);
                                }
                                hash = int.Parse(line);
                            }
                        } else {
                            revision += line + "\n";
                        }
                        i++;
                    }
                    revision = revision.Trim();
                    streamReader.Close();

                    var dbDocuments = from dDocument in dbContext.Documents
                                      where dDocument.Id == id
                                      select dDocument;
                    if (id > 0 && dbDocuments.Count() == 0) {
                        if (Directory.Exists(Path.Combine(parentPath, Helper.GenerateName(id, title)) + ".txt")) {
                            Directory.Move(Path.Combine(parentPath, Helper.GenerateName(id, title)) + ".txt", Path.Combine(parentPath, Helper.GenerateName(0, title)) + ".txt");
                        }
                        id = 0;
                    }
                    if (id > 0) {
                        // Updating document
                        dbDocument = dbDocuments.First();
                        dbDocument.Title = title;
                        if (container == Container.Project) {
                            dbDocument.ProjectId = parentId;
                            dbDocument.FolderId = null;
                        } else {
                            dbDocument.ProjectId = null;
                            dbDocument.FolderId = parentId;
                        }
                        dbDocument.IsMerged = isRevision;
                        if (dbDocument.CurrentHash == hash) {
                            dbDocument.CurrentRevision = revision;
                            dbDocument.CurrentHash = revision.GetHashCode();
                            UpdateHash(fileName, revision.GetHashCode());
                        } else {
                            // Handle merge (and conflicts)
                            string merge = Merger.Merge(revision, dbDocument.CurrentRevision);
                            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                            StreamWriter streamWriter = new StreamWriter(fs);
                            string[] lines = merge.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                            streamWriter.WriteLine("rev" + dbDocument.CurrentHash);
                            foreach (string l in lines) {
                                streamWriter.WriteLine(l);
                            }
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                    } else {
                        // Creating document
                        dbDocument = new Document {
                            Title = title,
                            CurrentRevision = revision,
                            CurrentHash = revision.GetHashCode()
                        };
                        if (container == Container.Project) {
                            dbDocument.ProjectId = parentId;
                            dbDocument.FolderId = null;
                        } else {
                            dbDocument.ProjectId = null;
                            dbDocument.FolderId = parentId;
                        }
                        UpdateHash(fileName, revision.GetHashCode());
                        dbContext.Documents.AddObject(dbDocument);
                    }
                    dbContext.SaveChanges();
                }
                // Rename document file
                if (File.Exists(Path.Combine(parentPath, Helper.GenerateName(0, dbDocument.Title + ".txt")))) {
                    File.Move(Path.Combine(parentPath, Helper.GenerateName(0, dbDocument.Title + ".txt")), Path.Combine(parentPath, Helper.GenerateName(dbDocument.Id, dbDocument.Title + ".txt")));
                }
                // Create revision
                using (var dbContext = new sliceofpieEntities2()) {
                    var dbRevisions = from dRevision in dbContext.Revisions
                                      where dRevision.DocumentId == dbDocument.Id && dRevision.ContentHash == dbDocument.CurrentHash
                                      select dRevision;
                    if (dbRevisions.Count() == 0) {
                        dbContext.Revisions.AddObject(new Revision {
                            DocumentId = dbDocument.Id,
                            Content = dbDocument.CurrentRevision,
                            ContentHash = dbDocument.CurrentHash,
                            Timestamp = DateTime.Now
                        });
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Update the hash of a file (rewriting first line).
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hash"></param>
        public void UpdateHash(string fileName, int hash) {
            List<string> lines = new List<string>();
            lines.Add(hash + "");
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream);
            string line;
            int i = 0;
            while ((line = streamReader.ReadLine()) != null) {
                if (i > 0) {
                    lines.Add(line);
                }
                i++;
            }
            streamReader.Close();

            fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            foreach (string cLine in lines) {
                streamWriter.WriteLine(cLine);
            }
            streamWriter.Close();
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
                    AddProject(project.Title, email, project.Id, true);
                }
            }
            foreach (Project project in projectsContainer) {
                DownloadFolders(project, Container.Project);
                DownloadDocuments(project, Container.Project);
            }
        }

        /// <summary>
        /// Download all folders for a specific parent folder. Do this recursively.
        /// </summary>
        /// <param name="parent"></param>
        public void DownloadFolders(IItemContainer parent, Container container = Container.Folder) {
            List<Folder> foldersContainer = new List<Folder>();
            using (var dbContext = new sliceofpieEntities2()) {
                IEnumerable<Folder> folders;
                if (container == Container.Project) {
                    folders = from folder in dbContext.Folders
                              where folder.ProjectId == parent.Id
                              select folder;
                } else {
                    folders = from folder in dbContext.Folders
                              where folder.FolderId == parent.Id
                              select folder;
                }
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
        public void DownloadDocuments(IItemContainer parent, Container container = Container.Folder) {
            using (var dbContext = new sliceofpieEntities2()) {
                IEnumerable<Document> documents;
                if (container == Container.Project) {
                    documents = from document in dbContext.Documents
                                where document.ProjectId == parent.Id
                                select document;
                } else {
                    documents = from document in dbContext.Documents
                                where document.FolderId == parent.Id
                                select document;
                }
                foreach (Document document in documents) {
                    document.Parent = parent;
                    document.IsMerged = false;
                    if (File.Exists(document.GetPath())) {
                        FileStream fileStream = new FileStream(document.GetPath(), FileMode.Open, FileAccess.Read);
                        StreamReader streamReader = new StreamReader(fileStream);
                        string line;
                        int i = 0;
                        while ((line = streamReader.ReadLine()) != null) {
                            if (i == 0) {
                                if (line.Length > 0) {
                                    if (line.Substring(0, 3).Equals("rev")) {
                                        document.IsMerged = true;
                                    }
                                }
                            }
                            i++;
                        }
                        streamReader.Close();
                    }
                    if (!document.IsMerged) {
                        AddDocument(parent, document.Title, document.CurrentHash + "\n" + document.CurrentRevision, document.Id, true);
                    }
                }
            }
        }

        /// <summary>
        /// Download revisions for a document.
        /// </summary>
        /// <param name="document"></param>
        public override IEnumerable<Revision> DownloadRevisions(Document document) {
            if (document.Id == 0) {
                throw new ArgumentException("Document has to be synced, before being able to retrieve revisions");
            }
            List<Revision> documentRevisions = new List<Revision>();
            using (var dbContext = new sliceofpieEntities2()) {
                var revisions = from revision in dbContext.Revisions
                                where revision.DocumentId == document.Id
                                orderby revision.Timestamp ascending
                                select revision;
                foreach (Revision revision in revisions) {
                    documentRevisions.Add(revision);
                }
            }
            foreach (Revision revision in documentRevisions) {
                yield return revision;
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
                Directory.CreateDirectory(DefaultProjectPath);
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
                string[] parts = pathName.Split('-');
                Project project = new Project {
                    Id = int.Parse(parts[0]),
                    Title = pathName.Replace(parts[0] + "-", ""),
                    AppPath = AppPath
                };
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
                string[] parts = pathName.Split('-');
                Folder folder = new Folder {
                    Id = int.Parse(parts[0]),
                    Title = pathName.Replace(parts[0] + "-", ""),
                    Parent = parent
                };
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
                string[] parts = pathName.Split('-');
                int id = int.Parse(parts[0]);
                bool isRevision = false;
                int hash = "".GetHashCode();
                string revision = "";
                FileStream fileStream = new FileStream(documentName, FileMode.Open, FileAccess.Read);
                StreamReader streamReader = new StreamReader(fileStream);
                string line;
                int i = 0;
                while ((line = streamReader.ReadLine()) != null) {
                    if (i == 0) {
                        if (line.Length > 0) {
                            if (line.Substring(0, 3).Equals("rev")) {
                                isRevision = true;
                                line = line.Substring(3);
                            }
                            hash = int.Parse(line);
                        }
                    } else {
                        revision += line + "\n";
                    }
                    i++;
                }
                streamReader.Close();
                fileStream.Close();
                Document document = new Document {
                    Id = id,
                    Title = pathName.Replace(parts[0] + "-", "").Replace(".txt", ""),
                    Parent = parent,
                    CurrentRevision = revision,
                    CurrentHash = (id == 0 ? revision.GetHashCode() : hash),
                    IsMerged = isRevision
                };
                parent.Documents.Add(document);
            }
        }

        public string GetAvailableName(string title, int id, string path, string ext = "") {
            string name = Path.Combine(path, Helper.GenerateName(id, title)) + ext;
            if (!Directory.Exists(name) && !File.Exists(name)) {
                return title;
            }
            Match match = Regex.Match(title, @"(.*)-([0-9]+)", RegexOptions.IgnoreCase);
            if (match.Success) {
                int num = int.Parse(match.Groups[2].Value) + 1;
                return GetAvailableName(match.Groups[1].Value + "-" + num, id, path, ext);
            }
            return GetAvailableName(title + "-1", id, path, ext);
        }

        public override Project GetProject(int id) {
            throw new InvalidOperationException("Not supported in local file system");
        }

        public override Folder GetFolder(int id) {
            throw new InvalidOperationException("Not supported in local file system");
        }

        public override Document GetDocument(int id) {
            throw new InvalidOperationException("Not supported in local file system");
        }
    }
}
