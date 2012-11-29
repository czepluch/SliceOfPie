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
            IEnumerable<Project> projects = Model.GetProjects(0);
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

            IEnumerable<Project> projects = Model.GetProjects(0);
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
        public void TestFindProjects() {
            string projectPath = Path.Combine(AppPath, "TestProject");
            Directory.CreateDirectory(projectPath);

            Model.FindProjects();

            IEnumerable<Project> projects = Model.GetProjects(0);
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
        public void TestFindFolders() {
            string folderPath = Path.Combine(Path.Combine(AppPath, "default"), "TestFolder");
            Directory.CreateDirectory(folderPath);

            Model.FindProjects();

            IEnumerable<Project> projects = Model.GetProjects(0);
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();
            Assert.AreEqual(Path.Combine(AppPath, "default"), Path.Combine(project.AppPath, project.Title));

            Assert.AreEqual(1, project.Folders.Count());
            Folder folder = project.Folders.First();
            Assert.AreEqual("TestFolder", folder.Title);
        }

        [TestMethod]
        public void TestFindDocuments() {
            string folderPath = Path.Combine(Path.Combine(AppPath, "default"), "TestFolder");
            Directory.CreateDirectory(folderPath);
            string documentPath = Path.Combine(Path.Combine(Path.Combine(AppPath, "default"), "TestFolder"), "TestFile");
            File.Create(documentPath);
            Assert.AreEqual(true, File.Exists(documentPath));

            Model.FindProjects();

            IEnumerable<Project> projects = Model.GetProjects(0);
            Assert.AreEqual(1, projects.Count());
            Project project = projects.First();
            Assert.AreEqual(Path.Combine(AppPath, "default"), Path.Combine(project.AppPath, project.Title));

            Assert.AreEqual(1, project.Folders.Count());
            Folder folder = project.Folders.First();
            Assert.AreEqual("TestFolder", folder.Title);

            Assert.AreEqual(1, folder.Documents.Count());
            Document document = folder.Documents.First();
            Assert.AreEqual("TestFile", document.Title);
        }

        [TestInitialize]
        public void Initialize() {
            ClearFolder(AppPath);
            Model = new LocalFileModel();
        }

        [TestCleanup]
        public void Cleanup() {
            //ClearFolder(AppPath);
        }

        private void ClearFolder(string path) {
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
