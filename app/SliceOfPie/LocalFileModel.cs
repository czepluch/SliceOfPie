using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class LocalFileModel : FileModel {
        public static List<Project> GetProjects(int userId) {
            List<Project> projects = new List<Project>();

            return projects;
        }

        public static IEnumerable<Item> ListProject(Project project) {
            List<Item> items = new List<Item>();

            items.Add(new Folder());
            items.Add(new Document());

            return items;
        }

        public static IEnumerable<Item> ListFolder(Folder folder) {
            List<Item> items = new List<Item>();

            items.Add(new Folder());
            items.Add(new Document());

            return items;
        }

        public static void SaveDocument(Document doc) {
            throw new NotImplementedException();
        }

        public static Document LoadDocument(int docId) {
            throw new NotImplementedException();
        }
    }
}
