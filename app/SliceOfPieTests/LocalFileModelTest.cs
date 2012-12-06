using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SliceOfPie;

namespace SliceOfPieTests {
    [TestClass]
    public class LocalFileModelTest {
        string AppPath;
        LocalFileModel Model;

        public LocalFileModelTest() {
            AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SliceOfPie");
        }

        [TestMethod]
        public void TestConstruct() {
            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());

            int i = 0;
            foreach (Project project in projects) {
                switch (i) {
                    case 0:
                        Assert.AreEqual(Path.Combine(AppPath, "default"), Path.Combine(project.AppPath, project.Title));
                        break;
                }
                i++;
            }
        }

        [TestMethod]
        public void TestAddProject() {
            Model.AddProject("TestProject");

            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(2, projects.Count());

            int i = 0;
            foreach (Project project in projects) {
                switch (i) {
                    case 0:
                        Assert.AreEqual(Path.Combine(AppPath, "default"), Path.Combine(project.AppPath, project.Title));
                        break;
                    case 1:
                        Assert.AreEqual(Path.Combine(AppPath, "TestProject"), Path.Combine(project.AppPath, project.Title));
                        break;
                }
                i++;
            }
        }

        [TestMethod]
        public void TestRenameProject() {
            Project testProject = Model.AddProject("TestProject");
            Model.RenameProject(testProject, "RenamedProject");

            Assert.AreEqual(testProject.Title, "RenamedProject");
        }

        [TestMethod]
        public void TestRemoveProject() {
            Project project = Model.AddProject("TestProject");

            Folder folder = Model.AddFolder(project, "TestFolder");
            Document document = Model.AddDocument(folder, "TestDocument");

            Assert.AreEqual(1, project.Folders.Count());
            Assert.AreEqual(1, folder.Documents.Count());
            String path = project.GetPath();
            String folderPath = folder.GetPath();
            String docPath = document.GetPath();

            Model.RemoveProject(project);

            Assert.AreEqual(1, Model.GetProjects("local").Count());
            Assert.AreEqual(false, Directory.Exists(path));
            Assert.AreEqual(false, Directory.Exists(folderPath));
            Assert.AreEqual(false, File.Exists(docPath));
        }

        [TestMethod]
        public void TestAddFolder() {
            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();

            Model.AddFolder(project, "TestFolder");

            Assert.AreEqual(1, project.Folders.Count());
            Folder folder = project.Folders.First();
            Assert.AreEqual("TestFolder", folder.Title);
        }

        [TestMethod]
        public void TestRenameFolder() {
            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();

            Model.AddFolder(project, "TestFolder");

            Assert.AreEqual(1, project.Folders.Count());
            Folder folder = project.Folders.First();
            Assert.AreEqual("TestFolder", folder.Title);

            Model.RenameFolder(folder, "RenamedFolder");
            Assert.AreEqual(folder.Title, "RenamedFolder");
        }

        [TestMethod]
        public void TestRemoveFolder() {
            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();

            Folder folder = Model.AddFolder(project, "TestFolder");
            Document document = Model.AddDocument(folder, "TestDocument");

            Assert.AreEqual(1, project.Folders.Count());
            String path = folder.GetPath();
            String docPath = document.GetPath();

            Model.RemoveFolder(folder);

            Assert.AreEqual(0, project.Folders.Count());
            Assert.AreEqual(false, Directory.Exists(path));
            Assert.AreEqual(false, File.Exists(docPath));
        }

        [TestMethod]
        public void TestAddDocument() {
            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();

            Model.AddDocument(project, "TestDocument");

            Assert.AreEqual(1, project.Documents.Count());
            Document document = project.Documents.First();
            Assert.AreEqual("TestDocument", document.Title);
        }

        [TestMethod]
        public void TestAddExistingDocument() {
            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();

            Document document = Model.AddDocument(project, "TestDocument");
            Document document1 = Model.AddDocument(project, "TestDocument");
            Document document2 = Model.AddDocument(project, "TestDocument");
            Assert.AreEqual(3, project.Documents.Count());

            Assert.AreEqual("TestDocument", document.Title);
            Assert.AreEqual("TestDocument-1", document1.Title);
            Assert.AreEqual("TestDocument-2", document2.Title);
        }

        [TestMethod]
        public void TestRenameDocument() {
            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();

            Model.AddDocument(project, "TestDocument");

            Assert.AreEqual(1, project.Documents.Count());
            Document document = project.Documents.First();
            Assert.AreEqual("TestDocument", document.Title);

            Model.RenameDocument(document, "RenamedDocument");
            Assert.AreEqual(document.Title, "RenamedDocument");
        }

        [TestMethod]
        public void TestSaveDocument() {
            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();

            Document document = Model.AddDocument(project, "TestDocument");
            document.CurrentRevision = "This is a test!";
            Model.SaveDocument(document);

            FileStream fileStream = new FileStream(document.GetPath(), FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream);
            string contents = "";
            while (streamReader.Peek() >= 0) {
                contents += streamReader.ReadLine() + "\n";
            }
            streamReader.Close();
            fileStream.Close();

            Assert.AreEqual("This is a test!\n", contents);
        }

        [TestMethod]
        public void TestRemoveDocument() {
            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();

            Document document = Model.AddDocument(project, "TestDocument");

            Assert.AreEqual(1, project.Documents.Count());
            String path = document.GetPath();

            Model.RemoveDocument(document);

            Assert.AreEqual(0, project.Documents.Count());
            Assert.AreEqual(false, File.Exists(path));
        }

        [TestMethod]
        public void TestFindProjects() {
            string projectPath = Path.Combine(AppPath, "0-TestProject");
            Directory.CreateDirectory(projectPath);

            Model.FindProjects();

            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(2, projects.Count());

            int i = 0;
            foreach (Project project in projects) {
                switch (i) {
                    case 0:
                        Assert.AreEqual(Path.Combine(AppPath, "0-default"), project.GetPath());
                        break;
                    case 1:
                        Assert.AreEqual(Path.Combine(AppPath, "0-TestProject"), project.GetPath());
                        break;
                }
                i++;
            }
        }

        [TestMethod]
        public void TestFindFolders() {
            string folderPath = Path.Combine(Path.Combine(AppPath, "0-default"), "0-TestFolder");
            Directory.CreateDirectory(folderPath);

            Model.FindProjects();

            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();
            Assert.AreEqual(Path.Combine(AppPath, "0-default"), project.GetPath());

            Assert.AreEqual(1, project.Folders.Count());
            Folder folder = project.Folders.First();
            Assert.AreEqual(Path.Combine(project.GetPath(), "0-TestFolder"), folder.GetPath());
        }

        [TestMethod]
        public void TestFindDocuments() {
            string folderPath = Path.Combine(Path.Combine(AppPath, "0-default"), "0-TestFolder");
            Directory.CreateDirectory(folderPath);
            string documentPath = Path.Combine(Path.Combine(Path.Combine(AppPath, "0-default"), "0-TestFolder"), "0-TestFile.txt");
            FileStream fileStream = File.Create(documentPath);
            fileStream.Close();
            Assert.AreEqual(true, File.Exists(documentPath));

            Model.FindProjects();

            IEnumerable<Project> projects = Model.GetProjects("local");
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();
            Assert.AreEqual(Path.Combine(AppPath, "0-default"), project.GetPath());

            Assert.AreEqual(1, project.Folders.Count());
            Folder folder = project.Folders.First();
            Assert.AreEqual("TestFolder", folder.Title);

            Assert.AreEqual(1, folder.Documents.Count());
            Document document = folder.Documents.First();
            Assert.AreEqual("TestFile", document.Title);
        }

        [TestMethod]
        public void TestGetAvailableName() {
            string documentPath = Path.Combine(Path.Combine(AppPath, "0-default"), "0-TestFile");
            FileStream fileStream = File.Create(documentPath);
            fileStream.Close();
            Assert.AreEqual(true, File.Exists(documentPath));
            Assert.AreEqual("TestFile-1", Model.GetAvailableName("TestFile", 0, Path.Combine(AppPath, "0-default")));
        }

        //[TestMethod]
        //public void TestUploadStructure() {
        //    Assert.AreEqual(true, Model.UploadStructure("me@michaelstorgaard.com"));
        //}

        [TestInitialize]
        public void Initialize() {
            ClearFolder(AppPath);
            Model = new LocalFileModel();
        }

        [TestCleanup]
        public void Cleanup() {
            ClearFolder(AppPath);
        }

        private void ClearFolder(string path) {
            if (Directory.Exists(path)) {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo file in dir.GetFiles()) {
                    file.Delete();
                }
                foreach (DirectoryInfo folder in dir.GetDirectories()) {
                    ClearFolder(folder.FullName);
                    folder.Delete();
                }
            }
        }
    }
}
