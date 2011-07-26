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

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput

Namespace Colibra
    ''' <summary>
    ''' Allows selecting a single object from a document through a pick.
    ''' </summary>
    ''' <typeparam name="T">Type of object to select.</typeparam>
    Public Class SingleObjectSelector(Of T)
        Implements IObjectSelector
        Public Sub New()
            SelectedId = ObjectId.Null
            PromptMessage = "Select object: "
            RejectMessage = "Object type does not match."
        End Sub
        ''' <summary>
        ''' Returns the object id of the selected object.
        ''' </summary>
        Public Property SelectedId() As ObjectId
            Get
                Return m_SelectedId
            End Get
            Friend Set(value As ObjectId)
                m_SelectedId = Value
            End Set
        End Property
        Private m_SelectedId As ObjectId

        ''' <summary>
        ''' Get or set the message to prompt the user. If not specified,
        ''' the default message is "Select object: ".
        ''' </summary>
        Public Property PromptMessage() As String
            Get
                Return m_PromptMessage
            End Get
            Set(value As String)
                m_PromptMessage = Value
            End Set
        End Property
        Private m_PromptMessage As String

        ''' <summary>
        ''' Get or set the reject message when object type does not match.
        ''' If not specified, the default is "Object type does not match.".
        ''' </summary>
        Public Property RejectMessage() As String
            Get
                Return m_RejectMessage
            End Get
            Set(value As String)
                m_RejectMessage = Value
            End Set
        End Property
        Private m_RejectMessage As String

        ''' <summary>
        ''' Selects a document from the specified object by prompting the
        ''' user to select it with a pick.
        ''' </summary>
        ''' <param name="document">Document from which to select the object.</param>
        ''' <returns>True if a valid object is selected or false otherwise.</returns>
        Public Function [Select](document As Document) As Boolean Implements IObjectSelector.Select
            SelectedId = ObjectId.Null
            Dim editor As Editor = document._acaddoc.Editor
            Dim options As New PromptEntityOptions(PromptMessage)
            options.SetRejectMessage(RejectMessage)
            options.AddAllowedClass(GetType(T), True)
            Dim result As PromptEntityResult = editor.GetEntity(options)
            If result.Status = PromptStatus.OK Then
                SelectedId = result.ObjectId
                Return True
            End If
            Return False
        End Function
    End Class
End Namespace