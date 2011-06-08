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

Imports Autodesk.Civil.Land.DatabaseServices

Namespace Colibra
    ''' <summary>
    ''' Abstract base class for all alignment entities.
    ''' </summary>
    ''' <para>
    ''' This class serves as a wrapper for alignment entity objects.
    ''' The class it self is abstract so it cannot be instantiated,
    ''' but sub classes must provide the wrapped entity, which will
    ''' be managed in this class.
    ''' </para>
    Public MustInherit Class AAlignmentEntity
        Friend Sub New()
            m_TheEntity = Nothing
        End Sub

        ''' <summary>
        ''' Returns whether the wrapper entity is valid (has been assigned).
        ''' </summary>
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return m_TheEntity IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Writes the alignment entity information to the specified writer.
        ''' </summary>
        ''' <param name="writer">Writer to which write the inforamtion.</param>
        Public Sub WriteInfo(writer As IAlignmentEntityInfoWriter)
            writeCommonInfo(writer)
            WriteCustomInfo(writer)
            writer.EntityInfoDone()
        End Sub

        ''' <summary>
        ''' Assigns the wrapped entity.
        ''' </summary>
        ''' <param name="entity">Entity to wrap.</param>
        Friend Sub AssignEntity(entity As AlignmentEntity)
            m_TheEntity = entity
        End Sub

        ''' <summary>
        ''' This method must be implemented in derived classes to write
        ''' the custom entity information.
        ''' </summary>
        ''' <param name="writer"></param>
        Protected MustOverride Sub WriteCustomInfo( _
            writer As IAlignmentEntityInfoWriter)

        Private Sub writeCommonInfo(writer As IAlignmentEntityInfoWriter)
            writer.WriteEntityId(m_TheEntity.EntityId)
            writer.WriteWrappedEntityClassType(m_TheEntity.[GetType]())
            writer.WriteSubEntityCount(m_TheEntity.SubEntityCount)
        End Sub

        Private m_TheEntity As AlignmentEntity
    End Class
End Namespace