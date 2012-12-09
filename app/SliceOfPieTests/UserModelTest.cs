using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SliceOfPie.Tests {
    [TestClass]
    public class UserModelTest {
        UserModel userModel = new UserModel();

        /// <summary>
        /// Assert that a known user can login correctly.
        /// </summary>
        [TestMethod]
        public void TestValidateLoginSuccess() {
            Assert.IsTrue(userModel.ValidateLogin("me@hypesystem.dk", "pw"));
        }

        /// <summary>
        /// Assert that failure to login returns false.
        /// </summary>
        [TestMethod]
        public void TestValidateLoginFailure() {
            Assert.IsFalse(userModel.ValidateLogin("me@hypesystem.dk", "WrongPassword"));
        }
    }
}
