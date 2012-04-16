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
Imports System.Collections.Generic

Imports Autodesk.Civil.DatabaseServices

Namespace Colibra
    ''' <summary>
    ''' Enumeration policy that enumerates the alignment entities by
    ''' order of ID.
    ''' </summary>
    Public Class ByEntityIdEnumerationPolicy
        Implements IAlignmentEntityEnumerationPolicy
        ''' <summary>
        ''' Initializes the policy object with the current entity collection
        ''' for the alignment.
        ''' </summary>
        ''' <param name="entities">Entities to enunmerate.</param>
        Public Sub Initialize(entities As AlignmentEntityCollection) _
            Implements IAlignmentEntityEnumerationPolicy.Initialize
            m_TheEntities = entities
        End Sub

        ''' <summary>
        ''' Returns an alignment entity enumerator.
        ''' </summary>
        ''' <returns>Returns the enumerator object.</returns>
        Public Function GetEnumerator() As IEnumerator(Of AAlignmentEntity) _
            Implements IAlignmentEntityEnumerationPolicy.GetEnumerator
            Return New ByEntityIdEnumerator(m_TheEntities)
        End Function

        ''' <summary>
        ''' Returns an alignment entity enumerator.
        ''' </summary>
        ''' <returns>Returns the enumerator object.</returns>
        Private Function System_Collections_IEnumerable_GetEnumerator() _
            As System.Collections.IEnumerator _
            Implements System.Collections.IEnumerable.GetEnumerator
            Return New ByEntityIdEnumerator(m_TheEntities)
        End Function

        Private m_TheEntities As AlignmentEntityCollection
    End Class


    ''' <summary>
    ''' Enumerator object that enumerates entities by ID order.
    ''' </summary>
    Public Class ByEntityIdEnumerator
        Implements IEnumerator(Of AAlignmentEntity)
        ''' <summary>
        ''' Initialzies the enumerator using the entity collection.
        ''' </summary>
        ''' <param name="entities">Alignment entity collection to enumerate.
        ''' </param>
        Friend Sub New(entities As AlignmentEntityCollection)
            m_TheEntities = entities
            m_Enumerator = m_TheEntities.GetEnumerator()
        End Sub

        ''' <summary>
        ''' Returns the current entity.
        ''' </summary>
        Public ReadOnly Property Current() As AAlignmentEntity _
            Implements IEnumerator(Of AAlignmentEntity).Current
            Get
                Return AlignmentEntityWrapperFactory.WrapEntity( _
                    m_Enumerator.Current)
            End Get
        End Property

        ''' <summary>
        ''' Disposes the object.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            m_Enumerator.Dispose()
            m_TheEntities.Dispose()
        End Sub

        ''' <summary>
        ''' Returns the current entity as an object.
        ''' </summary>
        Private ReadOnly Property System_Collections_IEnumerator_Current() _
            As Object Implements System.Collections.IEnumerator.Current
            Get
                Return AlignmentEntityWrapperFactory.WrapEntity( _
                    m_Enumerator.Current)
            End Get
        End Property

        ''' <summary>
        ''' Moves to the next entity.
        ''' </summary>
        ''' <returns>Returns true if there is an entity left or false
        ''' otherwise.</returns>
        Public Function MoveNext() As Boolean _
            Implements System.Collections.IEnumerator.MoveNext
            Return m_Enumerator.MoveNext()
        End Function

        ''' <summary>
        ''' Resets the enumerator.
        ''' </summary>
        Public Sub Reset() Implements System.Collections.IEnumerator.Reset
            m_Enumerator.Reset()
        End Sub

        Private m_TheEntities As AlignmentEntityCollection
        Private m_Enumerator As IEnumerator(Of AlignmentEntity)

    End Class
End Namespace