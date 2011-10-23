using System;

using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.Land.DatabaseServices;

using Colibra;

[assembly: CommandClass(typeof(Autodesk.CivilizedDevelopment.AlignmentEntitiesCommandsColibra))]

namespace Autodesk.CivilizedDevelopment
{
    public class AlignmentEntitiesCommandsColibra : SimpleDrawingCommand
    {
        [CommandMethod("CDS_WriteAlignmentEntitiesById")]
        public void CDS_WriteAlignmentEntitiesById()
        {
            doWriteAlignmentEntities(new ByEntityIdEnumerationPolicy());
        }

        [CommandMethod("CDS_WriteAlignmentEntitiesBySequence")]
        public void CDS_WriteAlignmentEntitiesBySequence()
        {
            doWriteAlignmentEntities(new BySequenceEnumerationPolicy());
        }

        private void doWriteAlignmentEntities(
            IAlignmentEntityEnumerationPolicy enumerationPolicy)
        {
            Document current = DocumentManager.ActiveDocument;
            using (Transaction tr = current.StartTransaction())
            {
                // First we select the alignment to write.
                //
                SingleObjectSelector<Alignment> selector =
                new SingleObjectSelector<Alignment>();
                if (!selector.Select(current))
                {
                    write("\nNo Alignment was selected.");
                    return;
                }

                // We create the processor and assign the policy.
                //
                AlignmentEntityProcessor processor =
                    new AlignmentEntityProcessor(selector.SelectedId);
                processor.EnumerationPolicy = enumerationPolicy;
                
                // We create the writer and write the information to it.
                //
                DocumentOutputAlignmentEntityInfoWriter writer =
                    new DocumentOutputAlignmentEntityInfoWriter(current);
                processor.WriteInfo(writer);
            }
        }
    }
}