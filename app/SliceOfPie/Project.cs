using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie {
    public partial class Project:ItemContainer {
        public string ProjectPath {
            get;
            set;
        }

        public IEnumerable<Item> ListItems() {
            List<Item> items = new List<Item>();

            items.Add(new Folder());
            items.Add(new Document());

            foreach (Item item in items) {
                yield return item;
            }
        }

        public void CreateFolder(string name) {
            string folderName = Path.Combine(ProjectPath, name);
            if (!Directory.Exists(folderName)) {
                Directory.CreateDirectory(folderName);
            }
        }
    }
}
