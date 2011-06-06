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

using acaddb = Autodesk.AutoCAD.DatabaseServices;
using c3dlanddb = Autodesk.Civil.Land.DatabaseServices;
using c3ddb = Autodesk.Civil.DatabaseServices;

using Colibra;
using TinyTest;

namespace ColibraShould
{
    [TestClass]
    public class ObjectNodeProviderShould
    {
        public ObjectNodeProviderShould()
        {
            m_TestDocument = DocumentManager.OpenDocument(_testDocumentName);
        }

        [TestMethod]
        public void ReturnTheCorrectObjectIdCollection()
        {
            using (Transaction tr = m_TestDocument.StartTransaction())
            {
                Type wantedType = typeof(c3dlanddb.Alignment);
                _givenNodeType(wantedType);
                _verifyObjectTypeIs(wantedType);
            }
        }

        [TestMethod]
        public void ThrowExceptionIfNodeTypeNotRegistered()
        {
            ObjectNodeProvider provider = m_TestDocument.NodeProvider;
            try
            {
                provider.GetNode(typeof(String));
            }
            catch (NotImplementedException exception)
            {
                // Expected to throw.
                return;
            }
            Assert.Fail("Exception not thrown un unregistered type.");
        }

        private void _givenNodeType(Type nodeType)
        {
            m_RequestedType = nodeType;
        }

        private void _verifyObjectTypeIs(Type requestedType)
        {
            acaddb.ObjectIdCollection node = 
                m_TestDocument.NodeProvider.GetNode(m_RequestedType);
            acaddb.ObjectId id = node[0];
            c3ddb.Entity entity = id.GetObject(acaddb.OpenMode.ForRead) 
                as c3ddb.Entity;
            Assert.AreEqual(requestedType, entity.GetType(), 
                "Incorrect node returned.");
        }

        private string _testDocumentName
        {
            get
            {
                return Path.Combine(AbsoluteLocation.DataDirectory, "OneEach.dwg");
            }
        }

        private Document m_TestDocument;
        private Type m_RequestedType;
    }
}