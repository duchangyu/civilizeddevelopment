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
using System.Collections.Generic;

using c3ddb = Autodesk.Civil.Land.DatabaseServices;

namespace Colibra
{
    /// <summary>
    /// 
    /// </summary>
    public class AlignmentEntityList : IEnumerable<AAlignmentEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alignment"></param>
        internal AlignmentEntityList(c3ddb.AlignmentEntityCollection entities)
        {
            m_Entities = entities;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<AAlignmentEntity> GetEnumerator()
        {
            return new AlignmentEntityEnumerator(m_Entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new AlignmentEntityEnumerator(m_Entities);
        }

        private c3ddb.AlignmentEntityCollection m_Entities;
    }

    /// <summary>
    /// 
    /// </summary>
    public class AlignmentEntityEnumerator : IEnumerator<AAlignmentEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alignment"></param>
        internal AlignmentEntityEnumerator(c3ddb.AlignmentEntityCollection entities)
        {
            m_Entities = entities;
            m_CurrentEntityIndex = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public AAlignmentEntity Current
        {
            get { return getCurrentEntity(); }
        }

        /// <summary>
        /// 
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return getCurrentEntity(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            m_CurrentEntityIndex = -1;
            m_Entities = null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            m_CurrentEntityIndex++;
            return m_CurrentEntityIndex < m_Entities.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            m_CurrentEntityIndex = -1;
        }

        private AAlignmentEntity getCurrentEntity()
        {
            c3ddb.AlignmentEntity entity = m_Entities.GetEntityByOrder(m_CurrentEntityIndex);
            return wrapEntity(entity);
        }

        private AAlignmentEntity wrapEntity(c3ddb.AlignmentEntity entity)
        {
            AAlignmentEntity wrappedEntity;
            switch(entity.EntityType)
            {
                case c3ddb.AlignmentEntityType.Line:
                    wrappedEntity = new AlignmentEntityLine();
                    break;

                case c3ddb.AlignmentEntityType.Arc:
                    wrappedEntity = new AlignmentEntityCurve();
                    break;

                case c3ddb.AlignmentEntityType.Spiral:
                    wrappedEntity = new AlignmentEntitySpiral();
                    break;

                case c3ddb.AlignmentEntityType.SpiralCurveSpiral:
                    wrappedEntity = new AlignmentEntitySCS();
                    break;

                case c3ddb.AlignmentEntityType.SpiralLineSpiral:
                    wrappedEntity = new AlignmentEntitySLS();
                    break;

                case c3ddb.AlignmentEntityType.SpiralLine:
                    wrappedEntity = new AlignmentEntitySL();
                    break;

                case c3ddb.AlignmentEntityType.LineSpiral:
                    wrappedEntity = new AlignmentEntityLS();
                    break;

                case c3ddb.AlignmentEntityType.SpiralCurve:
                    wrappedEntity = new AlignmentEntitySC();
                    break;

                case c3ddb.AlignmentEntityType.CurveSpiral:
                    wrappedEntity = new AlignmentEntityCS();
                    break;

                case c3ddb.AlignmentEntityType.SpiralSpiralCurveSpiralSpiral:
                    wrappedEntity = new AlignmentEntitySSCSS();
                    break;

                case c3ddb.AlignmentEntityType.SpiralCurveSpiralCurveSpiral:
                    wrappedEntity = new AlignmentEntitySCSCS();
                    break;

                case c3ddb.AlignmentEntityType.SpiralCurveSpiralSpiralCurveSpiral:
                    wrappedEntity = new AlignmentEntitySCSSCS();
                    break;

                case c3ddb.AlignmentEntityType.SpiralSpiral:
                    wrappedEntity = new AlignmentEntitySS();
                    break;

                case c3ddb.AlignmentEntityType.SpiralSpiralCurve:
                    wrappedEntity = new AlignmentEntitySSC();
                    break;

                case c3ddb.AlignmentEntityType.CurveSpiralSpiral:
                    wrappedEntity = new AlignmentEntityCSS();
                    break;

                // Introduced in 2012.
                //
                //case c3ddb.AlignmentEntityType.CurveLineCurve:
                //    wrappedEntity = new AlignmentEntityCLC();
                //    break;

                //case c3ddb.AlignmentEntityType.CurveReverseCurve:
                //    wrappedEntity = new AlignmentEntityCRC();
                //    break;

                //case c3ddb.AlignmentEntityType.CurveCurveReverseCurve:
                //    wrappedEntity = new AlignmentEntityCCR();
                //    break;

                default:
                    throw new NotImplementedException("Specified entity type not supported.");
            }

            return wrappedEntity;
        }

        private c3ddb.AlignmentEntityCollection m_Entities;
        private int m_CurrentEntityIndex;
    }
}