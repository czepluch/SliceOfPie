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

            Project p = projects.First(proj => proj.Id == 1); //get first project
            if (p.Id < 1) throw new AssertFailedException("Project has id below allowed value");
            if (p.Title.Equals(string.Empty)) throw new AssertFailedException("Project title has not been set");

            if (p.GetFolders().Count() < 1) throw new AssertFailedException("No folders were contained in the project, "+p.Title);

            Folder f = p.GetFolders().First(fold => fold.Id == 1);
            if (f.Id < 1) throw new AssertFailedException("Folder has id below allowed value");
            if (f.Title.Equals(string.Empty)) throw new AssertFailedException("Folder title has not been set");

            if (f.GetDocuments().Count() < 1) throw new AssertFailedException("No documents were contained in the folder, "+f.Title+" in "+p.Title);

            Document d = f.GetDocuments().First(doc => doc.Id == 1);
            if (d.Id < 1) throw new AssertFailedException("Document has id below allowed value");
            if (d.Title.Equals(string.Empty)) throw new AssertFailedException("Document title has not been set");
            if (d.CurrentRevision.Equals(string.Empty)) throw new AssertFailedException("Document CurrentRevision is empty!!!");
            if (d.CurrentHash == 0) throw new AssertFailedException("Document Hash has not been set");

            if (d.GetRevisions().Count() < 1) throw new AssertFailedException("No revisions were contained in the document, "+d.Title+" in "+f.Title);

            string s = d.GetRevisions().First();
        }

        /// <summary>
        /// Tests that projects are returned correctly when added.
        /// </summary>
        [TestMethod]
        public void TestAddProject() {
            Project p = model.AddProject("Hello Kitty", "me@hypesystem.dk");

            Assert.AreNotEqual(0, p.Id);
            Assert.AreEqual("Hello Kitty", p.Title);
        }
    }
}
