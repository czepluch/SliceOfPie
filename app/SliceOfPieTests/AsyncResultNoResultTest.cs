using SliceOfPie.ApmHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace SliceOfPie.Tests
{
    
    
    /// <summary>
    ///This is a test class for AsyncResultNoResultTest and is intended
    ///to contain all AsyncResultNoResultTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AsyncResultNoResultTest {


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
        ///A test for IsCompleted
        ///</summary>
        [TestMethod()]
        public void IsCompletedTest() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResultNoResult target = new AsyncResultNoResult(asyncCallback, state); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsCompleted;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CompletedSynchronously
        ///</summary>
        [TestMethod()]
        public void CompletedSynchronouslyTest() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResultNoResult target = new AsyncResultNoResult(asyncCallback, state); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CompletedSynchronously;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AsyncWaitHandle
        ///</summary>
        [TestMethod()]
        public void AsyncWaitHandleTest() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResultNoResult target = new AsyncResultNoResult(asyncCallback, state); // TODO: Initialize to an appropriate value
            WaitHandle actual;
            actual = target.AsyncWaitHandle;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AsyncState
        ///</summary>
        [TestMethod()]
        public void AsyncStateTest() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResultNoResult target = new AsyncResultNoResult(asyncCallback, state); // TODO: Initialize to an appropriate value
            object actual;
            actual = target.AsyncState;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetAsCompleted
        ///</summary>
        [TestMethod()]
        public void SetAsCompletedTest() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResultNoResult target = new AsyncResultNoResult(asyncCallback, state); // TODO: Initialize to an appropriate value
            Exception exception = null; // TODO: Initialize to an appropriate value
            bool completedSynchronously = false; // TODO: Initialize to an appropriate value
            target.SetAsCompleted(exception, completedSynchronously);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for EndInvoke
        ///</summary>
        [TestMethod()]
        public void EndInvokeTest() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResultNoResult target = new AsyncResultNoResult(asyncCallback, state); // TODO: Initialize to an appropriate value
            target.EndInvoke();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AsyncResultNoResult Constructor
        ///</summary>
        [TestMethod()]
        public void AsyncResultNoResultConstructorTest1() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResultNoResult target = new AsyncResultNoResult(asyncCallback, state);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Parameter1
        ///</summary>
        public void Parameter1TestHelper<TParameter>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            TParameter parameter = default(TParameter); // TODO: Initialize to an appropriate value
            AsyncResultNoResult<TParameter> target = new AsyncResultNoResult<TParameter>(asyncCallback, state, parameter); // TODO: Initialize to an appropriate value
            TParameter actual;
            actual = target.Parameter1;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void Parameter1Test() {
            Parameter1TestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for AsyncResultNoResult`1 Constructor
        ///</summary>
        public void AsyncResultNoResultConstructorTestHelper<TParameter>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            TParameter parameter = default(TParameter); // TODO: Initialize to an appropriate value
            AsyncResultNoResult<TParameter> target = new AsyncResultNoResult<TParameter>(asyncCallback, state, parameter);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestMethod()]
        public void AsyncResultNoResultConstructorTest() {
            AsyncResultNoResultConstructorTestHelper<GenericParameterHelper>();
        }
    }
}
