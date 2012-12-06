using SliceOfPie.ApmHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SliceOfPie.Tests
{
    
    
    /// <summary>
    ///This is a test class for AsyncResultTest and is intended
    ///to contain all AsyncResultTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AsyncResultTest {


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
        ///A test for SetAsCompleted
        ///</summary>
        public void SetAsCompletedTestHelper<TResult>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResult<TResult> target = new AsyncResult<TResult>(asyncCallback, state); // TODO: Initialize to an appropriate value
            TResult result = default(TResult); // TODO: Initialize to an appropriate value
            bool completedSynchronously = false; // TODO: Initialize to an appropriate value
            target.SetAsCompleted(result, completedSynchronously);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod()]
        public void SetAsCompletedTest() {
            SetAsCompletedTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for EndInvoke
        ///</summary>
        public void EndInvokeTestHelper<TResult>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResult<TResult> target = new AsyncResult<TResult>(asyncCallback, state); // TODO: Initialize to an appropriate value
            TResult expected = default(TResult); // TODO: Initialize to an appropriate value
            TResult actual;
            actual = target.EndInvoke();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void EndInvokeTest() {
            EndInvokeTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for AsyncResult`1 Constructor
        ///</summary>
        public void AsyncResultConstructorTest3Helper<TResult>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            AsyncResult<TResult> target = new AsyncResult<TResult>(asyncCallback, state);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestMethod()]
        public void AsyncResultConstructorTest3() {
            AsyncResultConstructorTest3Helper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for Parameter1
        ///</summary>
        public void Parameter1TestHelper<TResult, TParameter>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            TParameter param1 = default(TParameter); // TODO: Initialize to an appropriate value
            AsyncResult<TResult, TParameter> target = new AsyncResult<TResult, TParameter>(asyncCallback, state, param1); // TODO: Initialize to an appropriate value
            TParameter actual;
            actual = target.Parameter1;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void Parameter1Test() {
            Parameter1TestHelper<GenericParameterHelper, GenericParameterHelper>();
        }

        /// <summary>
        ///A test for AsyncResult`2 Constructor
        ///</summary>
        public void AsyncResultConstructorTest2Helper<TResult, TParameter>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            TParameter param1 = default(TParameter); // TODO: Initialize to an appropriate value
            AsyncResult<TResult, TParameter> target = new AsyncResult<TResult, TParameter>(asyncCallback, state, param1);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestMethod()]
        public void AsyncResultConstructorTest2() {
            AsyncResultConstructorTest2Helper<GenericParameterHelper, GenericParameterHelper>();
        }

        /// <summary>
        ///A test for Parameter2
        ///</summary>
        public void Parameter2TestHelper<TResult, TParameter1, TParameter2>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            TParameter1 param1 = default(TParameter1); // TODO: Initialize to an appropriate value
            TParameter2 param2 = default(TParameter2); // TODO: Initialize to an appropriate value
            AsyncResult<TResult, TParameter1, TParameter2> target = new AsyncResult<TResult, TParameter1, TParameter2>(asyncCallback, state, param1, param2); // TODO: Initialize to an appropriate value
            TParameter2 actual;
            actual = target.Parameter2;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void Parameter2Test() {
            Parameter2TestHelper<GenericParameterHelper, GenericParameterHelper, GenericParameterHelper>();
        }

        /// <summary>
        ///A test for AsyncResult`3 Constructor
        ///</summary>
        public void AsyncResultConstructorTest1Helper<TResult, TParameter1, TParameter2>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            TParameter1 param1 = default(TParameter1); // TODO: Initialize to an appropriate value
            TParameter2 param2 = default(TParameter2); // TODO: Initialize to an appropriate value
            AsyncResult<TResult, TParameter1, TParameter2> target = new AsyncResult<TResult, TParameter1, TParameter2>(asyncCallback, state, param1, param2);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestMethod()]
        public void AsyncResultConstructorTest1() {
            AsyncResultConstructorTest1Helper<GenericParameterHelper, GenericParameterHelper, GenericParameterHelper>();
        }

        /// <summary>
        ///A test for Parameter3
        ///</summary>
        public void Parameter3TestHelper<TResult, TParameter1, TParameter2, TParameter3>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            TParameter1 param1 = default(TParameter1); // TODO: Initialize to an appropriate value
            TParameter2 param2 = default(TParameter2); // TODO: Initialize to an appropriate value
            TParameter3 param3 = default(TParameter3); // TODO: Initialize to an appropriate value
            AsyncResult<TResult, TParameter1, TParameter2, TParameter3> target = new AsyncResult<TResult, TParameter1, TParameter2, TParameter3>(asyncCallback, state, param1, param2, param3); // TODO: Initialize to an appropriate value
            TParameter3 actual;
            actual = target.Parameter3;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void Parameter3Test() {
            Parameter3TestHelper<GenericParameterHelper, GenericParameterHelper, GenericParameterHelper, GenericParameterHelper>();
        }

        /// <summary>
        ///A test for AsyncResult`4 Constructor
        ///</summary>
        public void AsyncResultConstructorTestHelper<TResult, TParameter1, TParameter2, TParameter3>() {
            AsyncCallback asyncCallback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            TParameter1 param1 = default(TParameter1); // TODO: Initialize to an appropriate value
            TParameter2 param2 = default(TParameter2); // TODO: Initialize to an appropriate value
            TParameter3 param3 = default(TParameter3); // TODO: Initialize to an appropriate value
            AsyncResult<TResult, TParameter1, TParameter2, TParameter3> target = new AsyncResult<TResult, TParameter1, TParameter2, TParameter3>(asyncCallback, state, param1, param2, param3);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestMethod()]
        public void AsyncResultConstructorTest() {
            AsyncResultConstructorTestHelper<GenericParameterHelper, GenericParameterHelper, GenericParameterHelper, GenericParameterHelper>();
        }
    }
}
