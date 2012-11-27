using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class LocalFileModel : FileModel {
        public static IEnumerable<Project> GetProjects(int userId) {
            List<Project> projects = new List<Project>();

            foreach (Project project in projects) {
                yield return project;
            }
        }

        public static void SaveDocument(Document doc) {
            throw new NotImplementedException();
        }

        public static Document LoadDocument(int docId) {
            throw new NotImplementedException();
        }

        public static void Main(string[] args) {
            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }
    }
}
