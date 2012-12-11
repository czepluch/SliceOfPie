﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SliceOfPie.Tests {
    [TestClass]
    public class UserModelTest {
        string AppPath;
        UserModel userModel = new UserModel();

        public UserModelTest() {
            AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SliceOfPie");
        }

        /// <summary>
        /// Assert that a known user can login correctly.
        /// </summary>
        [TestMethod]
        public void TestValidateLoginSuccess() {
            Assert.IsTrue(userModel.ValidateLogin("common@test.mail", "pw"));
        }

        /// <summary>
        /// Assert that failure to login returns false.
        /// </summary>
        [TestMethod]
        public void TestValidateLoginFailure() {
            Assert.IsFalse(userModel.ValidateLogin("common@test.mail", "WrongPassword"));
        }

        [TestMethod]
        public void TestShareProject() {
            TestHelper.ClearFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SliceOfPie"));

            LocalFileModel model = new LocalFileModel();

            IEnumerable<Project> projects = model.GetProjects("local");
            Project project = projects.First();

            model.UploadStructure("common@test.mail");
            model.FindProjects();
            projects = model.GetProjects("local");
            project = projects.First();

            Assert.AreEqual(1, projects.Count());

            userModel.ShareProject(project.Id, "me@hypesystem.dk");

            using (var dbContext = new sliceofpieEntities2()) {
                var projectUsers = from projectUser in dbContext.ProjectUsers
                                   where projectUser.ProjectId == project.Id && projectUser.UserEmail == "me@hypesystem.dk"
                                   select projectUser;
                Assert.AreEqual(1, projectUsers.Count());
            }
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
