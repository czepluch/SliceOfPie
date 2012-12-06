using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SliceOfPie;

namespace SliceOfPie.Tests {
    [TestClass]
    public class ControllerAPMTest {
        private Controller controller = Controller.Instance;

        /// <summary>
        /// Tests that projects may be created asynchronously
        /// </summary>
        [TestMethod]
        public void TestProjectCreate() {
            string projectName = "New Project";
            IAsyncResult ar = controller.BeginCreateProject(projectName, "user@mail.com", null, null);

            Project p = controller.EndCreateProject(ar);

            Assert.AreEqual(projectName, p.Title);
        }

        /// <summary>
        /// Test removal of project
        /// </summary>
        [TestMethod]
        public void TestProjectRemove() {
            string projectName = "AProject";
            IAsyncResult createAr = controller.BeginCreateProject(projectName, "user@mail.com", null, null);
            Project p = controller.EndCreateProject(createAr);

            IAsyncResult getAllAr = controller.BeginGetProjects("user@mail.com", null, null);
            Assert.IsTrue(controller.EndGetProjects(getAllAr).Contains(p));

            IAsyncResult removeAr = controller.BeginRemoveProject(p, null, null);
            controller.EndRemoveProject(removeAr);

            IAsyncResult getAllAgainAr = controller.BeginGetProjects("user@mail.com", null, null);
            Assert.IsFalse(controller.EndGetProjects(getAllAgainAr).Contains(p));
        }

        [TestMethod]
        public void TestProjectShare() {
            throw new NotImplementedException("Shoop da woop");
        }

        [TestMethod]
        public void TestDocumentCreate() {
            string documentTitle = "Hello World";
            Project p = controller.CreateProject("New Pruhjekt", "user@mail.com");
            IAsyncResult ar = controller.BeginCreateDocument(documentTitle, "user@mail.com", p, null, null);
            Document d = controller.EndCreateDocument(ar);

            Assert.AreEqual(documentTitle, d.Title);
        }

        [TestMethod]
        public void TestDocumentSave() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestDocumentRemove() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestFolderCreate() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestFolderRemove() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestGetProjects() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestSyncProjects() {
            throw new NotImplementedException();
        }
        


    }
}
