using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SliceOfPie;

namespace SliceOfPie.Tests {
    /// <summary>
    /// tests that the APM implementation of the controller works. The primary purpose of these tests
    /// is to assert that they work asynchronously.
    /// </summary>
    [TestClass]
    public class ControllerAPMTest {
        private Controller controller = Controller.Instance;
        private string AppPath;

        public ControllerAPMTest() {
            AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SliceOfPie");
        }

        /// <summary>
        /// Tests that projects may be created asynchronously
        /// </summary>
        [TestMethod]
        public void TestProjectCreate() {
            string projectName = "New Project";
            IAsyncResult ar = controller.BeginCreateProject(projectName, "common@test.mail", null, null);

            Project p = controller.EndCreateProject(ar);

            Assert.AreEqual(projectName, p.Title);
        }

        /// <summary>
        /// Test removal of project
        /// </summary>
        [TestMethod]
        public void TestProjectRemove() {
            string projectName = "AProject";
            IAsyncResult createAr = controller.BeginCreateProject(projectName, "common@test.mail", null, null);
            Project p = controller.EndCreateProject(createAr);

            IAsyncResult getAllAr = controller.BeginGetProjects("common@test.mail", null, null);
            Assert.IsTrue(controller.EndGetProjects(getAllAr).Contains(p));

            IAsyncResult removeAr = controller.BeginRemoveProject(p, null, null);
            controller.EndRemoveProject(removeAr);

            IAsyncResult getAllAgainAr = controller.BeginGetProjects("common@test.mail", null, null);
            Assert.IsFalse(controller.EndGetProjects(getAllAgainAr).Contains(p));
        }

        /// <summary>
        /// Test that projects are shared correctly.
        /// </summary>
        [TestMethod]
        public void TestProjectShare() {
            LocalFileModel model = new LocalFileModel();

            IEnumerable<Project> projects = model.GetProjects("local");
            Project project = projects.First();

            model.UploadStructure("common@test.mail");
            model.FindProjects();
            projects = model.GetProjects("local");
            project = projects.First();

            Assert.AreEqual(1, projects.Count());

            string[] emails = {"me@hypesystem.dk"};
            IAsyncResult shareAr = controller.BeginShareProject(project, emails, null, null);
            controller.EndShareProject(shareAr);

            using (var dbContext = new sliceofpieEntities2()) {
                var projectUsers = from projectUser in dbContext.ProjectUsers
                                   where projectUser.ProjectId == project.Id && projectUser.UserEmail == "me@hypesystem.dk"
                                   select projectUser;
                Assert.AreEqual(1, projectUsers.Count());
            }
        }

        /// <summary>
        /// Tests that documents may be created asynchronously.
        /// </summary>
        [TestMethod]
        public void TestDocumentCreate() {
            string documentTitle = "Hello World";
            Project p = controller.CreateProject("New Pruhjekt", "common@test.mail");
            IAsyncResult ar = controller.BeginCreateDocument(documentTitle, "common@test.mail", p, null, null);
            Document d = controller.EndCreateDocument(ar);

            Assert.AreEqual(documentTitle, d.Title);
        }

        /// <summary>
        /// Tests that documents can be saved asynchronously
        /// </summary>
        [TestMethod]
        public void TestDocumentSave() {
            Project p = controller.CreateProject("TestProj", "common@test.mail");
            Document d = controller.CreateDocument("NewDoc", "common@test.mail", p);

            d.CurrentRevision = "New Text Here.";

            IAsyncResult ar2 = controller.BeginSaveDocument(d, null, null);
            controller.EndSaveDocument(ar2);

            //Document freshFetch = controller.GetProjects("me@hypesystem.dk")
            //                            .First(proj => proj.Id == p.Id).GetDocuments()
            //                            .First(doc => doc.Id == d.Id);

            //Assert.AreEqual("New Text Here.".Trim(), freshFetch.CurrentRevision.Trim());
        }

        /// <summary>
        /// Tests that documents may be removed asynchronously
        /// </summary>
        [TestMethod]
        public void TestDocumentRemove() {
            Project p = controller.CreateProject("TestProj", "common@test.mail");
            Document d = controller.CreateDocument("NewDoc22", "common@test.mail", p);

            IAsyncResult ar2 = controller.BeginRemoveDocument(d, null, null);
            controller.EndRemoveDocument(ar2);
        }

        /// <summary>
        /// Tests that folders can be created asynchronously
        /// </summary>
        [TestMethod]
        public void TestFolderCreate() {
            Project p = controller.CreateProject("TestProjzxx", "common@test.mail");
            IAsyncResult ar = controller.BeginCreateFolder("FolderCoolSauce", "common@test.mail", p, null, null);
            Folder f = controller.EndCreateFolder(ar);

            Assert.AreEqual("FolderCoolSauce", f.Title);
        }

        /// <summary>
        /// Tests that folders can be removed asynchronously
        /// </summary>
        [TestMethod]
        public void TestFolderRemove() {
            Project p = controller.CreateProject("TestProjzxx", "common@test.mail");
            Folder f = controller.CreateFolder("FolderLolz", "common@test.mail", p);

            IAsyncResult ar = controller.BeginRemoveFolder(f, null, null);
            controller.EndRemoveFolder(ar);
        }

        /// <summary>
        /// Tests that projects may be correctly retrieved from the model.
        /// </summary>
        [TestMethod]
        public void TestGetProjects() {
            IAsyncResult argh = controller.BeginGetProjects("common@test.mail", (ar) => {
                Project[] projects = controller.EndGetProjects(ar).ToArray();

                Assert.IsTrue(projects.Count() > 0);

                foreach (Project p in projects) {
                    Assert.IsTrue(!p.Title.Equals(string.Empty));
                }
            }, null);
            controller.EndGetProjects(argh);
        }

        /// <summary>
        /// Tests that synchronization of projects works asynchronously
        /// </summary>
        [TestMethod]
        public void TestSyncProjects() {
            LocalFileModel model = new LocalFileModel();

            IAsyncResult ar = controller.BeginSyncProjects("common@test.mail", "pw", null, null);
            IEnumerable<Project> projectsSynced = controller.EndSyncProjects(ar);

            Assert.IsTrue(projectsSynced.Count() > 0);
        }

        [TestInitialize]
        public void Initialize() {
            TestHelper.ClearDatabase("common@test.mail");
            TestHelper.ClearFolder(AppPath);
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            TestHelper.ClearDatabase("common@test.mail");
            TestHelper.ClearFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SliceOfPie"));
        }
    }
}
