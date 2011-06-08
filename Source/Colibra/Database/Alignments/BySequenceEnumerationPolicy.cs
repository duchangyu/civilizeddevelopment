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

using Autodesk.Civil.Land.DatabaseServices;

namespace Colibra
{
    /// <summary>
    /// Enumeration policy that enumerates the alignment entities by
    /// sequence in the alignment.
    /// </summary>
    public class BySequenceEnumerationPolicy : IAlignmentEntityEnumerationPolicy
    {
        /// <summary>
        /// Initializes the policy from the alignment entities.
        /// </summary>
        /// <param name="entities">Entities to enumerate.</param>
        public void Initialize(AlignmentEntityCollection entities)
        {
            m_TheEntities = entities;
        }

        /// <summary>
        /// Returns enumerator.
        /// </summary>
        /// <returns>Returns enumerator object.</returns>
        public IEnumerator<AAlignmentEntity> GetEnumerator()
        {
            return new ByEntityIdEnumerator(m_TheEntities);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns>Returns enumerator object.</returns>
        System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator()
        {
            return new ByEntityIdEnumerator(m_TheEntities);
        }

        private AlignmentEntityCollection m_TheEntities;
    }


    /// <summary>
    /// Enumerator object that enumerates the entities by sequence order.
    /// </summary>
    public class BySequenceEnumerator : IEnumerator<AAlignmentEntity>
    {
        /// <summary>
        /// Initializes the class from the alignment entities.
        /// </summary>
        /// <param name="entities">Entities to enumerate</param>
        internal BySequenceEnumerator(AlignmentEntityCollection entities)
        {
            m_TheEntities = entities;
            m_EntityIndex = -1;
        }

        /// <summary>
        /// Returns the current entity.
        /// </summary>
        public AAlignmentEntity Current
        {
            get {
                AlignmentEntity entity = 
                    m_TheEntities.GetEntityByOrder(m_EntityIndex);
                return AlignmentEntityWrapperFactory.WrapEntity(entity);
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            m_TheEntities = null;
            m_EntityIndex = -1;
        }

        /// <summary>
        /// Returns the current entity.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get {
                AlignmentEntity entity =
                    m_TheEntities.GetEntityByOrder(m_EntityIndex);
                return AlignmentEntityWrapperFactory.WrapEntity(entity);
            }
        }

        /// <summary>
        /// Moves to the next entity.
        /// </summary>
        /// <returns>Returns true if there are entities remaining or
        /// false otherwise.</returns>
        public bool MoveNext()
        {
            m_EntityIndex++;
            return m_EntityIndex < m_TheEntities.Count;
        }

        /// <summary>
        /// Resets the enumerator.
        /// </summary>
        public void Reset()
        {
            m_EntityIndex = -1;
        }

        private AlignmentEntityCollection m_TheEntities;
        private int m_EntityIndex;
    }
}