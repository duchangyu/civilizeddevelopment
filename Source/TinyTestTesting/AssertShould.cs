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

using TinyTest;

namespace TinyTestTesting
{
    /// <summary>
    /// Unit test class that validates the implementation of the
    /// Assert class in the TinyTest framework.
    /// </summary>
    [TestClass]
    public class AssertShould
    {
        [TestMethod]
        public void FailsWhenFailMethodIsInvoked()
        {
            try
            {
                Assert.Fail("Should FAIL");
            }
            catch (AssertionFailureException e)
            {
                // We expect the assertion to fail.
                return;
            }
            throw new AssertionFailureException("Assert.Fail did not fail.");
        }

        [TestMethod]
        public void PassIsTrueTestWhenExpressionIsTrue()
        {
            Assert.IsTrue(true, "Should PASS");
        }

        [TestMethod]
        public void FailIsTrueTestWhenExpressionIsFalse()
        {
            try 
            {
                Assert.IsTrue(false, "Should FAIL");
            }
            catch(AssertionFailureException e)
            {
                // We expect the assertion to fail.
                return;
            }
            throw new AssertionFailureException("Assert.IsTrue did not fail with false expression.");
        }

        [TestMethod]
        public void PassIsFalseWhenExpressionIsFalse()
        {
            Assert.IsFalse(false, "Should PASS");
        }

        [TestMethod]
        public void FailIsFalseWhenExpressionIsTrue()
        {
            try
            {
                Assert.IsFalse(true, "Should FAIL");
            }
            catch (AssertionFailureException e)
            {
            	// We expect the assertion to fail.
                return;
            }
            throw new AssertionFailureException("Assert.IsFalise did not fail with true expression.");
        }

        [TestMethod]
        public void PassAreEqualIfIntegersAreEqual()
        {
            Assert.AreEqual(1, 1, "Should PASS");
        }

        [TestMethod]
        public void FailAreEqualIfIntegersAreNotEqual()
        {
            try
            {
                Assert.AreEqual(1, 2, "Should FAIL");
            }
            catch (AssertionFailureException e)
            {
            	// We expect the assertion to fail.
                return;
            }
            throw new AssertionFailureException("Assert.AreEqual did not fail with different integers.");
        }

        [TestMethod]
        public void PassAreEqualIfDoublesAreEqual()
        {
            Assert.AreEqual(2.0, 2.0, "Should PASS");
        }

        [TestMethod]
        public void FailAreEqualIfDoublesAreNotEqual()
        {
            try
            {
                Assert.AreEqual(2.0, 2.05, "Should FAIL");
            }
            catch (AssertionFailureException e)
            {
            	// We expect the assertion to fail.
                return;
            }
            throw new AssertionFailureException("Assert.AreEqual did not fail with different doubles.");
        }

        [TestMethod]
        public void PassAreEqualIfDoublesWithinTolerance()
        {
            Assert.AreEqual(2.5, 2.50001, 0.0001, "Should PASS");
        }

        [TestMethod]
        public void FailAreEqualIfDoublesNotWithinTolerance()
        {
            try 
            {
                Assert.AreEqual(2.5, 2.50001, 0.000001, "Should FAIL");
            }
            catch (AssertionFailureException e)
            {
                // We expect the assertion to fail.
                return;
            }
            throw new AssertionFailureException("Assert.AreEqual did not fail with different doubles over tolerance.");
        }

        [TestMethod]
        public void PassAreEqualIfStringsAreEqual()
        {
            Assert.AreEqual("Foo", "Foo", "Should PASS");
        }

        [TestMethod]
        public void FailAreEqualIfStringsAreNotEqual()
        {
            try
            {
                Assert.AreEqual("Foo", "Bar", "Should FAIL");
            }
            catch (AssertionFailureException e)
            {
            	// We expect the assertion to fail.
                return;
            }
            throw new AssertionFailureException("Assert.AreEqual did not fail with different strings.");
        }

        [TestMethod]
        public void PassAreEqualIfTypesAreEqual()
        {
            Assert.AreEqual(this.GetType(), this.GetType(), "Should PASS");
        }

        [TestMethod]
        public void FailAreEqualIfTypesAreNotEqual()
        {
            try
            {
                Assert.AreEqual(this.GetType(), typeof(System.String), "Should FAIL");
            }
            catch
            {
                // We expect the assertion to fail.
                return;
            }
            throw new AssertionFailureException("Assert.AreEqual did not fail with different types.");
        }
    }
}