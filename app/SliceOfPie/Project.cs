using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie {
    public partial class Project:ItemContainer, ListableItem {
        public string ProjectPath {
            get;
            set;
        }

        public Project() {
            string[] folderPaths = Directory.GetDirectories(ProjectPath);
            foreach (string folderName in folderPaths) {
                Folder folder = new Folder();
                folder.Title = folderName;
                folder.FolderPath = Path.Combine(ProjectPath, folderName);
                Folders.Add(folder);
            }

            string[] docmentPaths = Directory.GetFiles(ProjectPath);
            foreach (string documentName in docmentPaths) {
                Document document = new Document();
                document.Title = documentName;
                document.DocumentPath = Path.Combine(ProjectPath, documentName);
                Documents.Add(document);
            }
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

        public void CreateDocument(string name) {

        }
    }
}
