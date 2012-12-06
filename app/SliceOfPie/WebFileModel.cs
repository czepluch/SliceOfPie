using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class WebFileModel : IFileModel {
        public override IEnumerable<Project> GetProjects(string userMail) {
            List<Project> projects = new List<Project>();

            foreach (Project project in projects) {
                yield return project;
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
