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

using acadappsvcs = Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;

namespace Colibra 
{
    /// <summary>
    /// Encapsulates access to the AutoCAD and Civil 3D document objects.
    /// </summary>
    public class Document 
    {
        /// <summary>
        /// Initializes the object from an AutoCAD document and a Civil 3D
        /// document.
        /// </summary>
        /// <param name="acadDoc">Represented AutoCAD document.</param>
        /// <param name="civilDoc">Represented Civil 3D document.</param>
        /// <para>
        /// The class encapsulates access to the AutoCAD and Civil 3D
        /// document objects. The Document object is instantiated by the
        /// 'DocumentManager' class, which access the AutoCAD and Civil 3D
        /// document objects representing the same drawing and instantiates
        /// a new Document wrapper.
        /// </para>
        internal Document(acadappsvcs.Document acadDoc, CivilDocument civilDoc)
        {
            m_ThisAcadDocument = acadDoc;
            m_ThisCivilDocument = civilDoc;
            m_ActiveTransaction = null;
        }

        /// <summary>
        /// Returns the name of the document.
        /// </summary>
        public string Name 
        { 
            get
            {
                return m_ThisAcadDocument.Name;
            }
        }

        /// <summary>
        /// Returns the list of alignments in the drawing.
        /// </summary>
        public AlignmentList Alignments 
        {
            get 
            {
                return new AlignmentList(this);
            }
        }

        /// <summary>
        /// Activates the document.
        /// </summary>
        public void Activate()
        {
            DocumentManager._activateDocument(this);
        }

        /// <summary>
        /// Starts a document transaction.
        /// </summary>
        /// <returns>The newly created Transaction object.</returns>
        public Transaction StartTransaction()
        {
            if (m_ActiveTransaction == null)
            {
                m_ActiveTransaction = new Transaction(this);
            }
            return m_ActiveTransaction;
        }

        /// <summary>
        /// Returns a reference to the AutoCAD document represented
        /// by this object.
        /// </summary>
        /// <para>
        /// This method provides internal access to the AutoCAD document
        /// object. The property is for internal use and should not be
        /// exposed to users of Colibra.
        /// </para>
        internal acadappsvcs.Document _acaddoc 
        {
            get
            {
                return m_ThisAcadDocument;
            }
        }

        /// <summary>
        /// Returns a reference to the Civil document represented
        /// by this object.
        /// </summary>
        /// <para>
        /// This method provides internal access the the Civil document
        /// object. The property is for internal use and should not be
        /// exposed to users of Colibra.
        /// </para>
        internal CivilDocument _civildoc
        {
            get
            {
                return m_ThisCivilDocument;
            }
        }

        /// <summary>
        /// Terminates the current active transaction.
        /// </summary>
        /// <para>
        /// This method is used internally by the Transaction object
        /// to clear the current transaction when the Transaction
        /// object is being disposed. No other objects in the library
        /// should invoke this method.
        /// </para>
        internal void _closeTransaction()
        {
            m_ActiveTransaction = null;
        }


        private acadappsvcs.Document m_ThisAcadDocument;
        private CivilDocument m_ThisCivilDocument;
        private Transaction m_ActiveTransaction;
    }
}