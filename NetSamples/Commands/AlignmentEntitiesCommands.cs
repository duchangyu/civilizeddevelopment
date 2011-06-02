using System;
using System.IO;
using System.Text;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.Land.DatabaseServices;

[assembly: CommandClass(typeof(Autodesk.CivilizedDevelopment.AlignmentEntitiesCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class AlignmentEntitiesCommands
    {
        public AlignmentEntitiesCommands()
        {
            _log = false;
            m_OutputBuilder = new StringBuilder();
        }

        [CommandMethod("CDS_DisplayAlignmentEntities")]
        public void CDS_DisplayAlignmentEntities()
        {
            EnumerateEntities += enumerateEntitiesById;
            doDisplayAlignmentEntities();
            EnumerateEntities -= enumerateEntitiesById;
        }

        [CommandMethod("CDS_DisplayAlignmentEntitiesByOrder")]
        public void CDS_DisplayAlignmentEntitiesByOrder()
        {
            EnumerateEntities += enumerateEntitiesByOrder;
            doDisplayAlignmentEntities();
            EnumerateEntities -= enumerateEntitiesByOrder;
        }

        [CommandMethod("CDS_DisplayAllAlignments")]
        public void CDS_DisplayAllAlignments()
        {
            EnumerateEntities += enumerateEntitiesById;
            _log = true;
            doDisplayAllAlignments();
            writeToLogFile("AllAlignments.log");
            EnumerateEntities -= enumerateEntitiesByOrder;
        }

        private void doDisplayAlignmentEntities()
        {
            ObjectId alignmentId = promptForAlignment();
            if (ObjectId.Null != alignmentId)
            {
                displayAlignmentEntities(alignmentId);
            }
        }

        private void doDisplayAllAlignments()
        {
            ObjectIdCollection alignmentIds = CivilApplication.ActiveDocument.GetAlignmentIds();
            using (Transaction tr = startTransaction())
            {
                foreach (ObjectId alignmentId in alignmentIds)
                {
                    displayAlignmentEntities(alignmentId);
                }
            }
        }

        private void writeToLogFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false))
            {
                writer.Write(m_OutputBuilder.ToString());
            }
            
        }

        private ObjectId promptForAlignment()
        {
            // We use PromptEntityOptions to insure the entity selected
            // is an Alignment object. When filtering for a type of entity,
            // we need to set a reject message in case the user selects
            // an entity type not allowed.
            //
            string promptMsg = "\nSelect Alignment: ";
            string rejectMsg = "\nSelected entity is not an alignment.";
            PromptEntityOptions opts = new PromptEntityOptions(promptMsg);
            opts.SetRejectMessage(rejectMsg);
            opts.AddAllowedClass(typeof(Alignment), false);
            PromptEntityResult result = _editor.GetEntity(opts);
            if (result.Status == PromptStatus.OK)
            {
                return result.ObjectId;
            }
            return ObjectId.Null;
        }

        private void displayAlignmentEntities(ObjectId alignmentId)
        {
            using (Transaction tr = startTransaction())
            {
                Alignment alignment = 
                    alignmentId.GetObject(OpenMode.ForRead) as Alignment;
                write("\n----------------------------------------------------");
                write("\nAlignment Name: " + alignment.Name);
                EnumerateEntities(alignment);
                write("\n----------------------------------------------------");
                tr.Commit();
            }
        }

        private void enumerateEntitiesById(Alignment alignment)
        {
            foreach (AlignmentEntity entity in alignment.Entities)
            {
                write("\n.. Entity ID: " + entity.EntityId);
                write("\n.. Entity Class: " + entity.GetType());
                write("\n.. Entity Type: " + entity.EntityType);
                write("\n.. Subentities: " + entity.SubEntityCount);
            }
        }

        private void enumerateEntitiesByOrder(Alignment alignment)
        {
            AlignmentEntityCollection entities = alignment.Entities;
            for (int i = 0; i < entities.Count; i++)
            {
                AlignmentEntity entity = entities.GetEntityByOrder(i);
                write("\n.. Entity ID: " + entity.EntityId);
                write("\n.. Entity Sequence: " + i);
                write("\n.. Entity Class: " + entity.GetType());
                write("\n.. Entity Type: " + entity.EntityType);
                write("\n.. Subentities: " + entity.SubEntityCount);
            }
        }

        /// <summary>
        /// Starts a new database transaction.
        /// </summary>
        /// <returns>The new transaction object.</returns>
        private Transaction startTransaction()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            return db.TransactionManager.StartTransaction();
        }

        /// <summary>
        /// Writes a message string to the AutocAD command line window.
        /// </summary>
        /// <param name="msg">String to write.</param>
        private void write(string msg)
        {
            _editor.WriteMessage("\n" + msg);
            if (_log)
            {
                m_OutputBuilder.AppendLine(msg);
            }
        }

        /// <summary>
        /// Indicates if the output should be logged to a log fiel.
        /// </summary>
        private bool _log { get; set; }

        /// <summary>
        /// Returns a reference to the Editor of the current document.
        /// </summary>
        private Editor _editor
        {
            get
            {
                if (m_Editor == null)
                {
                    m_Editor =
                        Application.DocumentManager.MdiActiveDocument.Editor;
                }

                return m_Editor;
            }
        }

        private Editor m_Editor = null;
        private StringBuilder m_OutputBuilder;

        // To delegate enumeration of entities.
        //
        private delegate void EnumerateEntitiesDelegate(Alignment alignment);
        private EnumerateEntitiesDelegate EnumerateEntities;
        
    }
}