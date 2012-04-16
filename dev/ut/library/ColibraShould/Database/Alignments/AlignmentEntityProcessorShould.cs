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
using System.Text;

using acaddb = Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

using Colibra;
using TinyTest;

namespace ColibraShould
{
    [TestClass]
    public class AlignmentEntityProcessorShould
    {
        [TestMethod]
        public void WriteInformationCorrectly()
        {
            Document doc = getTestDocument();
            using (Transaction tr = doc.StartTransaction())
            {
                ByNameObjectSelector<Alignment> selector = 
                    new ByNameObjectSelector<Alignment>();
                selector.ObjectName = "Alignment - (1)";
                selector.Select(doc);
                AlignmentEntityProcessor processor = 
                    new AlignmentEntityProcessor(selector.SelectedId);
                AlignmentEntityInfoWriterMock writer = 
                    new AlignmentEntityInfoWriterMock();
                processor.WriteInfo(writer);

                Assert.AreEqual(_expectedOutput, writer.Output, 
                    "Incorrect information written.");
            }
        }

        private string _expectedOutput
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Alignment - (1)");
                builder.Append(1);
                builder.Append("Autodesk.Civil.DatabaseServices.AlignmentLine");
                builder.Append(1);
                builder.Append("Line");
                builder.Append(2);
                builder.Append("Autodesk.Civil.DatabaseServices.AlignmentLine");
                builder.Append(1);
                builder.Append("Line");
                builder.Append(3);
                builder.Append("Autodesk.Civil.DatabaseServices.AlignmentLine");
                builder.Append(1);
                builder.Append("Line");
                builder.Append(4);
                builder.Append("Autodesk.Civil.DatabaseServices.AlignmentArc");
                builder.Append(1);
                builder.Append("Curve");
                builder.Append(5);
                builder.Append("Autodesk.Civil.DatabaseServices.AlignmentArc");
                builder.Append(1);
                builder.Append("Curve");
                return builder.ToString();
            }
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
    }
}