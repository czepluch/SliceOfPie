using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public interface ItemContainer {
        IEnumerable<Folder> GetFolders();
        IEnumerable<Document> GetDocuments();
    }
}
