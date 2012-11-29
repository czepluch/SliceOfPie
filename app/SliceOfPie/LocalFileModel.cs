using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie {
    public class LocalFileModel : FileModel {
        private string AppPath;
        private string DefaultProjectPath;

        public LocalFileModel() {
            AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SliceOfPie");
            if (!Directory.Exists(AppPath)) {
                Directory.CreateDirectory(AppPath);
            }

            DefaultProjectPath = Path.Combine(AppPath, "default");
            if (!Directory.Exists(DefaultProjectPath)) {
                Directory.CreateDirectory(DefaultProjectPath);
            }

            foreach (Project p in GetProjects(0)) {
                Console.WriteLine(p.Title);
            }
        }

        public override IEnumerable<Project> GetProjects(int userId) {
            List<Project> projects = new List<Project>();
            string[] folders = Directory.GetDirectories(AppPath);
            foreach (string folderName in folders) {
                Project project = new Project();
                project.Title = Path.GetFileName(folderName);
                project.AppPath = AppPath;
                project.ThisPath = Path.Combine(AppPath, folderName);
                projects.Add(project);
            }

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
