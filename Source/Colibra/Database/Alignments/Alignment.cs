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

using Autodesk.AutoCAD.DatabaseServices;
using c3ddb = Autodesk.Civil.Land.DatabaseServices;

namespace Colibra 
{
    /// <summary>
    /// Encapsulates a Civil 3D alignment object.
    /// </summary>
    public class Alignment
    {
        /// <summary>
        /// Class constructor that initializes the object
        /// from the object id of the alignment it represents.
        /// </summary>
        /// <param name="alignmentId">Id of the wrapped alignment.</param>
        /// <para>
        /// The constructor is internal and invoked by the AlignmentList 
        /// class to wrap and initialize access to an alignment object.
        /// </para>
        internal Alignment(ObjectId alignmentId)
        {
            m_AlignmentId = alignmentId;
        }

        /// <summary>
        /// Returns a list of entities in the alignment.
        /// </summary>
        public AlignmentEntityList Entities
        {
            get 
            {
                return new AlignmentEntityList(instance.Entities);
            }
        }

        /// <summary>
        /// Returns the alignment name.
        /// </summary>
        public string Name 
        {
            get
            {
                return instance.Name;
            }
        }

        /// <summary>
        /// Provides access to the alignment object instance.
        /// </summary>
        private c3ddb.Alignment instance 
        {
            get
            {
                return m_AlignmentId.GetObject(OpenMode.ForRead) as c3ddb.Alignment;
            }
        }

        private ObjectId m_AlignmentId;
    }
}