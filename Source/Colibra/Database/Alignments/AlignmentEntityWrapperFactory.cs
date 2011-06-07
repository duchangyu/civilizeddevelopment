// (C) Copyright 2011 Autodesk, Inc.  All rights reserved.
//
// Permission to use, copy, modify, and distribute these source code samples is
// hereby granted, provided that (i) you must clearly identify any modified 
// source code files and any resulting binary files as works developed by you,
// and not by Autodesk;  and (ii) you may distribute the resulting binary files
// of the source code samples in works that are commercially distributed 
// software applications only if:  (a) such applications require an Autodesk
// product to operate; and (b) such applications contain, subject to Autodesk's
// sole discretion, significant features and functionality in addition to the 
// source code samples so that the source code samples are not the primary
// source of value.  In any copy of the source code samples, derivative works,
// and resulting binary files, you must include the copyright notices of 
// Autodesk, Inc., the limited warranty and restricted rights notice below, and
// (if modified) the following statement: "This software contains copyrighted 
// code owned by Autodesk but has been modified and is not endorsed by Autodesk
// in its modified form".
//
// AUTODESK PROVIDES THIS SOFTWARE "AS IS" AND WITH ALL FAULTS.  AUTODESK MAKES
// NO WARRANTIES, EXPRESS OR IMPLIED, AS TO NONINFRINGEMENT OF THIRD PARTY
// RIGHTS, MERCHANTABILITY, OR FITNESS FOR ANY PARTICULAR PURPOSE. IN NO EVENT
// WILL AUTODESK BE LIABLE TO YOU FOR ANY CONSEQUENTIAL, INCIDENTAL OR SPECIAL
// DAMAGES, INCLUDING ANY LOST PROFITS OR LOST SAVINGS, EVEN IF AUTODESK HAS
// BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES, OR FOR ANY CLAIM BY ANY
// THIRD PARTY. AUTODESK DOES NOT WARRANT THAT THE OPERATION OF THE SOFTWARE
// WILL BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to 
// restrictions set forth in FAR 52.227-19 (Commercial ComputerSoftware -
// Restricted Rights) and DFAR 252.227-7013(c)(1)(ii) (Rights in Technical Data
// and Computer Software), as applicable.
//
// You may not export the source code samples or any derivative works, 
// resulting binaries, or any related technical documentation,  in violation of
// U.S. or other applicable export control laws.
//
using System;

using Autodesk.Civil.Land.DatabaseServices;

namespace Colibra
{
    internal class AlignmentEntityWrapperFactory
    {
        /// <summary>
        /// Creates an AAlignmentEntity wraping an alignment entity.
        /// </summary>
        /// <param name="entity">Entity to be wrapped.</param>
        /// <returns>A wrapper AAlignmentEntity object.</returns>
        public static AAlignmentEntity WrapEntity(AlignmentEntity entity)
        {
            AAlignmentEntity wrapped = CreateWrapper(entity.EntityType);
            wrapped.AssignEntity(entity);
            return wrapped;
        }

        /// <summary>
        /// Factory method that creates the correct wrapper for a specified
        /// entity type.
        /// </summary>
        /// <remarks>
        /// This method was provided to be able to test the correct wrapper
        /// class is created for all supported entity types. Users should
        /// not call this method directly because it returns an empty
        /// wrapper. Instead, users should call WrapEntity(), which
        /// initializes the wrapper after is created.
        /// </remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static AAlignmentEntity CreateWrapper(AlignmentEntityType type)
        {
            switch(type)
            {
                case AlignmentEntityType.Line:
                    return new AlignmentEntityLine();

                case AlignmentEntityType.Arc:
                    return new AlignmentEntityCurve();
                    
                case AlignmentEntityType.Spiral:
                    return new AlignmentEntitySpiral();

                case AlignmentEntityType.SpiralCurveSpiral:
                    return new AlignmentEntitySCS();

                case AlignmentEntityType.SpiralLineSpiral:
                    return new AlignmentEntitySLS();

                case AlignmentEntityType.SpiralLine:
                    return new AlignmentEntitySL();

                case AlignmentEntityType.LineSpiral:
                    return new AlignmentEntityLS();

                case AlignmentEntityType.SpiralCurve:
                    return new AlignmentEntitySC();

                case AlignmentEntityType.CurveSpiral:
                    return new AlignmentEntityCS();

                case AlignmentEntityType.SpiralSpiralCurveSpiralSpiral:
                    return new AlignmentEntitySSCSS();

                case AlignmentEntityType.SpiralCurveSpiralCurveSpiral:
                    return new AlignmentEntitySCSCS();

                case AlignmentEntityType.SpiralCurveSpiralSpiralCurveSpiral:
                    return new AlignmentEntitySCSSCS();

                case AlignmentEntityType.SpiralSpiral:
                    return new AlignmentEntitySS();

                case AlignmentEntityType.SpiralSpiralCurve:
                    return new AlignmentEntitySSC();

                case AlignmentEntityType.CurveSpiralSpiral:
                    return new AlignmentEntityCSS();

                // Not used for now.
                // case AlignmentEntityType.MultipleSegments:
                // 

                case AlignmentEntityType.CurveLineCurve:
                    return new AlignmentEntityCLC();

                case AlignmentEntityType.CurveReverseCurve:
                    return new AlignmentEntityCRC();

                case AlignmentEntityType.CurveCurveReverseCurve:
                    return new AlignmentEntityCCRC();

                default:
                    throw new NotImplementedException(
                        "Specified entity wrapper not implemented.");
            }
        }
    }
}