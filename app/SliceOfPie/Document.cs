using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie {
    public partial class Document : IItem, IListableItem {

        public IItemContainer Parent { get; set; }
        public bool IsMerged { get; set; }

        public IEnumerable<string> GetRevisions()
        {
            yield return CurrentRevision;
        }

        public string GetPath() {
            return Path.Combine(Parent.GetPath(), Helper.GenerateName(Id, Title) + ".txt");
        }
    }
}
