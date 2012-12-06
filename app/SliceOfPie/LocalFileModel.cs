using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SliceOfPie {
    internal class LocalFileModel : IFileModel {
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
            foreach (string line in lines) {
                streamWriter.WriteLine(line);
            }
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
        public override void SyncFiles(string email) {
            UploadStructure(email);
            DownloadStructure(email);
        }

        /// <summary>
        /// Upload all files and folders to db for specific user.
        /// </summary>
        /// <param name="email"></param>
        public int UploadStructure(string email) {
            IEnumerable<Project> projects = GetProjects("local");
            foreach (Project project in projects) {
                List<Folder> folders = new List<Folder>();
                List<Document> documents = new List<Document>();
                // Insert or update project
                using (var dbContext = new sliceofpieEntities2()) {
                    folders = project.Folders.ToList();
                    documents = project.Documents.ToList();
                    if (project.Id == 0) {
                        dbContext.Projects.AddObject(project);
                    }
                    dbContext.SaveChanges();
                }
                // Move project due to ID-change
                //Directory.Move(Path.Combine(project.AppPath, "0-" + project.Title), project.GetPath());
                // Set relation between project and user
                using (var dbContext = new sliceofpieEntities2()) {
                    dbContext.ProjectUsers.AddObject(new ProjectUser {
                        ProjectId = project.Id,
                        UserEmail = email
                    });
                    dbContext.SaveChanges();
                }
                // Go through each folder in project
                foreach (Folder folder in folders) {
                    try {
                        List<Folder> subFolders = new List<Folder>();
                        List<Document> subDocuments = new List<Document>();
                        using (var dbContext = new sliceofpieEntities2()) {
                            // Add folder
                            subFolders = folder.Folders.ToList();
                            subDocuments = folder.Documents.ToList();
                            folder.ProjectId = project.Id;
                            folder.FolderId = null;
                            if (folder.Id == 0) {
                                dbContext.Folders.AddObject(folder);
                            }
                            dbContext.SaveChanges();
                        }
                        UploadFolders(folder.Id, subFolders);
                        UploadDocuments(folder.Id, subDocuments);
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
                // Go through each document in project
                foreach (Document document in documents) {
                    Document theirDocument = null;
                    if (document.Id != 0) {
                        using (var dbContext = new sliceofpieEntities2()) {
                            var sDocuments = from sDocument in dbContext.Documents
                                             where sDocument.Id == document.Id
                                             select sDocument;
                            theirDocument = sDocuments.First();
                        }
                    }
                    using (var dbContext = new sliceofpieEntities2()) {
                        // Add document
                        document.ProjectId = project.Id;
                        document.FolderId = null;
                        if (document.Id == 0) {
                            dbContext.Documents.AddObject(document);
                            dbContext.SaveChanges();
                        } else if (theirDocument.CurrentHash == document.CurrentHash) {
                            dbContext.SaveChanges();
                        } else {
                            // Handle conflict here
                        }
                    }
                    // Insert revision
                    using (var dbContext = new sliceofpieEntities2()) {
                        dbContext.Revisions.AddObject(new Revision {
                            DocumentId = document.Id,
                            Content = document.CurrentRevision,
                            ContentHash = document.CurrentRevision.GetHashCode()
                        });
                        dbContext.SaveChanges();
                    }
                }
            }
            return 0;
        }

        public void UploadFolders(int folderId, List<Folder> folders) {
            foreach (Folder folder in folders) {
                List<Folder> subFolders = new List<Folder>();
                List<Document> subDocuments = new List<Document>();
                using (var dbContext = new sliceofpieEntities2()) {
                    // Add folder
                    subFolders = folder.Folders.ToList();
                    subDocuments = folder.Documents.ToList();
                    folder.ProjectId = null;
                    folder.FolderId = folderId;
                    if (folder.Id == 0) {
                        dbContext.Folders.AddObject(folder);
                    }
                    dbContext.SaveChanges();
                }
                UploadFolders(folder.Id, subFolders);
                UploadDocuments(folder.Id, subDocuments);
            }
        }

        public void UploadDocuments(int folderId, List<Document> documents) {
            foreach (Document document in documents) {
                Document theirDocument = null;
                if (document.Id != 0) {
                    using (var dbContext = new sliceofpieEntities2()) {
                        var sDocuments = from sDocument in dbContext.Documents
                                        where sDocument.Id == document.Id
                                        select sDocument;
                        theirDocument = sDocuments.First();
                    }
                }
                using (var dbContext = new sliceofpieEntities2()) {
                    // Add document
                    document.ProjectId = null;
                    document.FolderId = folderId;
                    if (document.Id == 0) {
                        dbContext.Documents.AddObject(document);
                        dbContext.SaveChanges();
                    } else if (theirDocument.CurrentHash == document.CurrentHash) {
                        dbContext.SaveChanges();
                    } else {
                        // Handle conflict here
                    }
                }
                // Insert revision
                using (var dbContext = new sliceofpieEntities2()) {
                    dbContext.Revisions.AddObject(new Revision {
                        DocumentId = document.Id,
                        Content = document.CurrentRevision,
                        ContentHash = document.CurrentRevision.GetHashCode()
                    });
                    dbContext.SaveChanges();
                }
            }
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
                        string[] lines = document.CurrentRevision.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                        document.CurrentRevision = lines[0] + "\n" + document.CurrentRevision;
                        AddDocument(project, document.Title, document.CurrentRevision, document.Id, true);
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
                    string[] lines = document.CurrentRevision.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    document.CurrentRevision = lines[0] + "\n" + document.CurrentRevision;
                    AddDocument(parent, document.Title, document.CurrentRevision, document.Id, true);
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
                int hash = "".GetHashCode();
                string revision = "";
                FileStream fileStream = new FileStream(documentName, FileMode.Open, FileAccess.Read);
                StreamReader streamReader = new StreamReader(fileStream);
                string line;
                int i = 0;
                while ((line = streamReader.ReadLine()) != null) {
                    if (i == 0) {
                        if (line.Length > 0) {
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
                    CurrentHash = (id == 0 ? revision.GetHashCode() : hash)
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
    }
}
