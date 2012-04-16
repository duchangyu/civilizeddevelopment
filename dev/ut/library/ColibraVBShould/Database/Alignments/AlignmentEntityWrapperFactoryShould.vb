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

Imports acaddb = Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Civil.DatabaseServices

Imports Colibra
Imports TinyTest

Namespace ColibraShould
    <TestClass()> _
    Public Class AlignmentEntityWrapperFactoryShould
        <TestMethod()> _
        Public Sub CreateCorrectLineEntity()
            _given(AlignmentEntityType.Line)
            _assert(GetType(AlignmentEntityLine))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectCurveEntity()
            _given(AlignmentEntityType.Arc)
            _assert(GetType(AlignmentEntityCurve))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectSpiralEntity()
            _given(AlignmentEntityType.Spiral)
            _assert(GetType(AlignmentEntitySpiral))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectSLSEntity()
            _given(AlignmentEntityType.SpiralLineSpiral)
            _assert(GetType(AlignmentEntitySLS))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectSLEntity()
            _given(AlignmentEntityType.SpiralLine)
            _assert(GetType(AlignmentEntitySL))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectLSEntity()
            _given(AlignmentEntityType.LineSpiral)
            _assert(GetType(AlignmentEntityLS))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectSCEntity()
            _given(AlignmentEntityType.SpiralCurve)
            _assert(GetType(AlignmentEntitySC))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectCSEntity()
            _given(AlignmentEntityType.CurveSpiral)
            _assert(GetType(AlignmentEntityCS))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectSSCSSEntity()
            _given(AlignmentEntityType.SpiralSpiralCurveSpiralSpiral)
            _assert(GetType(AlignmentEntitySSCSS))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectSCSCSEntity()
            _given(AlignmentEntityType.SpiralCurveSpiralCurveSpiral)
            _assert(GetType(AlignmentEntitySCSCS))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectSCSSCSEntity()
            _given(AlignmentEntityType.SpiralCurveSpiralSpiralCurveSpiral)
            _assert(GetType(AlignmentEntitySCSSCS))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectSSEntity()
            _given(AlignmentEntityType.SpiralSpiral)
            _assert(GetType(AlignmentEntitySS))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectSSCEntity()
            _given(AlignmentEntityType.SpiralSpiralCurve)
            _assert(GetType(AlignmentEntitySSC))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectCSSEntity()
            _given(AlignmentEntityType.CurveSpiralSpiral)
            _assert(GetType(AlignmentEntityCSS))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectCLCEntity()
            _given(AlignmentEntityType.CurveLineCurve)
            _assert(GetType(AlignmentEntityCLC))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectCRCEntity()
            _given(AlignmentEntityType.CurveReverseCurve)
            _assert(GetType(AlignmentEntityCRC))
        End Sub

        <TestMethod()> _
        Public Sub CreateCorrectCCRCEntity()
            _given(AlignmentEntityType.CurveCurveReverseCurve)
            _assert(GetType(AlignmentEntityCCRC))
        End Sub

        <TestMethod()> _
        Public Sub ThrowExceptionOnNotImplementedType()
            Try
                AlignmentEntityWrapperFactory.CreateWrapper(AlignmentEntityType.MultipleSegments)
            Catch exception As NotImplementedException
                ' We expect it to throw.
                Return
            End Try
            Assert.Fail("Exception not thrown on not implemented entity.")
        End Sub

        <TestMethod()> _
        Public Sub WrapEntityCorrectly()
            Dim testDoc As Document = getTestDocument()
            Using tr As Transaction = testDoc.StartTransaction()
                Dim entity As AlignmentEntity = getTestEntity(testDoc)
                Dim wrapper As AAlignmentEntity = AlignmentEntityWrapperFactory.WrapEntity(entity)

                Assert.IsTrue(wrapper.IsValid, "Entity not wrapped correctly.")
            End Using
        End Sub

        Private Sub _given(entityType As AlignmentEntityType)
            m_RequestedEntityType = entityType
        End Sub

        Private Sub _assert(wrapperType As Type)
            Dim entity As AAlignmentEntity = AlignmentEntityWrapperFactory.CreateWrapper(m_RequestedEntityType)
            Assert.AreEqual(wrapperType, entity.[GetType](), "Incorrect entity type created.")
        End Sub

        Private ReadOnly Property _testDocumentName() As String
            Get
                Return Path.Combine(AbsoluteLocation.DataDirectory, "TwoAlignments.dwg")
            End Get
        End Property

        Private Function getTestDocument() As Document
            Return DocumentManager.OpenDocument(_testDocumentName)
        End Function

        Private Function getTestEntity(document As Document) As AlignmentEntity
            Dim selector As New ByNameObjectSelector(Of Alignment)()
            selector.ObjectName = "Alignment - (1)"
            selector.[Select](document)
            Dim alignment As Alignment = TryCast(selector.SelectedId.GetObject(acaddb.OpenMode.ForRead), Alignment)
            Return alignment.Entities(0)
        End Function

        Private m_RequestedEntityType As AlignmentEntityType
    End Class
End Namespace