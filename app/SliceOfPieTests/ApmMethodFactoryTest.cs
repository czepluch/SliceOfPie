using SliceOfPie.ApmHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SliceOfPie.Tests
{
    
    
    /// <summary>
    ///This is a test class for ApmMethodFactoryTest and is intended
    ///to contain all ApmMethodFactoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ApmMethodFactoryTest {


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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for CreateNoResultEndMethod
        ///</summary>
        [TestMethod()]
        public void CreateNoResultEndMethodTest() {
            Action<IAsyncResult> expected = null; // TODO: Initialize to an appropriate value
            Action<IAsyncResult> actual;
            actual = ApmMethodFactory.CreateNoResultEndMethod();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateNoResultBeginMethod
        ///</summary>
        [TestMethod()]
        public void CreateNoResultBeginMethodTest() {
            Action backingMethod = null; // TODO: Initialize to an appropriate value
            Func<AsyncCallback, object, IAsyncResult> expected = null; // TODO: Initialize to an appropriate value
            Func<AsyncCallback, object, IAsyncResult> actual;
            actual = ApmMethodFactory.CreateNoResultBeginMethod(backingMethod);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateNoResultBeginMethod
        ///</summary>
        public void CreateNoResultBeginMethodTest1Helper<TParameter>() {
            Action<TParameter> backingMethod = null; // TODO: Initialize to an appropriate value
            Func<TParameter, AsyncCallback, object, IAsyncResult> expected = null; // TODO: Initialize to an appropriate value
            Func<TParameter, AsyncCallback, object, IAsyncResult> actual;
            actual = ApmMethodFactory.CreateNoResultBeginMethod<TParameter>(backingMethod);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void CreateNoResultBeginMethodTest1() {
            CreateNoResultBeginMethodTest1Helper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for ApmMethodFactory Constructor
        ///</summary>
        [TestMethod()]
        public void ApmMethodFactoryConstructorTest() {
            ApmMethodFactory target = new ApmMethodFactory();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
