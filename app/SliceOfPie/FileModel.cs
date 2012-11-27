using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public abstract class FileModel {
        public abstract IEnumerable<Project> GetProjects(int userId);
        public abstract IEnumerable<Item> ListProject(Project project);
        public abstract IEnumerable<Item> ListFolder(Folder folder);
        public abstract void SaveDocument(Document doc);
        public abstract Document LoadDocument(int docId);
    }
}
