using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public abstract class IFileModel {
        public abstract IEnumerable<Project> GetProjects(string userMail);
        public abstract void SaveDocument(Document document);
    }
}