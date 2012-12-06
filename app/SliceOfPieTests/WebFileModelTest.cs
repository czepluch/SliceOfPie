using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SliceOfPie.Tests {
    [TestClass]
    public class WebFileModelTest {
        WebFileModel model;

        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        [TestInitialize]
        public void Initialize() {
            model = new WebFileModel();
        }

        /// <summary>
        /// Tests that projects are properly returned from the model and contain all the sub-layers.
        /// </summary>
        [TestMethod]
        public void TestGetProjects() {
            //Assumes that me@michaelstorgaard.com has at least one project with both folders and documents
            IEnumerable<Project> projects = model.GetProjects("me@michaelstorgaard.com");

            if (projects.Count() < 1) throw new AssertFailedException("No projects returned from model");

            Project p = projects.First(); //get first project

            if (p.Folders.Count() < 1) throw new AssertFailedException("No folders were contained in the project");

            Folder f = p.Folders.First();

            if (f.Documents.Count() < 1) throw new AssertFailedException("No documents were contained in the folder");

            Document d = f.Documents.First();

            if (d.Revisions.Count() < 1) throw new AssertFailedException("No revisions were contained in the document");
        }
    }
}
