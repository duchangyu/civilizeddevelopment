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
    /// Contains information about a test class.
    /// </summary>
    internal class TestClassInfo
    {
        /// <summary>
        /// Constructor that initializes the object using the type of the test class.
        /// </summary>
        /// <param name="testClassType">Type of the test class.</param>
        internal TestClassInfo(Type testClassType)
        {
            m_TestClassType = testClassType;
            m_TestMethods = new List<MethodInfo>();
            foreach (MethodInfo method in m_TestClassType.GetMethods())
            {
                if (method.GetCustomAttributes(typeof(TestMethodAttribute), false).Length > 0)
                {
                    m_TestMethods.Add(method);
                }
            }
        }

        /// <summary>
        /// Executes the test methods identified in the test class.
        /// </summary>
        /// <returns>Returns the results of the execution.</returns>
        internal TestClassResult ExecuteTestCases()
        {
            m_Results = new TestClassResult();
        
            if (canInstantiateTestClass())
            {
                foreach (MethodInfo method in m_TestMethods)
                {
                    if(setupSucceed())
                    {
                        runTest(method);
                    }
                    runTearDown();
                }
            }
 
            return m_Results;
        }

        private bool canInstantiateTestClass()
        {
            try
            {
                ConstructorInfo constructor = m_TestClassType.GetConstructor(Type.EmptyTypes);
                m_Instance = constructor.Invoke(Type.EmptyTypes);
            }
            catch (System.Exception e)
            {
                m_Results.AddInstantiationFailure(m_TestClassType, e.Message);
                return false;
            }
            return true;
        }

        private bool setupSucceed()
        {
            bool success = true;
            MethodInfo setup = m_TestClassType.GetMethod("Setup");
            if (null != setup)
            {
                try
                {
                    setup.Invoke(m_Instance, Type.EmptyTypes);
                }
                catch (System.Exception e)
                {
                    m_Results.AddSetupFailure(m_TestClassType, e.Message);
                    success = false;
                }
                
            }
            return success;
        }

        private void runTest(MethodInfo method)
        {
            try
            {
                method.Invoke(m_Instance, Type.EmptyTypes);
                m_Results.AddSuccess(m_TestClassType, method);
            }
            catch (System.Exception e)
            {
                AssertionFailureException inner = e.InnerException as AssertionFailureException;
                if (inner != null)
                {
                    m_Results.AddFailure(m_TestClassType, method, inner);
                }
                else
                {
                    m_Results.AddUnknownFailure(m_TestClassType, method, e.Message);
                }
            }
        }

        private void runTearDown()
        {
            MethodInfo teardown = m_TestClassType.GetMethod("TearDown");
            if (null != teardown)
            {
                try
                {
                    teardown.Invoke(m_Instance, Type.EmptyTypes);
                }
                catch (System.Exception e)
                {
                    m_Results.AddTeardownFailure(m_TestClassType, e.Message);
                }
            }
        }

        
        private Type m_TestClassType;
        private List<MethodInfo> m_TestMethods;
        private TestClassResult m_Results;
        object m_Instance;
    }
}