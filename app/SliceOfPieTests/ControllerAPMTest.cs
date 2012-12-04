using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SliceOfPie;

namespace SliceOfPieTests {
    [TestClass]
    public class ControllerAPMTest {
        private Controller controller = Controller.Instance;

        /// <summary>
        /// Tests that projects may be created asynchronously
        /// </summary>
        [TestMethod]
        public void TestProjectCreate() {
            string folderName = "New Project";
            IAsyncResult ar = controller.BeginCreateProject(folderName, "user@mail.com", (a) => {
                //Project p = controller.EndCreateProject(ar);
            }, new object());
        }
    }
}
