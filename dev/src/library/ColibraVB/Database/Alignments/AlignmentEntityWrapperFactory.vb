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

Imports Autodesk.Civil.DatabaseServices

Namespace Colibra
    Friend Class AlignmentEntityWrapperFactory
        ''' <summary>
        ''' Creates an AAlignmentEntity wraping an alignment entity.
        ''' </summary>
        ''' <param name="entity">Entity to be wrapped.</param>
        ''' <returns>A wrapper AAlignmentEntity object.</returns>
        Public Shared Function WrapEntity(entity As AlignmentEntity) As AAlignmentEntity
            Dim wrapped As AAlignmentEntity = CreateWrapper(entity.EntityType)
            wrapped.AssignEntity(entity)
            Return wrapped
        End Function

        ''' <summary>
        ''' Factory method that creates the correct wrapper for a specified
        ''' entity type.
        ''' </summary>
        ''' <remarks>
        ''' This method was provided to be able to test the correct wrapper
        ''' class is created for all supported entity types. Users should
        ''' not call this method directly because it returns an empty
        ''' wrapper. Instead, users should call WrapEntity(), which
        ''' initializes the wrapper after is created.
        ''' </remarks>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Friend Shared Function CreateWrapper(type As AlignmentEntityType) As AAlignmentEntity
            Select Case type
                Case AlignmentEntityType.Line
                    Return New AlignmentEntityLine()

                Case AlignmentEntityType.Arc
                    Return New AlignmentEntityCurve()

                Case AlignmentEntityType.Spiral
                    Return New AlignmentEntitySpiral()

                Case AlignmentEntityType.SpiralCurveSpiral
                    Return New AlignmentEntitySCS()

                Case AlignmentEntityType.SpiralLineSpiral
                    Return New AlignmentEntitySLS()

                Case AlignmentEntityType.SpiralLine
                    Return New AlignmentEntitySL()

                Case AlignmentEntityType.LineSpiral
                    Return New AlignmentEntityLS()

                Case AlignmentEntityType.SpiralCurve
                    Return New AlignmentEntitySC()

                Case AlignmentEntityType.CurveSpiral
                    Return New AlignmentEntityCS()

                Case AlignmentEntityType.SpiralSpiralCurveSpiralSpiral
                    Return New AlignmentEntitySSCSS()

                Case AlignmentEntityType.SpiralCurveSpiralCurveSpiral
                    Return New AlignmentEntitySCSCS()

                Case AlignmentEntityType.SpiralCurveSpiralSpiralCurveSpiral
                    Return New AlignmentEntitySCSSCS()

                Case AlignmentEntityType.SpiralSpiral
                    Return New AlignmentEntitySS()

                Case AlignmentEntityType.SpiralSpiralCurve
                    Return New AlignmentEntitySSC()

                Case AlignmentEntityType.CurveSpiralSpiral
                    Return New AlignmentEntityCSS()

                    ' Not used for now.
                    ' case AlignmentEntityType.MultipleSegments:
                    ' 

                Case AlignmentEntityType.CurveLineCurve
                    Return New AlignmentEntityCLC()

                Case AlignmentEntityType.CurveReverseCurve
                    Return New AlignmentEntityCRC()

                Case AlignmentEntityType.CurveCurveReverseCurve
                    Return New AlignmentEntityCCRC()
                Case Else

                    Throw New NotImplementedException("Specified entity wrapper not implemented.")
            End Select
        End Function
    End Class
End Namespace