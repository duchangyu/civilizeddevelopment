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
Imports System.Threading

Imports acadappsvcs = Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices

Namespace Colibra
    ''' <summary>
    ''' This class manages and provides access to Document objects.
    ''' </summary>
    Public Class DocumentManager
        ''' <summary>
        ''' Returns the active document.
        ''' </summary>
        Public Shared ReadOnly Property ActiveDocument() As Document
            Get
                If m_ActiveDocument Is Nothing Then
                    ' We were never called, so lets initialize the class to the
                    ' current active document in AutoCAD.
                    '

                    createNewAndActivateFromAutoCADDocument(acadappsvcs.Application.DocumentManager.MdiActiveDocument)
                End If
                Return m_ActiveDocument
            End Get
        End Property

        ''' <summary>
        ''' Opens a document from a DWG file name. Opening a DWG creates a new Document object
        ''' and activates it.
        ''' </summary>
        ''' <param name="fileName">DWG file to open.</param>
        ''' <returns>Returns the created Document object.</returns>
        Public Shared Function OpenDocument(fileName As String) As Document
            Dim acadDoc As acadappsvcs.Document = acadappsvcs.Application.DocumentManager.Open(fileName)
            createNewAndActivateFromAutoCADDocument(acadDoc)
            Return m_ActiveDocument
        End Function

        ''' <summary>
        ''' Activates the specified Document.
        ''' </summary>
        ''' <param name="doc">Document to be activated.</param>
        Friend Shared Sub _activateDocument(doc As Document)
            acadappsvcs.Application.DocumentManager.MdiActiveDocument = doc._acaddoc
            m_ActiveDocument = doc
        End Sub

        Private Shared Sub createNewAndActivateFromAutoCADDocument(acadDoc As acadappsvcs.Document)
            Dim civilDoc As CivilDocument = getCivilDocumentAndActivate(acadDoc)
            m_ActiveDocument = New Document(acadDoc, civilDoc)
        End Sub

        Private Shared Function getCivilDocumentAndActivate(acadDoc As acadappsvcs.Document) As CivilDocument
            acadappsvcs.Application.DocumentManager.MdiActiveDocument = acadDoc
            Return CivilApplication.ActiveDocument
        End Function

        Private Shared m_ActiveDocument As Document = Nothing
    End Class
End Namespace