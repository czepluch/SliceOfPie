using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class WebFileModel : IFileModel {

        public override IEnumerable<Project> GetProjects(string email) {
            List<Project> projectsContainer = new List<Project>();
            using (var dbContext = new sliceofpieEntities2()) {
                var projects = from projectUser in dbContext.ProjectUsers
                               from project in dbContext.Projects
                               where projectUser.UserEmail == email && projectUser.ProjectId == project.Id
                               select project;
                foreach (Project project in projects) { // Get all folders from project
                    projectsContainer.Add(project);
                    GetFolders(project);
                    GetDocuments(project);
                }
            }
            return projectsContainer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="dbContext"></param>
        private void GetFolders(Project project) {
            using (var dbContext = new sliceofpieEntities2()) {
                var folders = from folder in dbContext.Folders
                              where folder.ProjectId == project.Id
                              select folder;
                foreach (Folder folder in folders) {
                    folder.Parent = project;
                    project.Folders.Add(folder);
                    GetFolders(folder);
                    GetDocuments(folder);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="dbContext"></param>
        private void GetDocuments(Project project) {
            using (var dbContext = new sliceofpieEntities2()) {
                var documents = from document in dbContext.Documents
                                where document.ProjectId == project.Id
                                select document;
                foreach (Document document in documents) {
                    document.Parent = project;
                    project.Documents.Add(document);
                }
            }
        }

        /// <summary>
        /// Recurrrrsive
        /// </summary>
        /// <param name="folder"></param>
        private void GetFolders(Folder parent) {
            using (var dbContext = new sliceofpieEntities2()) {
                var folders = from folder in dbContext.Folders
                              where folder.FolderId == parent.Id
                              select folder;
                foreach (Folder folder in folders) {
                    folder.Parent = parent;
                    parent.Folders.Add(folder);
                    GetFolders(folder);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        private void GetDocuments(Folder parent) {
            using (var dbContext = new sliceofpieEntities2()) {
                var documents = from document in dbContext.Documents
                                where document.FolderId == parent.Id
                                select document;
                foreach (Document document in documents) {
                    document.Parent = parent;
                    parent.Documents.Add(document);
                }
            }
        }

        public override Project AddProject(string title, int id = 0, bool db = false) {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
