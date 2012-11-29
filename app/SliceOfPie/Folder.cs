using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie {
    public partial class Folder : Item, ItemContainer, ListableItem {
        public string ProjectPath {
            get;
            set;
        }
        public string ThisPath {
            get;
            set;
        }

        public Folder() {
            string[] folderPaths = Directory.GetDirectories(ThisPath);
            foreach (string folderName in folderPaths) {
                Folder folder = new Folder();
                folder.Title = folderName;
                folder.ThisPath = Path.Combine(ThisPath, folderName);
                Folders.Add(folder);
            }

            string[] docmentPaths = Directory.GetFiles(ThisPath);
            foreach (string documentName in docmentPaths) {
                Document document = new Document();
                document.Title = documentName;
                document.DocumentPath = Path.Combine(ThisPath, documentName);
                Documents.Add(document);
            }
        }

        public void Rename(string name) {
            string newPath = Path.Combine(ProjectPath, name);
            if (Directory.Exists(newPath)) {
                throw new ArgumentException("Project/folder name is already in use");
            }
            Directory.Move(ThisPath, newPath);
            ProjectPath = newPath;
            Title = name;
        }

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

        public void CreateFolder(string name) {
            string folderPath = Path.Combine(ThisPath, name);
            if (Directory.Exists(folderPath)) {
                throw new ArgumentException("Folder name is already in use");
            }
            Directory.CreateDirectory(folderPath);
            Folder folder = new Folder();
            folder.Title = name;
            Folders.Add(folder);
        }

        public void CreateDocument(string name) {
            string documentPath = Path.Combine(ThisPath, name);
            if (File.Exists(documentPath)) {
                throw new ArgumentException("Document name is already in use");
            }
            File.Create(documentPath);
            Document document = new Document();
            document.Title = name;
            Documents.Add(document);
        }
    }
}
