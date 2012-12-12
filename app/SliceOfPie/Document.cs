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
            foreach (Revision r in Revisions)
                yield return r.Content;
        }

        public string GetPath() {
            return Path.Combine(Parent.GetPath(), Helper.GenerateName(Id, Title) + ".txt");
        }
    }
}
