using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public partial class Document:Item, ListableItem {
        public string DocumentPath {
            get;
            set;
        }
    }
}
