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

        [TestMethod]
        public void TestGetProjects() {
            IEnumerable<Project> projects = model.GetProjects("me@michaelstorgaard.com");

            Assert.IsTrue(projects.Count() > 0);
        }
    }
}
