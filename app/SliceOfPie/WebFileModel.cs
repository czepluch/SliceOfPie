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

        public override void SaveDocument(Document document) {
            
        }

        public override Document AddDocument(IItemContainer parent, string title, bool db = false) {
            throw new NotImplementedException();
        }

        public override void RemoveDocument(Document d) {
            throw new NotImplementedException();
        }

        public override Folder AddFolder(IItemContainer parent, string title, bool db = false) {
            throw new NotImplementedException();
        }

        public override void RemoveFolder(Folder f) {
            throw new NotImplementedException();
        }

        public override Project AddProject(string title, bool db = false) {
            throw new NotImplementedException();
        }

        public override void RemoveProject(Project p) {
            throw new NotImplementedException();
        }
    }
}
