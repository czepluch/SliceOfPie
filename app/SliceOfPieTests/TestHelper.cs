using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie.Tests {
    public static class TestHelper {
        public static void ClearDatabase(string email) {
            List<Project> projectsContainer = new List<Project>();
            List<ProjectUser> projectUsersContainer = new List<ProjectUser>();
            using (var dbContext = new sliceofpieEntities2()) {
                var projects = from projectUser in dbContext.ProjectUsers
                               from project in dbContext.Projects
                               where projectUser.UserEmail == email && projectUser.ProjectId == project.Id
                               select new { projectUser, project };
                foreach (var project in projects) {
                    projectsContainer.Add(project.project);
                    projectUsersContainer.Add(project.projectUser);
                }
            }
            foreach (ProjectUser projectUser in projectUsersContainer) {
            }
            foreach (Project project in projectsContainer) {
                ClearDatabaseFolders(project, Container.Project);
                ClearDatabaseDocuments(project, Container.Project);
                using (var dbContext = new sliceofpieEntities2()) {
                    var projectUsers = from projectUser in dbContext.ProjectUsers
                                       where projectUser.UserEmail == email && projectUser.ProjectId == project.Id
                                       select projectUser;
                    dbContext.ProjectUsers.DeleteObject(projectUsers.First());
                    dbContext.SaveChanges();
                }
                using (var dbContext = new sliceofpieEntities2()) {
                    var projects = from dbProject in dbContext.Projects
                                   where dbProject.Id == project.Id
                                   select dbProject;
                    dbContext.Projects.DeleteObject(projects.First());
                    dbContext.SaveChanges();
                }
            }
        }

        private static void ClearDatabaseFolders(IItemContainer parent, Container container = Container.Folder) {
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
                    foldersContainer.Add(folder);
                }
            }
            foreach (Folder folder in foldersContainer) {
                ClearDatabaseFolders(folder);
                ClearDatabaseDocuments(folder);
                using (var dbContext = new sliceofpieEntities2()) {
                    var folders = from dbFolder in dbContext.Folders
                                  where dbFolder.Id == folder.Id
                                  select dbFolder;
                    dbContext.Folders.DeleteObject(folders.First());
                    dbContext.SaveChanges();
                }
            }
        }

        private static void ClearDatabaseDocuments(IItemContainer parent, Container container = Container.Folder) {
            List<Document> documentsContainer = new List<Document>();
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
                    documentsContainer.Add(document);
                }
            }
            foreach (Document document in documentsContainer) {
                ClearDatabaseRevisions(document);
                using (var dbContext = new sliceofpieEntities2()) {
                    var documents = from dbDocument in dbContext.Documents
                                    where dbDocument.Id == document.Id
                                    select dbDocument;
                    dbContext.Documents.DeleteObject(documents.First());
                    dbContext.SaveChanges();
                }
            }
        }

        private static void ClearDatabaseRevisions(Document document) {
            using (var dbContext = new sliceofpieEntities2()) {
                var revisions = from revision in dbContext.Revisions
                                where revision.DocumentId == document.Id
                                select revision;
                foreach (Revision revision in revisions) {
                    dbContext.Revisions.DeleteObject(revision);
                }
                dbContext.SaveChanges();
            }
        }

        public static void ClearFolder(string path) {
            if (Directory.Exists(path)) {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo file in dir.GetFiles()) {
                    file.Delete();
                }
                foreach (DirectoryInfo folder in dir.GetDirectories()) {
                    ClearFolder(folder.FullName);
                    folder.Delete();
                }
            }
        }
    }
}
