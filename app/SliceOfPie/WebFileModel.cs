using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class WebFileModel : IFileModel {
        public override IEnumerable<Project> GetProjects(int userId) {
            List<Project> projects = new List<Project>();

            foreach (Project project in projects) {
                yield return project;
            }
        }

        public override void SaveDocument(Document doc) {
            
        }

        public override Document LoadDocument(int docId) {
            return new Document();
        }
    }
}
