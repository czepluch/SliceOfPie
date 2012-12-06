using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace SliceOfPie {
    public interface IItemContainer {
        int Id { get; set; }
        string Title { get; set; }
        string AppPath { get; set; }
        IItemContainer Parent { get; set; }
        EntityCollection<Folder> Folders { get; set; }
        EntityCollection<Document> Documents { get; set; }

        IEnumerable<Folder> GetFolders();
        IEnumerable<Document> GetDocuments();
        string GetPath();
    }
}
