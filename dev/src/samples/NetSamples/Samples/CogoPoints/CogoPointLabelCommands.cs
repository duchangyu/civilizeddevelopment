using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.CogoPointLabelCommands))]

namespace Autodesk.CivilizedDevelopment
{
    class CogoPointLabelCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_ShowCogoPointLabelProperties")]
        public void CDS_ShowCogoPointLabelProperties()
        {
            ObjectId pointId = selectCogoPoint();
            if (pointId == ObjectId.Null)
            {
                return;
            }
            showLabelProperties(pointId);
        }

        [CommandMethod("CDS_OverrideLabelsForGroup")]
        public void CDS_OverrideLabelsForGroup()
        {
            using (Transaction tr = startTransaction())
            {
                ObjectId pointGroupId = promptForPointGroup();
                if (pointGroupId == ObjectId.Null)
                {
                    return;
                }
                overrideLabelsForPointsIn(pointGroupId);
                tr.Commit();
            }
            
        }

        private ObjectId selectCogoPoint()
        {
            PromptEntityOptions options = new PromptEntityOptions(
                "Select COGO point: ");
            options.SetRejectMessage("\nInvalid COGO point selected.");
            options.AddAllowedClass(typeof(CogoPoint), true);
            PromptEntityResult result = _editor.GetEntity(options);
            if (result.Status == PromptStatus.OK)
            {
                return result.ObjectId;
            }
            return ObjectId.Null;
        }

        private void showLabelProperties(ObjectId pointId)
        {
            using (Transaction tr = startTransaction())
            {
                CogoPoint point = pointId.GetObject(OpenMode.ForRead)
                    as CogoPoint;
                showLabelPropertiesFor(point);
            }
        }

        private void showLabelPropertiesFor(CogoPoint point)
        {
            write("\nPoint Label Properties:");
            write("\n- Style: " + getLabelStyleName(point.LabelStyleId));
            write("\n- Style override: " 
                + getLabelStyleName(point.LabelStyleIdOverride));
            write("\n- Visible: " + point.IsLabelVisible.ToString());
            write("\n- Location: " + point.LabelLocation.ToString());
            write("\n- Rotation: " + point.LabelRotation.ToString());
            write("\n- Dragged: " + point.IsLabelDragged.ToString());
            write("\n- Pinned: " + point.IsLabelPinned.ToString());
        }

        private string getLabelStyleName(ObjectId id)
        {
            LabelStyle style = id.GetObject(OpenMode.ForRead) as LabelStyle;
            return style.Name;
        }

        private ObjectId promptForPointGroup()
        {
            PromptResult result = _editor.GetString(
                "\nEnter point group name: ");
            if (result.Status == PromptStatus.OK)
            {
                return findGroup(result.StringResult);
            }
            return ObjectId.Null;
        }

        private void overrideLabelsForPointsIn(ObjectId pointGroupId)
        {
            PointGroup group = pointGroupId.GetObject(OpenMode.ForWrite)
                as PointGroup;
            group.IsPointLabelStyleOverridden = true;
        }

        private ObjectId findGroup(string name)
        {
            foreach (ObjectId id in _civildoc.PointGroups)
            {
                PointGroup group = id.GetObject(OpenMode.ForRead) 
                    as PointGroup;
                if (group.Name == name)
                {
                    return id;
                }
            }
            return ObjectId.Null;
        }
    }
}
