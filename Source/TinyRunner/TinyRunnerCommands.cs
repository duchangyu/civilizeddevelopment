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

using Autodesk.AutoCAD.Runtime;

using TinyTest;

[assembly: CommandClass(typeof(TinyRunner.TinyRunnerCommands))]

namespace TinyRunner
{
    /// <summary>
    /// Command class that implements commands that allow to run
    /// unit tests inside of AutoCAD.
    /// </summary>
    public class TinyRunnerCommands
    {
        [CommandMethod("RunTestsCS")]
        public void RunTestsCS()
        {
            executeTestConfiguration(new XMLConfiguration("ColibraTestsCS.xml"), new AutoCADCommandLineResultsCollector());
        }

        [CommandMethod("RunTestsVB")]
        public void RunTestsVB()
        {
            executeTestConfiguration(new XMLConfiguration("ColibraTestsVB.xml"), new AutoCADCommandLineResultsCollector());
        }

        /// <summary>
        /// This command runs the tests that validate the implementation
        /// of the unit test framework. It does not run the Colibra
        /// unit tests.
        /// </summary>
        [CommandMethod("RunFrameworkTests")]
        public void RunFrameworkTests()
        {
            executeTestConfiguration(new XMLConfiguration("FrameworkTests.xml"), new AutoCADCommandLineResultsCollector());
        }

        private void executeTestConfiguration(ITestRunConfiguration configuration, ITestResultCollector collector)
        {
            // We set the working directory to the location of this running assembly
            // to allow the framework to easily find the files that are needed (configuration
            // files, DLL's, etc. After we execute the tests, we want to restore the working
            // directory to where it was.
            //
            setWorkingDirectoryToThisAssemblyLocation();
            executeTests(configuration, collector);
            restoreWorkingDirectory();
        }


        private void setWorkingDirectoryToThisAssemblyLocation()
        {
            m_StartUpWorkingDir = Directory.GetCurrentDirectory();
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            string assemblyDir = Path.GetDirectoryName(
                thisAssembly.Location);
            Directory.SetCurrentDirectory(assemblyDir);
        }


        private void restoreWorkingDirectory()
        {
            Directory.SetCurrentDirectory(m_StartUpWorkingDir);
        }


        private void executeTests(ITestRunConfiguration configuration, ITestResultCollector collector)
        {
            TestEngine engine = new TestEngine(configuration);
            engine.Execute(collector);
        }


        private string m_StartUpWorkingDir;
    }
}