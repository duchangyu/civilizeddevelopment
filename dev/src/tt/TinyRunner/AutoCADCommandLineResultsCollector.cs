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

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

using TinyTest;

namespace TinyRunner
{
    /// <summary>
    /// Implements a test result collector interface to show the results in the
    /// AutoCAD command line.
    /// </summary>
    public class AutoCADCommandLineResultsCollector : ITestResultCollector
    {
        /// <summary>
        /// Constructor that initializes the object to redirect the
        /// results to the AutoCAD command line.
        /// </summary>
        public AutoCADCommandLineResultsCollector()
        {
            HidePassingTests = true;
            clearStats();
        }

        /// <summary>
        /// Displays module load failure.
        /// </summary>
        /// <param name="moduleName">Name of the module that failed to load.</param>
        /// <param name="exceptionMessage">Information about the failure.</param>
        public void AddModuleLoadFailure(string moduleName, string exceptionMessage)
        {
            string result = String.Format("ERROR: Module load failure. Module = {0}, Message = {1}",
                moduleName, exceptionMessage);
            _editor.WriteMessage("\n" + result);
        }

        /// <summary>
        /// Adds a test result to the collector.
        /// </summary>
        /// <param name="results">Test result object to be added.</param>
        public void AddTestClassResults(TestClassResult results)
        {
            foreach (TestResult result in results.Results)
            {
                collectStats(result);
                if (showAllResults() || didNotSucceed(result))
                {
                    showResult(result);    
                }
            }
            showStats();
            clearStats();
        }

        /// <summary>
        /// Allows hiding output for tests that pass. It helps to filter out
        /// results and only show the tests that failed.
        /// </summary>
        public bool HidePassingTests { get; set; }

        private Editor _editor 
        {
            get 
            {
                return Application.DocumentManager.MdiActiveDocument.Editor;
            }
        }

        private void collectStats(TestResult result)
        {
            if (m_TestClassName == String.Empty)
            {
                m_TestClassName = result.TestClassName;
            }
            m_TotalCounter++;
            switch(result.ResultType)
            {
                case ResultType.Failure:
                    m_FailCounter++;
                    break;
                case ResultType.Error:
                    m_ErrorCounter++;
                    break;
                default:
                    m_PassCounter++;
                    break;
            }
        }

        private void showStats()
        {
            string msg = String.Format("\nTest Class {0} Results.\nTotal tests executed: {1}\n... Pass: {2}\n... Failed: {3}\n... Errors: {4}",
                m_TestClassName, m_TotalCounter, m_PassCounter, m_FailCounter, m_ErrorCounter);
            _editor.WriteMessage(msg);
        }

        private void clearStats()
        {
            m_TestClassName = String.Empty;
            m_TotalCounter = 0;
            m_PassCounter = 0;
            m_FailCounter = 0;
            m_ErrorCounter = 0;
        }

        private void showResult(TestResult result)
        {
            string message = formatResult(result);
            _editor.WriteMessage("\n" + message);
        }

        private string formatResult(TestResult result)
        {
            string resultTypeString = resultTypeToString(result.ResultType);
            string formattingString = "{0}: {1} - {2}\n... {3}\n... {4}";
            string formatted = String.Format(formattingString, resultTypeString, result.TestClassName, result.TestMethodName,
                result.Description, result.Information);
            return formatted;
        }

        private string resultTypeToString(ResultType type)
        {
            if (type == ResultType.Success)
            {
                return "PASS";
            }
            else if (type == ResultType.Failure)
            {
                return "FAILED";
            }
            return "ERROR";
        }

        private bool showAllResults()
        {
            return !HidePassingTests;
        }

        private bool didNotSucceed(TestResult result)
        {
            return result.ResultType != ResultType.Success;
        }

        private string m_TestClassName;
        private int m_TotalCounter;
        private int m_PassCounter;
        private int m_FailCounter;
        private int m_ErrorCounter;
    }
}