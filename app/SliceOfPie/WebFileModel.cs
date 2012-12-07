using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace SliceOfPie {
    public class WebFileModel : IFileModel {

        public override IEnumerable<Project> GetProjects(string email) {
            List<Project> projectsContainer = new List<Project>();
            using (var dbContext = new sliceofpieEntities2()) {
                var projects = from projectUser in dbContext.ProjectUsers
                               from project in dbContext.Projects
                               where projectUser.UserEmail.Equals(email) && projectUser.ProjectId == project.Id
                               select project;
                foreach (Project project in projects) {
                    projectsContainer.Add(new Project() {
                        Id = project.Id,
                        Title = project.Title
                    });
                }
            }
            foreach (Project project in projectsContainer) {
                GetFolders(project);
                GetDocuments(project);
            }
            return projectsContainer;
        }

        /// <summary>
        /// Recursively get all folders and their documents. These are saved to the IItemContainer provided,
        /// hence no return value.
        /// </summary>
        /// <param name="parent">Item whose sub-folders to get</param>
        private void GetFolders(IItemContainer parent) {
            List<Folder> folderList = new List<Folder>();
            using (var dbContext = new sliceofpieEntities2()) {
                IQueryable<Folder> folders;
                if (parent is Folder) {
                    folders = from folder in dbContext.Folders
                              where folder.FolderId == parent.Id
                              select folder;
                }
                else {
                    folders = from folder in dbContext.Folders
                              where folder.ProjectId == parent.Id
                              select folder;
                }
                foreach (Folder folder in folders) {
                    folderList.Add(new Folder() {
                        Id = folder.Id,
                        Title = folder.Title,
                        Parent = parent
                    });
                }
            }
            foreach (Folder folder in folderList) {
                parent.Folders.Add(folder);
                GetFolders(folder);
                GetDocuments(folder);
            }
        }

        /// <summary>
        /// Get all documents in a IItemContainer. The documents are saved to the container,
        /// so there is no reason for return values.
        /// </summary>
        /// <param name="parent">Container whose documents to get.</param>
        private void GetDocuments(IItemContainer parent) {
            List<Document> docList = new List<Document>();
            using (var dbContext = new sliceofpieEntities2()) {
                IQueryable<Document> documents;
                if (parent is Folder) {
                    documents = from document in dbContext.Documents
                                where document.FolderId == parent.Id
                                select document;
                }
                else {
                    documents = from document in dbContext.Documents
                                where document.ProjectId == parent.Id
                                select document;
                }
                foreach (Document document in documents) {
                    docList.Add(new Document() {
                        Id = document.Id,
                        Title = document.Title,
                        Parent = parent,
                        CurrentRevision = document.CurrentRevision,
                        CurrentHash = document.CurrentRevision.GetHashCode()
                    });
                }
            }
            foreach (Document document in docList) {
                parent.Documents.Add(document);
            }
        }

        public override Project AddProject(string title, string userMail, int id = 0, bool db = false) {
            Project p = new Project() {
                Title = title
            };
            if (id > 0) p.Id = id;
            using (var dbContext = new sliceofpieEntities2()) {
                dbContext.Projects.AddObject(p);
                dbContext.SaveChanges();
            }
            return p;
        }

        public override void RemoveProject(Project project) {
            throw new NotImplementedException();
        }

        public override Folder AddFolder(IItemContainer parent, string title, int id = 0, bool db = false) {
            throw new NotImplementedException();
        }

        public override void RemoveFolder(Folder folder) {
            throw new NotImplementedException();
        }

        public override Document AddDocument(IItemContainer parent, string title, string revision = "", int id = 0, bool db = false) {
            throw new NotImplementedException();
        }

        public override void SaveDocument(Document document) {
            throw new NotImplementedException();
        }

        public override void RemoveDocument(Document document) {
            throw new NotImplementedException();
        }

        public override void SyncFiles(string userMail) {
            throw new NotSupportedException("Synchronization is not supported from web, as you should simply GetProjects ever time...");
        }
    }
}
