using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.AlignmentLabelCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class AlignmentLabelCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_AddElevationToMajorStationLabels")]
        public void CDS_AddElevationToMajorStationLabels()
        {
            ObjectId alignmentId = selectAlignment();
            if (alignmentId == ObjectId.Null)
            {
                return;
            }

            using (Transaction tr = startTransaction())
            {
                Alignment alignment = readable<Alignment>(alignmentId);
                addElevationComponentToMajorStations(alignment);
                
                tr.Commit();
            }
        }

        private ObjectId selectAlignment()
        {
            PromptEntityOptions options = new PromptEntityOptions(
                "\nSelect alignment: ");
            options.SetRejectMessage("\nSelected object is not an alignment.");
            options.AddAllowedClass(typeof(Alignment), true);
            PromptEntityResult result = _editor.GetEntity(options);
            if (result.Status == PromptStatus.OK)
            {
                return result.ObjectId;
            }

            return ObjectId.Null;
        }

        private void addElevationComponentToMajorStations(Alignment alignment)
        {
            LabelGroup group = getMajorStationsLabelGroup(alignment);
            addElevationComponentToStyle(group);

            
        }

        private LabelGroup getMajorStationsLabelGroup(Alignment alignment)
        {
            foreach (ObjectId id in alignment.GetAlignmentLabelGroupIds())
            {
                LabelGroup group = readable<LabelGroup>(id);
                if (group.GetType() == typeof(AlignmentStationLabelGroup))
                {
                    return group;
                }
            }

            return null;    // Shouldn't hit this in the example.
        }

        private void addElevationComponentToStyle(LabelGroup group)
        {
            LabelStyle style = writable<LabelStyle>(group.StyleId);
            ObjectId componentId = style.AddReferenceTextComponent(
                "ProfileElevation", ReferenceTextComponentSelectedType.Profile);
            LabelStyleReferenceTextComponent component =
                writable<LabelStyleReferenceTextComponent>(componentId);
            component.Text.Contents.Value =
                "<[Profile Elevation(Uft|P2|RN|AP|GC|UN|Sn|OF)]>";
        }

        private T readable<T>(ObjectId id)  
            where T : Autodesk.AutoCAD.DatabaseServices.DBObject
        {
            return openObject<T>(id, OpenMode.ForRead);
        }

        private T writable<T>(ObjectId id) 
            where T : Autodesk.AutoCAD.DatabaseServices.DBObject
        {
            return openObject<T>(id, OpenMode.ForWrite);
        }

        private T openObject<T>(ObjectId id, OpenMode mode) 
            where T : Autodesk.AutoCAD.DatabaseServices.DBObject
        {
            T obj = id.GetObject(mode) as T;
            if (obj == null)
            {
                throw new ArgumentException("Object not from specified type");
            }

            return obj;
        }
    }
}