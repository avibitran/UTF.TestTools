using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UTF.TestTools.Reporters;

namespace UTF.TestTools
{
    public static class AssertExtensions
    {
        #region Fields

        #endregion Fields

        #region Ctor

        #endregion Ctor

        #region Methods
        /// <summary>
        /// Reports a passed step
        /// </summary>
        /// <param name="assert">The calling assert.</param>
        /// <param name="step">The <see cref="StepInfo"/> to report to.</param>
        /// <param name="actualState">The message to include as the actual result in the step, and in the exception.</param>
        /// <exception cref="AssertFailedException">Thrown if <paramref name="continueOnError"/> is true.</exception>
        public static void ReportPass(this Assert assert, StepInfo step, string actualState)
        {
            if(actualState != null)
                step.Actual.Add(actualState);

            if (((int)StepStatusEnum.Pass) > ((int)step.Outcome))
            {
                step.Outcome = StepStatusEnum.Pass;
            }
        }

        /// <summary>
        /// Reports a failed step, and throws an AssertFailedException if needed.
        /// </summary>
        /// <param name="assert">The calling assert.</param>
        /// <param name="step">The <see cref="StepInfo"/> to report to.</param>
        /// <param name="actualState">The message to include as the actual result in the step, and in the exception.</param>
        /// <param name="continueOnError">If true, not exception is thrown. if false, <see cref="AssertFailedException"/> is thrown if the condition fails.</param>
        /// <exception cref="AssertFailedException">Thrown if condition fails and <paramref name="continueOnError"/> is false.</exception>
        public static void ReportFail(this Assert assert, StepInfo step, string actualState, bool continueOnError = false)
        {
            if (actualState != null)
                step.Actual.Add(actualState);

            if (((int)StepStatusEnum.Fail) > ((int)step.Outcome))
            {
                step.Outcome = StepStatusEnum.Fail;
            }

            if(!continueOnError)
                throw new AssertFailedException(actualState);
        }

        /// <summary>
        /// Reports a failed step, and throws an AssertInconclusiveException if needed.
        /// </summary>
        /// <param name="assert">The calling assert.</param>
        /// <param name="step">The <see cref="StepInfo"/> to report to.</param>
        /// <param name="actualState">The message to include as the actual result in the step, and in the exception.</param>
        /// <param name="continueOnError">If true, not exception is thrown. if false, <see cref="AssertFailedException"/> is thrown if the condition fails.</param>
        /// <exception cref="AssertInconclusiveException">Thrown if condition fails and <paramref name="continueOnError"/> is false.</exception>
        public static void ReportInconclusive(this Assert assert, StepInfo step, string actualState, bool continueOnError = false)
        {
            if (actualState != null)
                step.Actual.Add(actualState);

            if (((int)StepStatusEnum.Fail) > ((int)step.Outcome))
            {
                step.Outcome = StepStatusEnum.Fail;
            }

            if (!continueOnError)
                throw new AssertInconclusiveException(actualState);
        }

        /// <summary>
        /// Tests whether the specified condition is true, reports it and throws an exception if the
        ///     condition is false and <paramref name="continueOnError"/> is false.
        /// </summary>
        /// <param name="assert">The calling assert.</param>
        /// <param name="condition">The condition the test expects to be true.</param>
        /// <param name="step">The <see cref="StepInfo"/> to report to.</param>
        /// <param name="actualOnFail">The message to include as the actual result in the step, and in the exception if the tested condition is false.</param>
        /// <param name="actualOnPass">The message to include as the actual result in the step, and in the exception if the tested condition is true.</param>
        /// <param name="continueOnError">If true, not exception is thrown. if false, <see cref="AssertFailedException"/> is thrown if the condition fails.</param>
        /// <exception cref="AssertFailedException">Thrown if condition fails and <paramref name="continueOnError"/> is false.</exception>
        public static void ReportIsTrue(this Assert assert, bool condition, StepInfo step, string actualOnFail, string actualOnPass, bool continueOnError = false)
        {
            try
            {
                Assert.IsTrue(condition);
                ReportPass(assert, step, actualOnPass);
            }
            catch { ReportFail(assert, step, actualOnFail, continueOnError); }
        }

        /// <summary>
        /// Tests whether the specified condition is false, reports it 
        ///     and throws an exception if the condition fails and <paramref name="continueOnError"/> is false.
        /// </summary>
        /// <param name="assert">The calling assert.</param>
        /// <param name="condition">The condition the test expects to be false.</param>
        /// <param name="step">The <see cref="StepInfo"/> to report to.</param>
        /// <param name="actualOnFail">The message to include as the actual result in the step, and in the exception if the tested condition is true.</param>
        /// <param name="actualOnPass">The message to include as the actual result in the step, and in the exception if the tested condition is false.</param>
        /// <param name="continueOnError">If true, not exception is thrown. if false, <see cref="AssertFailedException"/> is thrown if the condition fails.</param>
        /// <exception cref="AssertFailedException">Thrown if condition fails and <paramref name="continueOnError"/> is false.</exception>
        public static void ReportIsFalse(this Assert assert, bool condition, StepInfo step, string actualOnFail, string actualOnPass, bool continueOnError = false)
        {
            try
            {
                Assert.IsFalse(condition);
                ReportPass(assert, step, actualOnPass);
            }
            catch { ReportFail(assert, step, actualOnFail, continueOnError); }
        }

        /// <summary>
        /// Tests whether the specified object is null, reports it 
        ///     and throws an exception if the condition fails and <paramref name="continueOnError"/> is false.
        /// </summary>
        /// <param name="assert">The calling assert.</param>
        /// <param name="value">The object the test expects to be null.</param>
        /// <param name="step">The <see cref="StepInfo"/> to report to.</param>
        /// <param name="actualOnFail">The message to include as the actual result in the step, and in the exception if the tested value is non-null.</param>
        /// <param name="actualOnPass">The message to include as the actual result in the step, and in the exception if the tested value is null.</param>
        /// <param name="continueOnError">If true, not exception is thrown. if false, <see cref="AssertFailedException"/> is thrown if the condition fails.</param>
        /// <exception cref="AssertFailedException">Thrown if condition fails and <paramref name="continueOnError"/> is false.</exception>
        public static void ReportIsNull(this Assert assert, object value, StepInfo step, string actualOnFail, string actualOnPass, bool continueOnError = false)
        {
            try
            {
                Assert.IsNull(value);
                ReportPass(assert, step, actualOnPass);
            }
            catch { ReportFail(assert, step, actualOnFail, continueOnError); }
        }

        /// <summary>
        /// Tests whether the specified object is non-null, reports it 
        ///     and throws an exception if the condition fails and <paramref name="continueOnError"/> is false.
        /// </summary>
        /// <param name="assert">The calling assert.</param>
        /// <param name="value">The object the test expects to be non-null.</param>
        /// <param name="step">The <see cref="StepInfo"/> to report to.</param>
        /// <param name="actualOnFail">The message to include as the actual result in the step, and in the exception if the tested value is null.</param>
        /// <param name="actualOnPass">The message to include as the actual result in the step, and in the exception if the tested value is non-null.</param>
        /// <param name="continueOnError">If true, not exception is thrown. if false, <see cref="AssertFailedException"/> is thrown if the condition fails.</param>
        /// <exception cref="AssertFailedException">Thrown if condition fails and <paramref name="continueOnError"/> is false.</exception>
        public static void ReportIsNotNull(this Assert assert, object value, StepInfo step, string actualOnFail, string actualOnPass, bool continueOnError = false)
        {
            try
            {
                Assert.IsNotNull(value);
                ReportPass(assert, step, actualOnPass);
            }
            catch { ReportFail(assert, step, actualOnFail, continueOnError); }
        }

        /// <summary>
        /// Tests whether the specified object is non-null, reports it 
        ///     and throws an exception if the condition fails and <paramref name="continueOnError"/> is false.
        /// </summary>
        /// <param name="assert">The calling assert.</param>
        /// <param name="expected">The first object to compare. This is the object the tests expects.</param>
        /// <param name="actual">The second object to compare. This is the object produced by the code under test.</param>
        /// <param name="step">The <see cref="StepInfo"/> to report to.</param>
        /// <param name="actualOnFail">The message to include as the actual result in the step, and in the exception if the tested value is null.</param>
        /// <param name="actualOnPass">The message to include as the actual result in the step, and in the exception if the tested value is non-null.</param>
        /// <param name="continueOnError">If true, not exception is thrown. if false, <see cref="AssertFailedException"/> is thrown if the condition fails.</param>
        /// <exception cref="AssertFailedException">Thrown if condition fails and <paramref name="continueOnError"/> is false.</exception>
        public static void ReportAreEqual<T>(this Assert assert, T expected, T actual, StepInfo step, string actualOnFail, string actualOnPass, bool continueOnError = false)
        {
            if (!expected.GetType().Equals(actual.GetType()))
            {
                ReportFail(assert, step, actualOnFail, continueOnError);
                return;
            }

            try
            {
                Assert.AreEqual<T>(expected, actual);
                ReportPass(assert, step, actualOnPass);
            }
            catch { ReportFail(assert, step, actualOnFail, continueOnError); }
        }

        /// <summary>
        /// Tests whether the specified object is non-null, reports it 
        ///     and throws an exception if the condition fails and <paramref name="continueOnError"/> is false.
        /// </summary>
        /// <param name="assert">The calling assert.</param>
        /// <param name="notExpected">The first object to compare. This is the object the tests expects not
        ///     to match <paramref name="actual"/>.
        /// <param name="actual">The second object to compare. This is the object produced by the code under test.</param>
        /// <param name="step">The <see cref="StepInfo"/> to report to.</param>
        /// <param name="actualOnFail">The message to include as the actual result in the step, and in the exception if the tested value is null.</param>
        /// <param name="actualOnPass">The message to include as the actual result in the step, and in the exception if the tested value is non-null.</param>
        /// <param name="continueOnError">If true, not exception is thrown. if false, <see cref="AssertFailedException"/> is thrown if the condition fails.</param>
        /// <exception cref="AssertFailedException">Thrown if condition fails and <paramref name="continueOnError"/> is false.</exception>
        public static void ReportAreNotEqual<T>(this Assert assert, T notExpected, T actual, StepInfo step, string actualOnFail, string actualOnPass, bool continueOnError = false)
        {
            if (notExpected.GetType().Equals(actual.GetType()))
            {
                ReportFail(assert, step, actualOnFail, continueOnError);
                return;
            }

            try
            {
                Assert.AreNotEqual<T>(notExpected, actual);
                ReportPass(assert, step, actualOnPass);
            }
            catch { ReportFail(assert, step, actualOnFail, continueOnError); }
        }

        public static T ReportThrowsException<T>(this Assert assert, Action action, StepInfo step, string actualOnFail, string actualOnPass, bool continueOnError = false) 
            where T : Exception
        {
            try
            {
                T exception = Assert.ThrowsException<T>(action);
                ReportPass(assert, step, actualOnPass);
                return exception;
            }
            catch (AssertFailedException assertFailed)
            {
                step.Messages.Add(assertFailed.Message);
                ReportFail(assert, step, actualOnFail, continueOnError);
                return null;
            }
        }

        public static T ReportThrowsException<T>(this Assert assert, Func<object> action, StepInfo step, string actualOnFail, string actualOnPass, bool continueOnError = false)
            where T : Exception
        {
            try
            {
                T exception = Assert.ThrowsException<T>(action);
                ReportPass(assert, step, actualOnPass);
                return exception;
            }
            catch (AssertFailedException assertFailed)
            {
                step.Messages.Add(assertFailed.Message);
                ReportFail(assert, step, actualOnFail, continueOnError);
                return null;
            }
        }

        public static Task<T> ReportThrowsExceptionAsync<T>(this Assert assert, Func<Task> action, StepInfo step, string actualOnFail, string actualOnPass, bool continueOnError = false)
            where T : Exception
        {
            try
            {
                Task<T> exception = Assert.ThrowsExceptionAsync<T>(action);
                ReportPass(assert, step, actualOnPass);
                return exception;
            }
            catch (AssertFailedException assertFailed)
            {
                step.Messages.Add(assertFailed.Message);
                ReportFail(assert, step, actualOnFail, continueOnError);
                return null;
            }
        }
        #endregion Methods

        #region Properties

        #endregion Properties
    }
}
