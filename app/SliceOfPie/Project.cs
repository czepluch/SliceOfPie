using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie {
    public partial class Project : IItemContainer, ListableItem {
        public string AppPath { get; set; }
        public IItemContainer Parent { get; set; } // Not used

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
            return Path.Combine(AppPath, Title);
        }
    }
}
