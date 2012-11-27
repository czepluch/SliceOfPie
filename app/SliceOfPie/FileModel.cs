using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public abstract class FileModel {
        public abstract void SaveDocument(Document doc);
        public abstract Document LoadDocument(int docId);
    }
}
