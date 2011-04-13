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
using System.Collections.Generic;
using System.Reflection;

namespace TinyTest
{
    /// <summary>
    /// Collects information about the execution of a test class and provides
    /// the collected results.
    /// </summary>
    public class TestClassResult
    {
        /// <summary>
        /// Instantiates a new test class result object.
        /// </summary>
        internal TestClassResult()
        {
            m_Results = new List<TestResult>();
        }

        /// <summary>
        /// Enumerates the results of execution the test class.
        /// </summary>
        public IEnumerable<TestResult> Results
        {
            get
            {
                return m_Results;
            }
        }

        /// <summary>
        /// Adds a failing result due to the inability to instantiate the test class.
        /// </summary>
        /// <param name="testClass">Name of the class that could not be instantiated.</param>
        /// <param name="message">Descriptive message about the failure.</param>
        internal void AddInstantiationFailure(Type testClass, string message)
        {
            addResult(ResultType.Error, testClass.Name, "Class Constructor", "Instantiation failure", message);
        }

        /// <summary>
        /// Adds a failing result when the setup method fails to complete.
        /// </summary>
        /// <param name="testClass">Name of the test class that failed the setup.</param>
        /// <param name="message">Descriptive message about the failure.</param>
        internal void AddSetupFailure(Type testClass, string message)
        {
            addResult(ResultType.Error, testClass.Name, "Setup", "Test setup failure", message);
        }

        /// <summary>
        /// Adds a success result.
        /// </summary>
        /// <param name="testClass">Name of the test class where the test was implemented.</param>
        /// <param name="method">Test method that succeeded.</param>
        internal void AddSuccess(Type testClass, MethodInfo method)
        {
            addResult(ResultType.Success, testClass.Name, method.Name, "Tests succeeded", "PASS");
        }

        /// <summary>
        /// Adds a failing result for a unit test.
        /// </summary>
        /// <param name="testClass">Name of the class that implemented the method that failed.</param>
        /// <param name="method">Test method that failed.</param>
        /// <param name="exception">Exception object raised.</param>
        internal void AddFailure(Type testClass, MethodInfo method, AssertionFailureException exception)
        {
            addResult(ResultType.Failure, testClass.Name, method.Name, "Test failed", exception.Message);
        }

        /// <summary>
        /// Adds a failing result due to an unknown error.
        /// </summary>
        /// <param name="testClass">Name of the class that caused the error.</param>
        /// <param name="method">Method that failed.</param>
        /// <param name="message">Descriptive message about the error.</param>
        internal void AddUnknownFailure(Type testClass, MethodInfo method, string message)
        {
            addResult(ResultType.Failure, testClass.Name, method.Name, "Unknown test failure", message);
        }

        /// <summary>
        /// Adds a failing result due to a Teardown method failure.
        /// </summary>
        /// <param name="testClass">Name of the test class containing the failed Teardown method.</param>
        /// <param name="message">Descriptive message about the error.</param>
        internal void AddTeardownFailure(Type testClass, string message)
        {
            addResult(ResultType.Error, testClass.Name, "TearDown", "Test tear down failure", message);
        }

        private void addResult(ResultType type, string className, string method, string description, string info)
        {
            TestResult result = new TestResult()
            {
                ResultType = type,
                TestClassName = className,
                TestMethodName = method,
                Description = description,
                Information = info
            };
            m_Results.Add(result);
        }

        private List<TestResult> m_Results;
    }
}