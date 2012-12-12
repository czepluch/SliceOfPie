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

        #region GetProject helpers

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
                GetRevisions(document);
            }
        }

        private void GetRevisions(Document document) {
            List<Revision> revList = new List<Revision>();
            using (var dbContext = new sliceofpieEntities2()) {
                var revisions = dbContext.Revisions.Where(rev => rev.DocumentId == document.Id).OrderByDescending(x => x.Timestamp);
                foreach (Revision r in revisions) {
                    revList.Add(new Revision() {
                        Document = document,
                        Content = r.Content,
                        ContentHash = r.Content.GetHashCode(),
                        Id = r.Id
                    });
                }
            }
            foreach (Revision r in revList) {
                document.Revisions.Add(r);
            }
        }

        #endregion

        public override Project AddProject(string title, string userMail, int id = 0, bool db = false) {
            if (title == null || userMail == null) throw new ArgumentNullException();

            Project p = new Project() {
                Title = title
            };
            using (var dbContext = new sliceofpieEntities2()) { //Insert project
                dbContext.Projects.AddObject(p);
                dbContext.SaveChanges();
            }
            User u = new User() {
                Email = userMail
            };
            ProjectUser pu = new ProjectUser() {
                UserEmail = userMail,
                ProjectId = p.Id
            };
            using (var dbContext = new sliceofpieEntities2()) { //Insert projectUser
                if (dbContext.Users.Count(dbUser => dbUser.Email.Equals(u.Email)) < 1) throw new ArgumentException("No user with email " + u.Email + " exists!");
                dbContext.ProjectUsers.AddObject(pu);
                dbContext.SaveChanges();
            }
            return new Project() {
                Title = p.Title,
                Id = p.Id
            };
        }

        public override void RemoveProject(Project project) {
            if (project == null) throw new ArgumentNullException();
            IEnumerable<Document> documents;
            IEnumerable<Folder> folders;
            using (var dbContext = new sliceofpieEntities2()) {
                Project projectToGetFrom = dbContext.Projects.First(proj => project.Id == proj.Id);
                documents = projectToGetFrom.Documents.ToList();
                folders = projectToGetFrom.Folders.ToList();
            }

            foreach (Document d in documents) {
                RemoveDocument(d);
            }

            foreach (Folder f in folders) {
                RemoveFolder(f);
            }
         
            using (var dbContext = new sliceofpieEntities2()) {
                Project p = dbContext.Projects.First(proj => project.Id == proj.Id);
                ProjectUser pu = dbContext.ProjectUsers.First(user => project.Id == user.ProjectId);
                dbContext.ProjectUsers.DeleteObject(pu);
                dbContext.Projects.DeleteObject(p);
                dbContext.SaveChanges();
            }
        }

        public override Folder AddFolder(IItemContainer parent, string title, int id = 0, bool db = false) {
            if (title == null || parent == null) throw new ArgumentNullException();
            Folder f = new Folder() {
                Title = title,
                Parent = parent
            };
            if (parent is Folder) f.FolderId = parent.Id;
            else f.ProjectId = parent.Id;
            using (var dbContext = new sliceofpieEntities2()) {
                dbContext.Folders.AddObject(f);
                dbContext.SaveChanges();
            }
            return new Folder() {
                Title = title,
                Parent = parent,
                Id = f.Id,
                ProjectId = f.ProjectId,
                FolderId = f.FolderId
            };
        }

        public override void RemoveFolder(Folder folder) {
            if (folder == null) throw new ArgumentNullException();
            IEnumerable<Document> documents;
            IEnumerable<Folder> folders;
            using (var dbContext = new sliceofpieEntities2()) {
                Folder folderToGetFrom = dbContext.Folders.First(fold => folder.Id == fold.Id);
                documents = folderToGetFrom.Documents.ToList();
                folders = folderToGetFrom.Folders.ToList();
            }

            foreach (Document d in documents) {
                RemoveDocument(d);
            }

            foreach (Folder f in folders) {
                RemoveFolder(f);
            }
            
            using (var dbContext = new sliceofpieEntities2()) {
                Folder f = dbContext.Folders.First(fold => fold.Id == folder.Id);
                dbContext.Folders.DeleteObject(f);
                dbContext.SaveChanges();
            }
        }

        public override Document AddDocument(IItemContainer parent, string title, string revision = "", int id = 0, bool db = false) {
            if (parent == null || title == null) throw new ArgumentNullException();
            Document d = new Document() {
                Title = title,
                Parent = parent,
                CurrentRevision = revision,
                CurrentHash = revision.GetHashCode()
            };
            if(parent is Project) d.ProjectId = parent.Id;
            else d.FolderId = parent.Id;

            using(var dbContext = new sliceofpieEntities2()) {
                dbContext.Documents.AddObject(d);
                dbContext.SaveChanges();
            }
            return d;
        }

        public override void SaveDocument(Document document) {
            using (var dbContext = new sliceofpieEntities2()) {
                Revision latestRevFromWeb = dbContext.Revisions.FirstOrDefault(rev => rev.DocumentId == document.Id);

                string merge;

                if (latestRevFromWeb != null)
                    merge = Merger.Merge(document.CurrentRevision, latestRevFromWeb.Content); //Merrrrrge
                else merge = document.CurrentRevision;

                Document d = dbContext.Documents.First(doc => doc.Id == document.Id);
                Revision newRevision = new Revision() {
                    Content = merge,
                    ContentHash = merge.GetHashCode(),
                    Timestamp = DateTime.Now,
                    DocumentId = d.Id
                };
                dbContext.Revisions.AddObject(newRevision);
                d.Revisions.Add(newRevision);
                d.CurrentRevision = merge;
                d.CurrentHash = merge.GetHashCode();
                dbContext.SaveChanges();
            }
        }

        public override void RemoveDocument(Document document) {
            if (document == null) throw new ArgumentNullException();
            using (var dbContext = new sliceofpieEntities2()) {
                IEnumerable<Revision> revisions = dbContext.Documents.First(doc => doc.Id == document.Id).Revisions.ToList();
                foreach (Revision r in revisions) {
                    dbContext.Revisions.DeleteObject(r);
                }
                dbContext.SaveChanges();
            }
            using (var dbContext = new sliceofpieEntities2()) {
                Document d = dbContext.Documents.First(doc => doc.Id == document.Id);
                dbContext.Documents.DeleteObject(d);
                dbContext.SaveChanges();
            }
        }

        public override Project GetProject(int id) {
            Project result;
            using (var dbContext = new sliceofpieEntities2()) {
                Project dbProj = dbContext.Projects.First(p => p.Id == id);
                result = new Project() {
                    Id = dbProj.Id,
                    Title = dbProj.Title
                };
            }
            GetFolders(result);
            GetDocuments(result);
            return result;
        }

        public override Folder GetFolder(int id) {
            Folder result;
            try {
                using (var dbContext = new sliceofpieEntities2()) {
                    Folder dbFolder = dbContext.Folders.First(f => f.Id == id);
                    result = new Folder() {
                        Id = dbFolder.Id,
                        Title = dbFolder.Title
                    };

                }
            } catch (InvalidOperationException e) {
                return null;
            }
            GetFolders(result);
            GetDocuments(result);
            return result;
        }

        public override Document GetDocument(int id) {
            Document result;
            try {
                using (var dbContext = new sliceofpieEntities2()) {
                    Document dbDoc = dbContext.Documents.First(d => d.Id == id);
                    result = new Document() {
                        Id = dbDoc.Id,
                        CurrentRevision = dbDoc.CurrentRevision,
                        CurrentHash = dbDoc.CurrentRevision.GetHashCode(),
                        Title = dbDoc.Title
                    };
                }
            } catch (InvalidOperationException e) {
                return null;
            }
            GetRevisions(result);
            return result;
        }

        public override void SyncFiles(string userMail) {
            throw new NotSupportedException("Synchronization is not supported from web, as you should simply GetProjects ever time...");
        }

        public override IEnumerable<Revision> DownloadRevisions(Document document) {
            throw new NotSupportedException("Not supported in web");
        }
    }
}