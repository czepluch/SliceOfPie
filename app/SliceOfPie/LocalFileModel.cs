using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SliceOfPie {
    public class LocalFileModel : IFileModel {
        private string AppPath;
        private string DefaultProjectPath;
        private List<Project> Projects = new List<Project>();

        public LocalFileModel() {
            CreateStructure();
            FindProjects();
        }

        public Project AddProject(string title) {
            string projectPath = Path.Combine(AppPath, title);
            if (Directory.Exists(projectPath)) {
                throw new ArgumentException("Project name is already in use (" + projectPath + ")");
            }
            Directory.CreateDirectory(projectPath);
            Project project = new Project();
            project.Title = title;
            project.AppPath = AppPath;
            Projects.Add(project);
            return project;
        }

        public void RenameProject(Project project, string title) {
            string projectPath = Path.Combine(AppPath, title);
            if (Directory.Exists(projectPath)) {
                throw new ArgumentException("Project name is already in use (" + projectPath + ")");
            }
            Directory.Move(Path.Combine(AppPath, project.Title), projectPath);
            project.Title = title;
        }

        public override IEnumerable<Project> GetProjects(int userId) {
            foreach (Project project in Projects) {
                yield return project;
            }
        }

        public Folder AddFolder(IItemContainer parent, string title) {
            string folderPath = Path.Combine(parent.GetPath(), title);
            if (Directory.Exists(folderPath)) {
                throw new ArgumentException("Project/folder name is already in use (" + folderPath + ")");
            }
            Directory.CreateDirectory(folderPath);
            Folder folder = new Folder();
            folder.Title = title;
            folder.Parent = parent;
            parent.Folders.Add(folder);
            return folder;
        }

        public void RenameFolder(Folder folder, string title) {
            string folderPath = Path.Combine(folder.Parent.GetPath(), title);
            if (Directory.Exists(folderPath)) {
                throw new ArgumentException("Project/folder name is already in use (" + folderPath + ")");
            }
            Directory.Move(folder.GetPath(), folderPath);
            folder.Title = title;
        }

        public Document AddDocument(IItemContainer parent, string title) {
            string documentPath = Path.Combine(parent.GetPath(), title);
            if (File.Exists(documentPath)) {
                throw new ArgumentException("File name is already in use (" + documentPath + ")");
            }
            FileStream fileStream = File.Create(documentPath);
            fileStream.Close();
            Document document = new Document();
            document.Title = title;
            document.Parent = parent;
            parent.Documents.Add(document);
            return document;
        }

        public void RenameDocument(Document document, string title) {
            if (File.Exists(document.GetPath())) {
                throw new ArgumentException("File name is already in use(" + document.GetPath() + ")");
            }
            File.Move(Path.Combine(document.Parent.GetPath(), document.Title), document.GetPath());
            document.Title = title;
        }

        public override void SaveDocument(Document document) {
            if (!File.Exists(document.GetPath())) {
                throw new ArgumentException("File does not exist (" + document.GetPath() + ")");
            }
            FileStream fileStream = new FileStream(document.GetPath(), FileMode.Open, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(document.CurrentRevision);
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        public void CreateStructure() {
            AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SliceOfPie");
            if (!Directory.Exists(AppPath)) {
                Directory.CreateDirectory(AppPath);
            }

            DefaultProjectPath = Path.Combine(AppPath, "default");
            if (!Directory.Exists(DefaultProjectPath)) {
                Directory.CreateDirectory(DefaultProjectPath);
            }
        }

        public void FindProjects() {
            Projects = new List<Project>();

            string[] folders = Directory.GetDirectories(AppPath);
            foreach (string folderName in folders) {
                Project project = new Project();
                project.Title = Path.GetFileName(folderName);
                project.AppPath = AppPath;
                Projects.Add(project);

                FindFolders(project);
                FindDocuments(project);
            }
        }

        public void FindFolders(IItemContainer parent) {
            string[] folders = Directory.GetDirectories(parent.GetPath());
            foreach (string folderName in folders) {
                Folder folder = new Folder();
                folder.Title = Path.GetFileName(folderName);
                folder.Parent = parent;
                parent.Folders.Add(folder);

                FindFolders(folder);
                FindDocuments(folder);
            }
        }

        public void FindDocuments(IItemContainer parent) {
            string[] documentPaths = Directory.GetFiles(parent.GetPath());
            foreach (string documentName in documentPaths) {
                Document document = new Document();
                document.Title = Path.GetFileName(documentName);
                document.Parent = parent;
                parent.Documents.Add(document);
            }
        }
    }
}
