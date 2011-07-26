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
using System.IO;
using System.Reflection;

namespace TinyTest
{
    /// <summary>
    /// Test execution engine class.
    /// </summary>
    public class TestEngine
    {
        /// <summary>
        /// Constructor that initializes the object from a configuration object.
        /// </summary>
        /// <param name="configuration">Configuration object to be used.</param>
        public TestEngine(ITestRunConfiguration configuration)
        {
            m_Configuration = configuration;
        }

        /// <summary>
        /// Executes all the tests in the contained configuration and adds the
        /// results to the specified collector.
        /// </summary>
        /// <param name="collector">Collector object to be used.</param>
        public void Execute(ITestResultCollector collector)
        {
            string enginePath = getEnginePath();
            m_Collector = collector;
            foreach (string module in m_Configuration.TestModules)
            {
                executeTestsIn(Path.Combine(enginePath, module));
            }
        }

        private void executeTestsIn(string module)
        {
            TestModuleInfo info = null;
            try
            {
                info = new TestModuleInfo(module);
                foreach (TestClassInfo testClass in info.TestClasses)
                {
                    TestClassResult results = testClass.ExecuteTestCases();
                    m_Collector.AddTestClassResults(results);
                }
                
            }
            catch (System.Exception e)
            {
                m_Collector.AddModuleLoadFailure(module, e.Message);
            }

        }
        private string getEnginePath()
        {
            Assembly engineAssembly = Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(engineAssembly.Location);
        }

        private ITestRunConfiguration m_Configuration;
        private ITestResultCollector m_Collector;
    }
}