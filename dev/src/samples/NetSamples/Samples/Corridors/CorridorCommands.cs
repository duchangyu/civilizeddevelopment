using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

[assembly: CommandClass(typeof(
    Autodesk.CivilizedDevelopment.CorridorCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class CorridorCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_CreateCorridorFromScratch")]
        public void CDS_CreateCorridorFromScratch()
        {
            try
            {
                executeCreateCorridorFromScratch();
                
            }
            catch (System.Exception ex)
            {
                logException(ex);
            }
        }

        private void executeCreateCorridorFromScratch()
        {
            using (Transaction tr = startTransaction())
            {
                CorridorCreator creator = new CorridorCreator()
                {
                    CorridorName = promptForString(
                        "\nEnter corridor name: "),
                    BaselineName = promptForString(
                        "\nEnter baseline name: "),
                    RegionName = promptForString(
                        "\nEnter region name: "),
                    AlignmentId = _desiredAlignmentId,
                    AssemblyId = _desiredAssemblyId
                };
                creator.CreateCorridor(_civildoc);

                tr.Commit();
            }
        }

        private string promptForString(string prompt)
        {
            PromptResult result = _editor.GetString(prompt);
            if (result.Status == PromptStatus.OK)
            {
                return result.StringResult;
            }
            return String.Empty;
        }

        private ObjectId _desiredAlignmentId
        {
            get
            {
                return _civildoc.GetAlignmentIds()[0];
            }
        }

        private ObjectId _desiredAssemblyId
        {
            get
            {
                return _civildoc.AssemblyCollection[0];
            }
        }
    }

    internal class CorridorCreator
    {
        public string CorridorName { get; set; }
        public string BaselineName { get; set; }
        public string RegionName { get; set; }
        public ObjectId AlignmentId { get; set; }
        public ObjectId AssemblyId { get; set; }

        public ObjectId ProfileId
        {
            get
            {
                Alignment alignment = AlignmentId.GetObject(
                    OpenMode.ForRead) as Alignment;
                return alignment.GetProfileIds()[0];
            }
        }

        public void CreateCorridor(CivilDocument document)
        {
            _document = document;
            createCorridorObject();
            createCorridorBaseline();
            createBaselineRegion();
            _corridor.Rebuild();
        }

        private void createCorridorObject()
        {
            ObjectId id = _corridors.Add(CorridorName);
            _corridor = id.GetObject(OpenMode.ForWrite)
                as Corridor;
        }

        private void createCorridorBaseline()
        {
            _baseline = _corridor.Baselines.Add(BaselineName, 
                AlignmentId, ProfileId);
        }

        private void createBaselineRegion()
        {
            _baseline.BaselineRegions.Add(RegionName, AssemblyId);
        }

        private CorridorCollection _corridors
        {
            get { return _document.CorridorCollection; }
        }

        private CivilDocument _document;
        private Corridor _corridor;
        private Baseline _baseline;
    }
}