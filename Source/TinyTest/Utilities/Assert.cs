// (C) Copyright 2011 Autodesk, Inc.  All rights reserved.
//
// Permission to use, copy, modify, and distribute these source code samples is
// hereby granted, provided that (i) you must clearly identify any modified 
// source code files and any resulting binary files as works developed by you,
// and not by Autodesk;  and (ii) you may distribute the resulting binary files
// of the source code samples in works that are commercially distributed 
// software applications only if:  (a) such applications require an Autodesk
// product to operate; and (b) such applications contain, subject to Autodesk's
// sole discretion, significant features and functionality in addition to the 
// source code samples so that the source code samples are not the primary
// source of value.  In any copy of the source code samples, derivative works,
// and resulting binary files, you must include the copyright notices of 
// Autodesk, Inc., the limited warranty and restricted rights notice below, and
// (if modified) the following statement: "This software contains copyrighted 
// code owned by Autodesk but has been modified and is not endorsed by Autodesk
// in its modified form".
//
// AUTODESK PROVIDES THIS SOFTWARE "AS IS" AND WITH ALL FAULTS.  AUTODESK MAKES
// NO WARRANTIES, EXPRESS OR IMPLIED, AS TO NONINFRINGEMENT OF THIRD PARTY
// RIGHTS, MERCHANTABILITY, OR FITNESS FOR ANY PARTICULAR PURPOSE. IN NO EVENT
// WILL AUTODESK BE LIABLE TO YOU FOR ANY CONSEQUENTIAL, INCIDENTAL OR SPECIAL
// DAMAGES, INCLUDING ANY LOST PROFITS OR LOST SAVINGS, EVEN IF AUTODESK HAS
// BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES, OR FOR ANY CLAIM BY ANY
// THIRD PARTY. AUTODESK DOES NOT WARRANT THAT THE OPERATION OF THE SOFTWARE
// WILL BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to 
// restrictions set forth in FAR 52.227-19 (Commercial ComputerSoftware -
// Restricted Rights) and DFAR 252.227-7013(c)(1)(ii) (Rights in Technical Data
// and Computer Software), as applicable.
//
// You may not export the source code samples or any derivative works, 
// resulting binaries, or any related technical documentation,  in violation of
// U.S. or other applicable export control laws.
//
using System;

namespace TinyTest
{
    /// <summary>
    /// Implements assertion methods to evaluate test results.
    /// </summary>
    public static class Assert 
    {
        /// <summary>
        /// Forces failure when called. This method should be used to indicate
        /// a test failure when the evaluation of the test is done outside of this class.
        /// </summary>
        public static void Fail()
        {
            Fail(String.Empty);
        }

        /// <summary>
        /// Forces failure when called. This method should be used to indicate
        /// a test failure when the evaluation of the test is done outside of this class.
        /// </summary>
        /// <param name="message">Additional information about the failure.</param>
        public static void Fail(string message)
        {
            throwAssertionFailure("Forced failure", message);
        }

        /// <summary>
        /// Evaluates the boolean expression and fails if the expression is not true.
        /// </summary>
        /// <param name="expression">Expression to be evaluated.</param>
        public static void IsTrue(bool expression)
        {
            IsTrue(expression, String.Empty);
        }

        /// <summary>
        /// Evaluates the boolean expression and fails if the expression is not true.
        /// </summary>
        /// <param name="expression">Expression to be evaluated.</param>
        /// <param name="message">Additional information about the failure.</param>
        public static void IsTrue(bool expression, string message)
        {
            executeAssertion<bool>(true, expression, message);
        }

        /// <summary>
        /// Evaluates the boolean expression and fails if the expression is not false.
        /// </summary>
        /// <param name="expression">Expression to be evaluated.</param>
        public static void IsFalse(bool expression)
        {
            IsFalse(expression, String.Empty);
        }

        /// <summary>
        /// Evaluates the boolean expression and fails if the expression is not false.
        /// </summary>
        /// <param name="expression">Expression to be evaluated.</param>
        /// <param name="message">Additional information about the failure.</param>
        public static void IsFalse(bool expression, String message)
        {
            executeAssertion<bool>(false, expression, message);
        }

        /// <summary>
        /// Compares the specified integer values for equality and fails if the values
        /// are not equal.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value to be compared.</param>
        public static void AreEqual(int expected, int actual)
        {
            AreEqual(expected, actual, String.Empty);
        }

        /// <summary>
        /// Compares the specified integer values for equality and fails if the values
        /// are not equal.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value to be compared.</param>
        /// <param name="message">Additional information about the failure.</param>
        public static void AreEqual(int expected, int actual, string message)
        {
            executeAssertion<int>(expected, actual, message);
        }

        /// <summary>
        /// Compares the specified double values for equality and fails if the values
        /// are not equal.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value to be compared.</param>
        public static void AreEqual(double expected, double actual)
        {
            AreEqual(expected, actual, String.Empty);
        }

        /// <summary>
        /// Compares the specified double values for equality and fails if the values
        /// are not equal.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value to be compared.</param>
        /// <param name="message">Additional information about the failure.</param>
        public static void AreEqual(double expected, double actual, string message)
        {
            executeAssertion<double>(expected, actual, message);
        }

        /// <summary>
        /// Compares the specified double values for equality considering the specified
        /// tolerance and fails if the difference between the values is greater than the
        /// tolerance value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value to be compared.</param>
        /// <param name="tolerance">Difference tolerance.</param>
        public static void AreEqual(double expected, double actual, double tolerance)
        {
            AreEqual(expected, actual, tolerance, String.Empty);
        }

        /// <summary>
        /// Compares the specified double values for equality considering the specified
        /// tolerance and fails if the difference between the values is greater than the
        /// tolerance value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value to be compared.</param>
        /// <param name="tolerance">Difference tolerance.</param>
        /// <param name="message">Additional information about the failure.</param>
        public static void AreEqual(double expected, double actual, double tolerance, string message)
        {
            if (!isCloseEnoughValue(expected, actual, tolerance))
            {
                throwAssertionFailure(expected.ToString(), actual.ToString(), tolerance.ToString(), message);
            }
        }

        /// <summary>
        /// Compares the specified strings for equality.
        /// </summary>
        /// <param name="expected">Expected string.</param>
        /// <param name="actual">Actual string.</param>
        public static void AreEqual(string expected, string actual)
        {
            AreEqual(expected, actual, String.Empty);
        }

        /// <summary>
        /// Compares the specified strings for equality.
        /// </summary>
        /// <param name="expected">Expected string.</param>
        /// <param name="actual">Actual string.</param>
        /// <param name="message">Additional information in case of failure.</param>
        public static void AreEqual(string expected, string actual, string message)
        {
            executeAssertion<string>(expected, actual, message);
        }

        /// <summary>
        /// Compares the specified types for equality.
        /// </summary>
        /// <param name="expected">Expected type.</param>
        /// <param name="actual">Actual type.</param>
        public static void AreEqual(Type expected, Type actual)
        {
            AreEqual(expected, actual, String.Empty);
        }

        /// <summary>
        /// Compares the specified types for equality.
        /// </summary>
        /// <param name="expected">Expected type.</param>
        /// <param name="actual">Actual type.</param>
        /// <param name="message">Additional information in case of failure.</param>
        public static void AreEqual(Type expected, Type actual, string message)
        {
            executeAssertion<Type>(expected, actual, message);
        }

        private static void executeAssertion<T>(T expected, T actual, string message)
        {
            if(!isExpectedValue<T>(expected, actual))
            {
                throwAssertionFailure(expected.ToString(), actual.ToString(), message);
            }
        }

        private static bool isExpectedValue<T>(T expected, T actual)
        {
            return expected.Equals(actual);
        }

        private static bool isCloseEnoughValue(double expected, double actual, double tolerance)
        {
            double absoluteDelta = Math.Abs(expected - actual);
            return (absoluteDelta <= tolerance);
        }

        private static void throwAssertionFailure(string assertionMsg, string message)
        {
            string msg = String.Format("{0}. {1}", assertionMsg, message);
            throw new AssertionFailureException(msg);
        }

        private static void throwAssertionFailure(string expected, string actual, string message)
        {
            string msg = String.Format("Expected=<{0}>, actual=<{1}>. {2}", expected, actual, message);
            throw new AssertionFailureException(msg);
        }

        private static void throwAssertionFailure(string expected, string actual, string tolerance, string message)
        {
            string msg = String.Format("Expected=<{0}>, actual=<{1}>, tolerance=<{2}>. {3}",
                expected, actual, tolerance, message);
            throw new AssertionFailureException(msg);
        }
    }
}