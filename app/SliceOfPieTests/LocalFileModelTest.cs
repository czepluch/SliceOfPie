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
