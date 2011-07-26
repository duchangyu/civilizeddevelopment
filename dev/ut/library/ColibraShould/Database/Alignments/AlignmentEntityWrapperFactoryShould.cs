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
using Autodesk.Civil.Land.DatabaseServices;

using Colibra;
using TinyTest;

namespace ColibraShould
{
    [TestClass]
    public class AlignmentEntityWrapperFactoryShould
    {
        [TestMethod]
        public void CreateCorrectLineEntity()
        {
            _given(AlignmentEntityType.Line);
            _assert(typeof(AlignmentEntityLine));
        }

        [TestMethod]
        public void CreateCorrectCurveEntity()
        {
            _given(AlignmentEntityType.Arc);
            _assert(typeof(AlignmentEntityCurve));
        }

        [TestMethod]
        public void CreateCorrectSpiralEntity()
        {
            _given(AlignmentEntityType.Spiral);
            _assert(typeof(AlignmentEntitySpiral));
        }

        [TestMethod]
        public void CreateCorrectSLSEntity()
        {
            _given(AlignmentEntityType.SpiralLineSpiral);
            _assert(typeof(AlignmentEntitySLS));
        }

        [TestMethod]
        public void CreateCorrectSLEntity()
        {
            _given(AlignmentEntityType.SpiralLine);
            _assert(typeof(AlignmentEntitySL));
        }

        [TestMethod]
        public void CreateCorrectLSEntity()
        {
            _given(AlignmentEntityType.LineSpiral);
            _assert(typeof(AlignmentEntityLS));
        }

        [TestMethod]
        public void CreateCorrectSCEntity()
        {
            _given(AlignmentEntityType.SpiralCurve);
            _assert(typeof(AlignmentEntitySC));
        }

        [TestMethod]
        public void CreateCorrectCSEntity()
        {
            _given(AlignmentEntityType.CurveSpiral);
            _assert(typeof(AlignmentEntityCS));
        }

        [TestMethod]
        public void CreateCorrectSSCSSEntity()
        {
            _given(AlignmentEntityType.SpiralSpiralCurveSpiralSpiral);
            _assert(typeof(AlignmentEntitySSCSS));
        }

        [TestMethod]
        public void CreateCorrectSCSCSEntity()
        {
            _given(AlignmentEntityType.SpiralCurveSpiralCurveSpiral);
            _assert(typeof(AlignmentEntitySCSCS));
        }

        [TestMethod]
        public void CreateCorrectSCSSCSEntity()
        {
            _given(AlignmentEntityType.SpiralCurveSpiralSpiralCurveSpiral);
            _assert(typeof(AlignmentEntitySCSSCS));
        }

        [TestMethod]
        public void CreateCorrectSSEntity()
        {
            _given(AlignmentEntityType.SpiralSpiral);
            _assert(typeof(AlignmentEntitySS));
        }

        [TestMethod]
        public void CreateCorrectSSCEntity()
        {
            _given(AlignmentEntityType.SpiralSpiralCurve);
            _assert(typeof(AlignmentEntitySSC));
        }

        [TestMethod]
        public void CreateCorrectCSSEntity()
        {
            _given(AlignmentEntityType.CurveSpiralSpiral);
            _assert(typeof(AlignmentEntityCSS));
        }

        [TestMethod]
        public void CreateCorrectCLCEntity()
        {
            _given(AlignmentEntityType.CurveLineCurve);
            _assert(typeof(AlignmentEntityCLC));
        }

        [TestMethod]
        public void CreateCorrectCRCEntity()
        {
            _given(AlignmentEntityType.CurveReverseCurve);
            _assert(typeof(AlignmentEntityCRC));
        }

        [TestMethod]
        public void CreateCorrectCCRCEntity()
        {
            _given(AlignmentEntityType.CurveCurveReverseCurve);
            _assert(typeof(AlignmentEntityCCRC));
        }

        [TestMethod]
        public void ThrowExceptionOnNotImplementedType()
        {
            try
            {
                AlignmentEntityWrapperFactory.CreateWrapper(
                    AlignmentEntityType.MultipleSegments);
            }
            catch (NotImplementedException exception)
            {
                // We expect it to throw.
                return;
            }
            Assert.Fail("Exception not thrown on not implemented entity.");
        }

        [TestMethod]
        public void WrapEntityCorrectly()
        {
            Document testDoc = getTestDocument();
            using (Transaction tr = testDoc.StartTransaction())
            {
                AlignmentEntity entity = getTestEntity(testDoc);
                AAlignmentEntity wrapper = AlignmentEntityWrapperFactory.WrapEntity(entity);

                Assert.IsTrue(wrapper.IsValid, "Entity not wrapped correctly.");
            }
        }

        private void _given(AlignmentEntityType entityType)
        {
            m_RequestedEntityType = entityType;
        }

        private void _assert(Type wrapperType)
        {
            AAlignmentEntity entity = 
                AlignmentEntityWrapperFactory.CreateWrapper(m_RequestedEntityType);
            Assert.AreEqual(wrapperType, entity.GetType(), 
                "Incorrect entity type created.");
        }

        private string _testDocumentName
        {
            get
            {
                return Path.Combine(AbsoluteLocation.DataDirectory, 
                    "TwoAlignments.dwg");
            }
        }

        private Document getTestDocument()
        {
            return DocumentManager.OpenDocument(_testDocumentName);
        }

        private AlignmentEntity getTestEntity(Document document)
        {
            ByNameObjectSelector<Alignment> selector = 
                new ByNameObjectSelector<Alignment>();
            selector.ObjectName = "Alignment - (1)";
            selector.Select(document);
            Alignment alignment = selector.SelectedId.GetObject(
                acaddb.OpenMode.ForRead) as Alignment;
            return alignment.Entities[0];
        }

        private AlignmentEntityType m_RequestedEntityType;
    }
}