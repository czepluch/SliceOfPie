using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie {
    public partial class Document:Item, ListableItem {
        public string FolderPath {
            get;
            set;
        }
        public string ThisPath {
            get;
            set;
        }

        public void Rename(string name) {
            string newPath = Path.Combine(FolderPath, name);
            if (File.Exists(newPath)) {
                throw new ArgumentException("Document name is already in use");
            }
            File.Move(ThisPath, newPath);
            ThisPath = newPath;
            Title = name;
        }
    }
}
