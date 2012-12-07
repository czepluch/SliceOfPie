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

        /// <summary>
        /// Tests that projects are removed correctly from the database.
        /// </summary>
        [TestMethod]
        public void TestRemoveProject() {
            Project p = model.AddProject("Shazam, hullu.", "me@hypesystem.dk");

            Assert.IsTrue(model.GetProjects("me@hypesystem.dk").Count(project => project.Id == p.Id) > 0);

            model.RemoveProject(p);

            Assert.IsFalse(model.GetProjects("me@hypesystem.dk").Count(project => project.Id == p.Id) > 0);
        }

        /// <summary>
        /// Tests that folders are added correctly with reference to their parent.
        /// </summary>
        [TestMethod]
        public void TestAddFolder() {
            Project p = model.AddProject("Hello World", "me@hypesystem.dk");
            Folder f = model.AddFolder(p, "Test Folder");

            Assert.AreEqual("Test Folder", f.Title);
            Assert.AreEqual(p.Id, f.Parent.Id);
            Assert.AreNotEqual(0, f.Id);
        }

        /// <summary>
        /// Tests that folders are actually removed
        /// </summary>
        [TestMethod]
        public void TestRemoveFolder() {
            Project p = model.AddProject("Shazam, hullu.", "me@hypesystem.dk");
            Folder f = model.AddFolder(p, "New Test Folder");

            Assert.IsTrue(model.GetProjects("me@hypesystem.dk") //assert that the project now contains this folder
                .First(project => project.Id == p.Id)
                .GetFolders().Count(folder => folder.Id == f.Id) > 0);

            model.RemoveFolder(f);

            Assert.IsFalse(model.GetProjects("me@hypesystem.dk") //assert that the above is no longer true
                .First(project => project.Id == p.Id)
                .GetFolders().Count(folder => folder.Id == f.Id) > 0);
        }

        /// <summary>
        /// Tests that documents returned by AddDocument are proper.
        /// </summary>
        [TestMethod]
        public void TestAddDocument() {
            Project testProject = model.AddProject("Common Test Project", "common@test.mail");
            Document testDoc = model.AddDocument(testProject, "Test Document");

            Assert.AreEqual("Test Document", testDoc.Title);
            Assert.AreNotEqual(0, testDoc.Id);
            Assert.AreEqual(testProject.Id, testDoc.Parent.Id);
        }

        /// <summary>
        /// Tests that documents are saved correctly
        /// </summary>
        [TestMethod]
        public void TestSaveDocument() {
            Project testProject = model.AddProject("Common Test Project", "common@test.mail");
            Document testDoc = model.AddDocument(testProject, "Test Document");

            testDoc.CurrentRevision = "Hello, party people";
            model.SaveDocument(testDoc);

            Assert.AreEqual("Hello, party people",
                model.GetProjects("common@test.mail")
                    .First(p => p.Id == testProject.Id)
                    .GetDocuments().First(doc => doc.Id == testDoc.Id).CurrentRevision);
        }

        /// <summary>
        /// Tests that documents are removed properly
        /// </summary>
        [TestMethod]
        public void TestRemoveDocument() {
            Project p = model.AddProject("Shazam, hullu.", "me@hypesystem.dk");
            Document d = model.AddDocument(p, "New Test Doc");

            Assert.IsTrue(model.GetProjects("me@hypesystem.dk") //assert that the project now contains this document
                .First(project => project.Id == p.Id)
                .GetDocuments().Count(doc => doc.Id == d.Id) > 0);

            model.RemoveDocument(d);

            Assert.IsFalse(model.GetProjects("me@hypesystem.dk") //assert that the above is no longer true
                .First(project => project.Id == p.Id)
                .GetDocuments().Count(doc => doc.Id == d.Id) > 0);
        }
    }
}
