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
            assignTargets();
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
            _region = _baseline.BaselineRegions.Add(
                RegionName, AssemblyId);
        }

        private void assignTargets()
        {
            // These will return empty collections because the
            // Corridor object has not been built yet.
            //
            // SubassemblyTargetInfoCollection targets = 
            //      _corridor.GetTargets();
            // SubassemblytargetInfoCollection targets = 
            //      _baseline.GetTargets();

            // Getting the targets from the BaselineRegion 
            // works because it access the information from
            // the specified Assembly object when creating the
            // BaselineRegion.
            //
            SubassemblyTargetInfoCollection targets = 
                _region.GetTargets();
            foreach (SubassemblyTargetInfo target in targets)
            {
                assignTarget(target);
                
            }
            // The collection is empty if retrieved from the
            // Corridor or Baseline object; therefore these
            // calls will do nothing.
            // 
            // _corridor.SetTargets(targets);
            // _baseline.SetTargets(targets);

            // Regions allow you to specify the targets.
            //
            _region.SetTargets(targets);
        }

        private void assignTarget(SubassemblyTargetInfo target)
        {
            switch(target.TargetType)
            {
                case SubassemblyLogicalNameType.Surface:
                    assignSurfaceTarget(target);
                    break;

                case SubassemblyLogicalNameType.Elevation:
                    assignElevationTarget(target);
                    break;

                case SubassemblyLogicalNameType.Offset:
                    assignOffsetTarget(target);
                    break;
            }
        }

        private void assignSurfaceTarget(
            SubassemblyTargetInfo target)
        {
            // The 'Add()' method of 'ObjectIdCollection'
            // knows nothing about subassembly targets; therefore
            // this call doesn't work.
            //
            // target.TargetIds.Add(surfaceId);

            target.TargetIds = _targetSurfaces;

            // Alternatively, you can get the collection,
            // manipulate it, and then set it again.
            //
            // ObjectIdCollection ids = target.TargetIds;
            // ... do whatever manipulations (add/remove targets)
            // target.TargetIds = ids;
            //
            // This will work, but trust me, my way (starting
            // clean) it is easier most of the time.
        }

        private void assignElevationTarget(
            SubassemblyTargetInfo target)
        {
            if (isRightSide(target.SubassemblyName))
            {
                target.TargetIds = _targetRightElevation;
            }
            else
            {
                target.TargetIds = _targetLeftElevation;
            }
        }

        private void assignOffsetTarget(
            SubassemblyTargetInfo target)
        {
            if (isRightSide(target.SubassemblyName))
            {
                target.TargetIds = _targetRightOffset;
            }
            else
            {
                target.TargetIds = _targetLeftOffset;
            }
        }

        private bool isRightSide(string value)
        {
            return value.Contains("Right");
        }

        private CorridorCollection _corridors
        {
            get { return _document.CorridorCollection; }
        }

        private ObjectIdCollection _targetSurfaces
        {
            get
            {
                return _document.GetSurfaceIds();
            }
        }

        private ObjectIdCollection _targetRightOffset
        {
            get
            {
                if (_rightOffsetAlignmentId == ObjectId.Null)
                {
                    resolveAllTargetIds();
                }
                return insideCollection(_rightOffsetAlignmentId);
            }
        }

        private ObjectIdCollection _targetLeftOffset
        {
            get
            {
                if (_leftOffsetAlignmentId == ObjectId.Null)
                {
                    resolveAllTargetIds();
                }
                return insideCollection(_leftOffsetAlignmentId);
            }
        }

        private ObjectIdCollection _targetRightElevation
        {
            get
            {
                if (_rightProfileId == ObjectId.Null)
                {
                    resolveAllTargetIds();
                }
                return insideCollection(_rightProfileId);
            }
        }

        private ObjectIdCollection _targetLeftElevation
        {
            get
            {
                if (_leftProfileId == ObjectId.Null)
                {
                    resolveAllTargetIds();
                }
                return insideCollection(_leftProfileId);
            }
        }

        private void resolveAllTargetIds()
        {
            Alignment alignment = AlignmentId.GetObject(
                OpenMode.ForRead) as Alignment;
            ObjectIdCollection offsetAlignments = 
                alignment.GetChildOffsetAlignmentIds();
            foreach (ObjectId offsetId in offsetAlignments)
            {
                resolveTargetIds(offsetId);
            }
        }

        private void resolveTargetIds(ObjectId alignmentId)
        {
            Alignment alignment = alignmentId.GetObject(
                OpenMode.ForRead) as Alignment;
            if (isRightSide(alignment.Name))
            {
                _rightOffsetAlignmentId = alignmentId;
                _rightProfileId = alignment.GetProfileIds()[0];

            }
            else
            {
                _leftOffsetAlignmentId = alignmentId;
                _leftProfileId = alignment.GetProfileIds()[0];
            }
        }

        private ObjectIdCollection insideCollection(ObjectId id)
        {
            ObjectIdCollection col = new ObjectIdCollection();
            col.Add(id);
            return col;
        }

        private CivilDocument _document;
        private Corridor _corridor;
        private Baseline _baseline;
        private BaselineRegion _region;
        private ObjectId _leftOffsetAlignmentId = ObjectId.Null;
        private ObjectId _rightOffsetAlignmentId = ObjectId.Null;
        private ObjectId _leftProfileId = ObjectId.Null;
        private ObjectId _rightProfileId = ObjectId.Null;
    }
}