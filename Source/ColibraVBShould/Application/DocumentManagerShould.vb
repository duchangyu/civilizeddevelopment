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

Imports Colibra
Imports TinyTest

Namespace ColibraShould
    <TestClass()> _
    Public Class DocumentManagerShould
        Public Sub New()
            m_FirstOpenedDocument = DocumentManager.OpenDocument(_FirstOpenedName)
            m_SecondOpenedDocument = DocumentManager.OpenDocument(_SecondOpenedName)
        End Sub

        <TestMethod()> _
        Public Sub ReturnTheCorrectActiveDocument()
            m_FirstOpenedDocument.Activate()
            Dim active As Document = DocumentManager.ActiveDocument

            Assert.AreEqual(_FirstOpenedName, active.Name, "Incorrect active document returned.")
        End Sub

        <TestMethod()> _
        Public Sub OpenDocumentByName()
            Assert.AreEqual(_FirstOpenedName, m_FirstOpenedDocument.Name, "The opened document is not correct.")
        End Sub

        Private ReadOnly Property _FirstOpenedName() As String
            Get
                Return Path.Combine(AbsoluteLocation.DataDirectory, k_FirstTestDocumentName)
            End Get
        End Property

        Private ReadOnly Property _SecondOpenedName() As String
            Get
                Return Path.Combine(AbsoluteLocation.DataDirectory, k_SecondTestDocumentName)
            End Get
        End Property

        Private Const k_FirstTestDocumentName As String = "first_opened.dwg"
        Private m_FirstOpenedDocument As Document
        Private Const k_SecondTestDocumentName As String = "second_opened.dwg"
        Private m_SecondOpenedDocument As Document
    End Class
End Namespace