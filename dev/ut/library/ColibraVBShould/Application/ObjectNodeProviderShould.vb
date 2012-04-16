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
Imports c3dlanddb = Autodesk.Civil.DatabaseServices
Imports c3ddb = Autodesk.Civil.DatabaseServices

Imports Colibra
Imports TinyTest

Namespace ColibraShould
    <TestClass()> _
    Public Class ObjectNodeProviderShould
        Public Sub New()
            m_TestDocument = DocumentManager.OpenDocument(_testDocumentName)
        End Sub

        <TestMethod()> _
        Public Sub ReturnTheCorrectObjectIdCollection()
            Using tr As Transaction = m_TestDocument.StartTransaction()
                Dim wantedType As Type = GetType(c3dlanddb.Alignment)
                _givenNodeType(wantedType)
                _verifyObjectTypeIs(wantedType)
            End Using
        End Sub

        <TestMethod()> _
        Public Sub ThrowExceptionIfNodeTypeNotRegistered()
            Dim provider As ObjectNodeProvider = m_TestDocument.NodeProvider
            Try
                provider.GetNode(GetType([String]))
            Catch exception As NotImplementedException
                ' Expected to throw.
                Return
            End Try
            Assert.Fail("Exception not thrown un unregistered type.")
        End Sub

        Private Sub _givenNodeType(nodeType As Type)
            m_RequestedType = nodeType
        End Sub

        Private Sub _verifyObjectTypeIs(requestedType As Type)
            Dim node As acaddb.ObjectIdCollection = m_TestDocument.NodeProvider.GetNode(m_RequestedType)
            Dim id As acaddb.ObjectId = node(0)
            Dim entity As c3ddb.Entity = TryCast(id.GetObject(acaddb.OpenMode.ForRead), c3ddb.Entity)
            Assert.AreEqual(requestedType, entity.[GetType](), "Incorrect node returned.")
        End Sub

        Private ReadOnly Property _testDocumentName() As String
            Get
                Return Path.Combine(AbsoluteLocation.DataDirectory, "OneEach.dwg")
            End Get
        End Property

        Private m_TestDocument As Document
        Private m_RequestedType As Type
    End Class
End Namespace