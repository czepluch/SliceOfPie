using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SliceOfPie.ApmHelpers {

    #region AsyncResult without return

    /// <summary>
    /// Based on a class by Jeffrey Richter.
    /// </summary>
    public class AsyncResultNoResult : IAsyncResult {
        // Fields set at construction which never change while 
        // operation is pending
        private readonly AsyncCallback m_AsyncCallback;
        private readonly object m_AsyncState;

        // Fields set at construction which do change after 
        // operation completes
        private const int c_StatePending = 0;
        private const int c_StateCompletedSynchronously = 1;
        private const int c_StateCompletedAsynchronously = 2;
        private int m_CompletedState = c_StatePending;

        // Field that may or may not get set depending on usage
        private ManualResetEvent m_AsyncWaitHandle;

        // Fields set when operation completes
        private Exception m_exception;

        public AsyncResultNoResult(AsyncCallback asyncCallback, object state) {
            m_AsyncCallback = asyncCallback;
            m_AsyncState = state;
        }

        public void SetAsCompleted(
           Exception exception, Boolean completedSynchronously) {
            // Passing null for exception means no error occurred. 
            // This is the common case
            m_exception = exception;

            // The m_CompletedState field MUST be set prior calling the callback
            int prevState = Interlocked.Exchange(ref m_CompletedState,
               completedSynchronously ? c_StateCompletedSynchronously :
               c_StateCompletedAsynchronously);
            if (prevState != c_StatePending)
                throw new InvalidOperationException(
                    "You can set a result only once");

            // If the event exists, set it
            if (m_AsyncWaitHandle != null) m_AsyncWaitHandle.Set();

            // If a callback method was set, call it
            if (m_AsyncCallback != null) m_AsyncCallback(this);
        }

        public void EndInvoke() {
            // This method assumes that only 1 thread calls EndInvoke 
            // for this object
            if (!IsCompleted) {
                // If the operation isn't done, wait for it
                AsyncWaitHandle.WaitOne();
                AsyncWaitHandle.Close();
                m_AsyncWaitHandle = null;  // Allow early GC
            }

            // Operation is done: if an exception occured, throw it
            if (m_exception != null) throw m_exception;
        }

        #region Implementation of IAsyncResult
        public object AsyncState { get { return m_AsyncState; } }

        public Boolean CompletedSynchronously {
            get {
                return Thread.VolatileRead(ref m_CompletedState) ==
                    c_StateCompletedSynchronously;
            }
        }

        public WaitHandle AsyncWaitHandle {
            get {
                if (m_AsyncWaitHandle == null) {
                    Boolean done = IsCompleted;
                    ManualResetEvent mre = new ManualResetEvent(done);
                    if (Interlocked.CompareExchange(ref m_AsyncWaitHandle,
                       mre, null) != null) {
                        // Another thread created this object's event; dispose 
                        // the event we just created
                        mre.Close();
                    }
                    else {
                        if (!done && IsCompleted) {
                            // If the operation wasn't done when we created 
                            // the event but now it is done, set the event
                            m_AsyncWaitHandle.Set();
                        }
                    }
                }
                return m_AsyncWaitHandle;
            }
        }

        public Boolean IsCompleted {
            get {
                return Thread.VolatileRead(ref m_CompletedState) !=
                    c_StatePending;
            }
        }
        #endregion
    }

    /// <summary>
    /// Like AsyncResultNoResult but brings a parameter along as well.
    /// </summary>
    /// <typeparam name="TParameter">Parameter for the represented method.</typeparam>
    public class AsyncResultNoResult<TParameter> : AsyncResultNoResult {
        private TParameter param1;

        public AsyncResultNoResult(AsyncCallback asyncCallback, object state, TParameter parameter)
            : base(asyncCallback, state) {
                param1 = parameter;
        }

        /// <summary>
        /// First parameter of represented method
        /// </summary>
        public TParameter Parameter1 {
            get {
                return param1;
            }
        }
    }

    #endregion

    #region AsyncResult with return

    /// <summary>
    /// Based on a class by Jeffrey Richter.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class AsyncResult<TResult> : AsyncResultNoResult {
        // Field set when operation completes
        private TResult m_result = default(TResult);

        public AsyncResult(AsyncCallback asyncCallback, object state) :
            base(asyncCallback, state) { }

        public void SetAsCompleted(TResult result,
           Boolean completedSynchronously) {
            // Save the asynchronous operation's result
            m_result = result;

            // Tell the base class that the operation completed 
            // sucessfully (no exception)
            base.SetAsCompleted(null, completedSynchronously);
        }

        new public TResult EndInvoke() {
            base.EndInvoke(); // Wait until operation has completed 
            return m_result;  // Return the result (if above didn't throw)
        }
    }

    /// <summary>
    /// This subclass adds a generic parameter to AsyncResult that may be passed along with the AsyncResult.
    /// This follows the same pattern as used in the Func delegates, where the first generic type represents
    /// the return value and the next X types represent parameters.
    /// </summary>
    /// <typeparam name="TResult">Return type of asynchronous method</typeparam>
    /// <typeparam name="TParameter">Parameter for asynchronous method</typeparam>
    /// <seealso cref="AsyncResult"/>
    /// <seealso cref="AsyncResultNoResult"/>
    public class AsyncResult<TResult, TParameter> : AsyncResult<TResult> {

        private readonly TParameter param1;

        public AsyncResult(AsyncCallback asyncCallback, object state, TParameter param1)
            : base(asyncCallback, state) {
                this.param1 = param1;
        }

        /// <summary>
        /// First parameter of method represented by this AsyncResult.
        /// </summary>
        public TParameter Parameter1 {
            get {
                return param1;
            }
        }
    }

    /// <summary>
    /// Like AsyncResult&lt;TResult,TParameter&gt; this version of the class adds another parameter.
    /// </summary>
    /// <typeparam name="TResult">Return type of the asynchronous method this represents</typeparam>
    /// <typeparam name="TParameter1">First parameter of method</typeparam>
    /// <typeparam name="TParameter2">Second parameter of method</typeparam>
    public class AsyncResult<TResult, TParameter1, TParameter2> : AsyncResult<TResult, TParameter1> {

        private readonly TParameter2 param2;

        public AsyncResult(AsyncCallback asyncCallback, object state, TParameter1 param1, TParameter2 param2)
            : base(asyncCallback, state, param1) {
                this.param2 = param2;
        }

        /// <summary>
        /// Second parameter of the method represented by this AsyncResult.
        /// </summary>
        public TParameter2 Parameter2 {
            get {
                return param2;
            }
        }
    }

    /// <summary>
    /// Like its previous versions (with only TParameter og with TParameter1 and TParameter2) this class adds a parameter
    /// for the method it represents.
    /// </summary>
    /// <typeparam name="TResult">Return type of method represented</typeparam>
    /// <typeparam name="TParameter1">First parameter of method</typeparam>
    /// <typeparam name="TParameter2">Second parameter of method</typeparam>
    /// <typeparam name="TParameter3">Third parameter of method</typeparam>
    public class AsyncResult<TResult, TParameter1, TParameter2, TParameter3> : AsyncResult<TResult, TParameter1, TParameter2> {

        private readonly TParameter3 param3;

        public AsyncResult(AsyncCallback asyncCallback, object state, TParameter1 param1, TParameter2 param2, TParameter3 param3)
            : base(asyncCallback, state, param1, param2) {
            this.param3 = param3;
        }

        /// <summary>
        /// Third parameter of the method represented by this class.
        /// </summary>
        public TParameter3 Parameter3 {
            get {
                return param3;
            }
        }
    }

    #endregion

    /// <summary>
    /// Helps create APM methods automatically to avoid code duplication in implementations.
    /// </summary>
    public class ApmMethodFactory {

        #region Create No Result methods on the fly

        #region No Result: End

        private static  Action<IAsyncResult> noResultEndMethod = delegate(IAsyncResult asyncResult) {
                AsyncResultNoResult ar = (AsyncResultNoResult)asyncResult;
                ar.EndInvoke();
            };

        /// <summary>
        /// Returns an End method for usage in APM where the backing method returns void, and takes however many parameters.
        /// </summary>
        /// <returns>APM end method</returns>
        public static Action<IAsyncResult> CreateNoResultEndMethod() {
            return noResultEndMethod;
        }

        #endregion

        #region No Result: Begin, No Parameters

        /// <summary>
        /// Returns a method that works as the begin method of an APM version of the backing method provided.
        /// </summary>
        /// <param name="backingMethod">Method to wrap in APM.</param>
        /// <returns>APM method</returns>
        public static Func<AsyncCallback,object,IAsyncResult> CreateNoResultBeginMethod(Action backingMethod) {
            return delegate(AsyncCallback callback,object state) {
                AsyncResultNoResult ar = new AsyncResultNoResult(callback, state);
                ThreadPool.QueueUserWorkItem((x) => {
                    AsyncResultNoResult result = (AsyncResultNoResult)x;
                    try {
                        backingMethod();
                        result.SetAsCompleted(null, false);
                    }
                    catch(Exception e) {
                        result.SetAsCompleted(e, false);
                    }
                }, ar);

                return ar;
            };
        }

        #endregion

        #region No Result: Begin, One Parameter

        /// <summary>
        /// Returns a method that wraps the backing method in APM. The backing method is a method which takes ONE parameter, defined
        /// as the type argumnt, TParameter.
        /// </summary>
        /// <typeparam name="TParameter">Type of parameter taken by backingMethod</typeparam>
        /// <param name="backingMethod">Method to be wrapped in APM</param>
        /// <returns>APM wrapped method</returns>
        public static Func<TParameter, AsyncCallback, object, IAsyncResult> CreateNoResultBeginMethod<TParameter>(Action<TParameter> backingMethod) {
            return delegate(TParameter parameter, AsyncCallback callback, object state) {
                AsyncResultNoResult<TParameter> ar = new AsyncResultNoResult<TParameter>(callback, state, parameter);
                ThreadPool.QueueUserWorkItem((x) => {
                    AsyncResultNoResult<TParameter> result = (AsyncResultNoResult<TParameter>)x;
                    try {
                        backingMethod(parameter);
                        result.SetAsCompleted(null, false);
                    }
                    catch (Exception e) {
                        result.SetAsCompleted(e, false);
                    }
                }, ar);

                return ar;
            };
        }

        #endregion

        #endregion

    }

}
