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

Namespace Colibra
    Public Interface IAlignmentEntityInfoWriter
        ''' <summary>
        ''' Implement to write the alignment name.
        ''' </summary>
        ''' <param name="name">Name of the alignment.</param>
        Sub WriteAlignmentName(name As String)

        ''' <summary>
        ''' Implement to write the entity id.
        ''' </summary>
        ''' <param name="id">Id of the entity.</param>
        Sub WriteEntityId(id As Integer)

        ''' <summary>
        ''' Implement to write the class type of the wrapped entity.
        ''' </summary>
        ''' <param name="classType">Type of the wrapped entity.</param>
        Sub WriteWrappedEntityClassType(classType As Type)

        ''' <summary>
        ''' Implement to write the sub-entity count.
        ''' </summary>
        ''' <param name="count">Count of sub-entities.</param>
        Sub WriteSubEntityCount(count As Integer)

        ''' <summary>
        ''' Implement to write the curve group name.
        ''' </summary>
        ''' <param name="name">Name of the curve group.</param>
        Sub WriteCurveGroupName(name As String)

        ''' <summary>
        ''' Implement to terminate writing information about an entity.
        ''' </summary>
        ''' <remarks>
        ''' The entities write the information in a specified order; however,
        ''' writers may decide to modify the order of the information due to its
        ''' requirements. In this case, the writer may cache the information, and
        ''' reorder the output when this method is called, which means all the
        ''' information from an entity has been provided.
        ''' </remarks>
        Sub EntityInfoDone()
    End Interface
End Namespace