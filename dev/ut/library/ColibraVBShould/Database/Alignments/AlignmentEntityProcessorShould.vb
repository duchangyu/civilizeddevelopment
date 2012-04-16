' (C) Copyright 2011 Autodesk, Inc.  All rights reserved.
'
' Permission to use, copy, modify, and distribute these source code samples is
' hereby granted, provided that (i) you must clearly identify any modified 
' source code files and any resulting binary files as works developed by you,
' and not by Autodesk;  and (ii) you may distribute the resulting binary files
' of the source code samples in works that are commercially distributed 
' software applications only if:  (a) such applications require an Autodesk
' product to operate; and (b) such applications contain, subject to Autodesk's
' sole discretion, significant features and functionality in addition to the 
' source code samples so that the source code samples are not the primary
' source of value.  In any copy of the source code samples, derivative works,
' and resulting binary files, you must include the copyright notices of 
' Autodesk, Inc., the limited warranty and restricted rights notice below, and
' (if modified) the following statement: "This software contains copyrighted 
' code owned by Autodesk but has been modified and is not endorsed by Autodesk
' in its modified form".
'
' AUTODESK PROVIDES THIS SOFTWARE "AS IS" AND WITH ALL FAULTS.  AUTODESK MAKES
' NO WARRANTIES, EXPRESS OR IMPLIED, AS TO NONINFRINGEMENT OF THIRD PARTY
' RIGHTS, MERCHANTABILITY, OR FITNESS FOR ANY PARTICULAR PURPOSE. IN NO EVENT
' WILL AUTODESK BE LIABLE TO YOU FOR ANY CONSEQUENTIAL, INCIDENTAL OR SPECIAL
' DAMAGES, INCLUDING ANY LOST PROFITS OR LOST SAVINGS, EVEN IF AUTODESK HAS
' BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES, OR FOR ANY CLAIM BY ANY
' THIRD PARTY. AUTODESK DOES NOT WARRANT THAT THE OPERATION OF THE SOFTWARE
' WILL BE UNINTERRUPTED OR ERROR FREE.
'
' Use, duplication, or disclosure by the U.S. Government is subject to 
' restrictions set forth in FAR 52.227-19 (Commercial ComputerSoftware -
' Restricted Rights) and DFAR 252.227-7013(c)(1)(ii) (Rights in Technical Data
' and Computer Software), as applicable.
'
' You may not export the source code samples or any derivative works, 
' resulting binaries, or any related technical documentation,  in violation of
' U.S. or other applicable export control laws.
'
Imports System.IO
Imports System.Text

Imports acaddb = Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Civil.DatabaseServices

Imports Colibra
Imports TinyTest

Namespace ColibraShould
  <TestClass()> _
  Public Class AlignmentEntityProcessorShould
    <TestMethod()> _
    Public Sub WriteInformationCorrectly()
      Dim doc As Document = getTestDocument()
      Using tr As Transaction = doc.StartTransaction()
        Dim selector As New ByNameObjectSelector(Of Alignment)()
        selector.ObjectName = "Alignment - (1)"
        selector.[Select](doc)
        Dim processor As New AlignmentEntityProcessor(selector.SelectedId)
        Dim writer As New AlignmentEntityInfoWriterMock()
        processor.WriteInfo(writer)

        Assert.AreEqual(_expectedOutput, writer.Output, "Incorrect information written.")
      End Using
    End Sub

    Private ReadOnly Property _expectedOutput() As String
      Get
        Dim builder As New StringBuilder()
        builder.Append("Alignment - (1)")
        builder.Append(1)
        builder.Append("Autodesk.Civil.DatabaseServices.AlignmentLine")
        builder.Append(1)
        builder.Append("Line")
        builder.Append(2)
        builder.Append("Autodesk.Civil.DatabaseServices.AlignmentLine")
        builder.Append(1)
        builder.Append("Line")
        builder.Append(3)
        builder.Append("Autodesk.Civil.DatabaseServices.AlignmentLine")
        builder.Append(1)
        builder.Append("Line")
        builder.Append(4)
        builder.Append("Autodesk.Civil.DatabaseServices.AlignmentArc")
        builder.Append(1)
        builder.Append("Curve")
        builder.Append(5)
        builder.Append("Autodesk.Civil.DatabaseServices.AlignmentArc")
        builder.Append(1)
        builder.Append("Curve")
        Return builder.ToString()
      End Get
    End Property

    Private ReadOnly Property _testDocumentName() As String
      Get
        Return Path.Combine(AbsoluteLocation.DataDirectory, "TwoAlignments.dwg")
      End Get
    End Property

    Private Function getTestDocument() As Document
      Return DocumentManager.OpenDocument(_testDocumentName)
    End Function
  End Class
End Namespace