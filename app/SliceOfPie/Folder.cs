using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie {
    public partial class Folder : IItem, IItemContainer, IListableItem {
        public string AppPath { get; set; } // Not used
        public IItemContainer Parent { get; set; }

        public IEnumerable<Folder> GetFolders() {
            foreach (Folder folder in Folders) {
                yield return folder;
            }
        }

        public IEnumerable<Document> GetDocuments() {
            foreach (Document document in Documents) {
                yield return document;
            }
        }

        public string GetPath() {
            return Path.Combine(Parent.GetPath(), Helper.GenerateName(Id, Title));
        }
    }
}
