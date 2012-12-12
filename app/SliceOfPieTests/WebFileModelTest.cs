using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SliceOfPie.Tests {
    [TestClass]
    public class WebFileModelTest {
        static readonly String user = "common@test.mail";
        WebFileModel model = new WebFileModel();
        Project proj;
        Folder projFolder;
        Document projFolderDoc;

        [TestInitialize]
        public void TestInitialize() {
            proj = model.AddProject("WFMTestProj", user);
            projFolder = model.AddFolder(proj, "WFMTestFolder");
            projFolderDoc = model.AddDocument(projFolder, "WFMTestDocument");
            projFolderDoc.CurrentRevision = @"3 May. Bistritz. Left Munich at 8:35 p. M., on ist May, ar- 
riving at Vienna early next morning; should have arrived at 
6:46, but train was an hour late. Buda-Pesth seems a wonderful 
place, from the glimpse which I got of it from the train and the 
little I could walk through the streets.";
            model.SaveDocument(projFolderDoc);
        }

        [TestCleanup]
        public void TestCleanup() {
            if (proj != null) {
                model.RemoveProject(proj);
                proj = null;
            }
            projFolder = null;
            projFolderDoc = null;
        }

        /// <summary>
        /// Tests that projects are properly returned from the model and contain all the sub-layers.
        /// </summary>
        [TestMethod]
        public void TestGetProjects() {
            IEnumerable<Project> projects = model.GetProjects(user);

            if (projects.Count() < 1) throw new AssertFailedException("No projects returned from model");

            Project p = projects.First(projectToGet => projectToGet.Id == proj.Id); //get first project
            Project pi = model.GetProject(proj.id);

            if (p.Id < 1) throw new AssertFailedException("Project has id below allowed value");
            if (pi.Id < 1) throw new AssertFailedException("Project has id below allowed value");
            if (p.Title.Equals(string.Empty)) throw new AssertFailedException("Project title has not been set");
            if (pi.Title.Equals(string.Empty)) throw new AssertFailedException("Project title has not been set");

            if (p.GetFolders().Count() < 1) throw new AssertFailedException("No folders were contained in the project, " + p.Title);
            if (pi.GetFolders().Count() < 1) throw new AssertFailedException("No folders were contained in the project, " + p.Title);

            Folder f = p.GetFolders().First();
            Folder fi = p.GetFolders().First();
            if (f.Id < 1) throw new AssertFailedException("Folder has id below allowed value");
            if (fi.Id < 1) throw new AssertFailedException("Folder has id below allowed value");
            if (fi.Title.Equals(string.Empty)) throw new AssertFailedException("Folder title has not been set");

            if (f.GetDocuments().Count() < 1) throw new AssertFailedException("No documents were contained in the folder, " + f.Title + " in " + p.Title);
            if (fi.GetDocuments().Count() < 1) throw new AssertFailedException("No documents were contained in the folder, " + f.Title + " in " + p.Title);

            Document d = f.GetDocuments().First();
            Document di = f.GetDocuments().First();
            if (d.Id < 1) throw new AssertFailedException("Document has id below allowed value");
            if (di.Id < 1) throw new AssertFailedException("Document has id below allowed value");
            if (d.Title.Equals(string.Empty)) throw new AssertFailedException("Document title has not been set");
            if (di.Title.Equals(string.Empty)) throw new AssertFailedException("Document title has not been set");
            if (d.CurrentRevision.Equals(string.Empty)) throw new AssertFailedException("Document CurrentRevision is empty!!!");
            if (di.CurrentRevision.Equals(string.Empty)) throw new AssertFailedException("Document CurrentRevision is empty!!!");
            if (d.CurrentHash == 0) throw new AssertFailedException("Document Hash has not been set");
            if (di.CurrentHash == 0) throw new AssertFailedException("Document Hash has not been set");

            if (d.GetRevisions().Count() < 1) throw new AssertFailedException("No revisions were contained in the document, " + d.Title + " in " + f.Title);
            if (di.GetRevisions().Count() < 1) throw new AssertFailedException("No revisions were contained in the document, " + d.Title + " in " + f.Title);
        }

        /// <summary>
        /// Ensure that one does not simply get a project with a negative id
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetProjectNegId() {
            model.GetProject(-1);
        }

        /// <summary>
        /// Ensure that one does not simply get a project with id == zero
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetProjectZeroId() {
            model.GetProject(0);
        }
        

        /// <summary>
        /// Ensure that one does not simply get a project from a null project
        /// </summary>
        [TestMethod]
        public void TestGetProjectSNull() {
            String s = null;
            Assert.IsFalse(model.GetProjects(s).ToList().Count > 0);
        }

        /// <summary>
        /// Tests that projects are returned correctly when added.
        /// </summary>
        [TestMethod]
        public void TestAddProject() {
            String name = "WFMNewAddProject";
            Project p = model.AddProject(name, user);

            Assert.AreNotEqual(0, p.Id);
            Assert.AreEqual(name, p.Title);
            //cleanup
            model.RemoveProject(p);
        }

        /// <summary>
        /// Ensure that one does not simply add a project with null for a name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddProjectNullUser() {
            String name = null;
            model.AddProject("Valid Title", name);
        }

        /// <summary>
        /// Ensure that one does not simply add a project for a null user
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddProjectNullTitle() {
            String title = null;
            Assert.IsNull(model.AddProject(title, user));
        }

        /// <summary>
        /// Ensure that one does not simply add a project for a null user, with a null title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddProjectNullBoth() {
            String nullStr = null;
            Assert.IsNull(model.AddProject(nullStr, nullStr));
        }

        /// <summary>
        /// Tests that projects are removed correctly from the database.
        /// </summary>
        [TestMethod]
        public void TestRemoveProject() {
            Assert.IsTrue(model.GetProjects(user).Count(project => project.Id == proj.Id) == 1);
            model.RemoveProject(proj);
            Assert.IsFalse(model.GetProjects(user).Count(project => project.Id == proj.Id) > 0);
            proj = null; //disable cleanup
        }

        /// <summary>
        /// Tests that projects and sub components are removed correctly from the database.
        /// </summary>
        [TestMethod]

        public void TestRemoveProjectsRecursively() {

            Folder f = model.AddFolder(projFolder, "WFMTestNewFolderRemoeProjet");
            model.RemoveProject(proj);
            Assert.IsFalse(model.GetProjects(user).Count(project => project.Id == proj.Id) > 0);
            Assert.IsNull(model.GetDocument(projFolderDoc.Id));
            Assert.IsNull(model.GetFolder(projFolder.Id));
            Assert.IsNull(model.GetFolder(f.Id));
            proj = null; //to disable the cleanup
        }

        /// <summary>
        /// Tests that trying to remove a non-existing project results in an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRemoveProjectNonexistingProject() {
            Project p = new Project();
            model.RemoveProject(p);
        }

        /// <summary>
        /// Tests that trying to remove a null project results in an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveProjectNull() {
            Project p = null;
            model.RemoveProject(p);
        }

        /// <summary>
        /// Tests that folders are added correctly with reference to their parent.
        /// </summary>
        [TestMethod]
        public void TestAddFolder() {
            String name = "WFMNewAddedFolder";
            Folder f = model.AddFolder(proj, name);

            Assert.AreEqual(name, f.Title);
            Assert.AreEqual(proj.Id, f.Parent.Id);
            Assert.AreNotEqual(0, f.Id);
        }

        /// <summary>
        /// Tests that folders can have the same name, though they are in the same parent
        /// </summary>
        [TestMethod]
        public void TestAddFolderWithExistingName() {
            String name = "WFMNewAddedFolder";
            Folder f = model.AddFolder(proj, name);
            Folder f2 = model.AddFolder(proj, name);

            Assert.AreEqual(f.Title, f2.Title);
            Assert.AreEqual(f2.Parent.Id, f.Parent.Id);
            Assert.AreNotEqual(f2.Id, f.Id);
        }

        /// <summary>
        /// Tests that folders with null for a name cannot be added
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddFolderNull() {
            String nullStr = null;
            Folder f = model.AddFolder(proj, nullStr);
        }

        /// <summary>
        /// Tests that folders are actually removed
        /// </summary>
        [TestMethod]
        public void TestRemoveFolder() {
            Folder f = model.AddFolder(proj, "New Test Folder");
            
            //assert that the project now contains this folder
            Assert.IsTrue(model.GetProjects(user).First(project => project.Id == proj.Id).GetFolders().Count(folder => folder.Id == f.Id) > 0);
            model.RemoveFolder(f);
            //assert that the above is no longer true
            Assert.IsFalse(model.GetProjects(user).First(project => project.Id == proj.Id).GetFolders().Count(folder => folder.Id == f.Id) > 0);
        }

        /// <summary>
        /// Test that trying to remove a non-existing folder will result in an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRemoveFolderNonexistingFolder() {
            Folder f = new Folder();
            model.RemoveFolder(f);
        }

        /// <summary>
        /// Test that trying to remove a null folder will result in an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRemoveFolderNonexistingFolder() {
            Folder f = null;
            model.RemoveFolder(f);
        }

        /// <summary>
        /// Tests that documents returned by AddDocument are proper.
        /// </summary>
        [TestMethod]
        public void TestAddDocument() {
            String name = "WFMTestAddDocumentNewDocument";
            Document testDoc = model.AddDocument(proj, name);

            Assert.AreEqual(name, testDoc.Title);
            Assert.AreNotEqual(0, testDoc.Id);
            Assert.AreEqual(proj.Id, testDoc.Parent.Id);
        }

        /// <summary>
        /// Tests that documents with null parents can't be added
        /// </summary>
        [TestMethod]
        [ExcpectedException(typeof(ArgumentNullException))]
        public void TestAddNullParentDocument() {
            String name = "WFMTestAddNullDocumentNewDocument";
	    Project p = null;
            Document testDoc = model.AddDocument(p, name);
        }

        /// <summary>
        /// Tests that documents with null name can't be added
        /// </summary>
        [TestMethod]
        [ExcpectedException(typeof(ArgumentNullException))]
        public void TestAddNullNameDocument() {
            String name = null;
            Document testDoc = model.AddDocument(proj, name);
        }

        /// <summary>
        /// Tests that documents with null parents and name can't be added
        /// </summary>
        [TestMethod]
        [ExcpectedException(typeof(ArgumentNullException))]
        public void TestAddNullParentDocument() {
            String name = null;
            Project p = null;
            Document testDoc = model.AddDocument(p, name);
        }

        /// <summary>
        /// Tests that documents are saved correctly
        /// </summary>
        [TestMethod]
        public void TestSaveDocument() {
            String rev = @"the crucifix round my neck! for it is a comfort and a strength 
to me whenever I touch it. It is odd that a thing which I have 
been taught to regard with disfavour and as idolatrous should 
in a time of loneliness and trouble be of help.".Trim();
                String rev2 = @"Midnight. I have had a long talk with the Count. I asked 
him a few questions on Transylvania history, and he warmed 
up to the subject wonderfully.".Trim();
            Document testDoc = model.AddDocument(proj, "WFMTestAddDocumentNewDocument");

            testDoc.CurrentRevision = rev;
            model.SaveDocument(testDoc);
            testDoc = model.GetDocument(testDoc.Id);
            Assert.AreEqual(rev, model.GetProjects(user).First(p => p.Id == proj.Id).GetDocuments().First(doc => doc.Id == testDoc.Id).CurrentRevision);

            //Test that savea dds revision and stuff
            testDoc.CurrentRevision = rev2;
            model.SaveDocument(testDoc);

            //Document freshFetchDoc = model.GetProjects(user).First(p => p.Id == proj.Id).GetDocuments().First(doc => doc.Id == testDoc.Id);

            Document freshFetchDoc = model.GetDocument(testDoc.Id);

            Assert.AreEqual(rev2.Trim(), freshFetchDoc.CurrentRevision.Trim());
            Assert.IsTrue(freshFetchDoc.GetRevisions().Count() > 1);
            Assert.AreEqual(rev.Trim(),freshFetchDoc.GetRevisions().Last().Trim());
            Assert.AreEqual(rev2.Trim(),freshFetchDoc.GetRevisions().First().Trim());
        }

        /// <summary>
        /// Asserts that saving a document that does not exist throws an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSaveDocumentNonexistingDocument() {
            Document d = new Document();
            model.SaveDocument(d);
        }

        /// <summary>
        /// Asserts that saving a null document throws an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSaveDocumentNullDocument() {
            Document d = null;
            model.SaveDocument(d);
        }

        /// <summary>
        /// Tests that documents are removed properly
        /// </summary>
        [TestMethod]
        public void TestRemoveDocument() {
            Document d = model.AddDocument(proj, "WFMNewRemoveDocumentDocument");

            //assert that the project now contains this document
            Assert.IsTrue(model.GetProjects(user).First(project => project.Id == proj.Id).GetDocuments().Count(doc => doc.Id == d.Id) > 0);
            model.RemoveDocument(d);
            //assert that the above is no longer true
            Assert.IsFalse(model.GetProjects(user) .First(project => project.Id == proj.Id).GetDocuments().Count(doc => doc.Id == d.Id) > 0);
        }

        /// <summary>
        /// Asserts that deleting a document that does not exist throws an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRemoveDocumentNonexistingDocument() {
            Document d = new Document() { Title = "WFMRemoveNonexistingDocumentDocument" };
            model.RemoveDocument(d);
        }

        /// <summary>
        /// Asserts that deleting a null document throws an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRemoveDocumentNullDocument() {
            Document d = null;
            model.RemoveDocument(d);
        }
    }
}
