using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public abstract class IFileModel {
        public abstract IEnumerable<Project> GetProjects(string userMail);
        public abstract Project AddProject(string title, bool db = false);
        public abstract void RemoveProject(Project p);
        public abstract Folder AddFolder(IItemContainer parent, string title, bool db = false);
        public abstract void RemoveFolder(Folder f);
        public abstract Document AddDocument(IItemContainer parent, string title, bool db = false);
        public abstract void SaveDocument(Document d);
        public abstract void RemoveDocument(Document d);
    }
}