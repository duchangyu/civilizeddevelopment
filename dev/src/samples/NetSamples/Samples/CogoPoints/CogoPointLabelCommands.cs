using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.CogoPointLabelCommands))]

namespace Autodesk.CivilizedDevelopment
{
    class CogoPointLabelCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_CreateDemoPointLabelStyle")]
        public void CDS_CreateDemoPointLabelStyle()
        {
            
            createPointLabelStyle("Demo"); 
        }

        private void createPointLabelStyle(string name)
        {
            ObjectId styleId = _pointLabelStyles.Add(name);
            removeAllComponents(styleId);
            customizeStyle(styleId);
        }

        private LabelStyleCollection _pointLabelStyles
        {
            get
            {
                return _civildoc.Styles.LabelStyles.PointLabelStyles.LabelStyles;
            }
        }

        private void removeAllComponents(ObjectId styleId)
        {
            IEnumerable<string> componentNames = getTextComponentNames(styleId);
            removeComponents(styleId, componentNames);
        }

        private IEnumerable<string> getTextComponentNames(ObjectId styleId)
        {
            List<string> names = new List<string>();
            using (Transaction tr = startTransaction())
            {
                LabelStyle style = styleId.GetObject(OpenMode.ForRead)
                    as LabelStyle;
                foreach (ObjectId id in style.GetComponents(
                    LabelStyleComponentType.Text))
                {
                    LabelStyleComponent component =
                        id.GetObject(OpenMode.ForRead) as LabelStyleComponent;
                    names.Add(component.Name);
                }
            }
            return names;
        }

        private void removeComponents(ObjectId styleId, 
            IEnumerable<string> componentNames)
        {
            using (Transaction tr = startTransaction())
            {
                LabelStyle style = styleId.GetObject(OpenMode.ForWrite)
                    as LabelStyle;
                foreach (string name in componentNames)
                {
                    style.RemoveComponent(name);
                }

                tr.Commit();
            }
        }

        private void customizeStyle(ObjectId styleId)
        {
            using (Transaction tr = startTransaction())
            {
                addStyleComponents(styleId);
                tr.Commit();
            }
        }

        private void addStyleComponents(ObjectId styleId)
        {
            LabelStyle style = styleId.GetObject(OpenMode.ForWrite)
                    as LabelStyle;
            addLeaderComponent(style);
            addPointNumberComponent(style);
            addLocationComponent(style);
        }

        private void addLeaderComponent(LabelStyle style)
        {
            ObjectId id = style.AddComponent("Leader", 
                LabelStyleComponentType.Line);
            LabelStyleLineComponent component = id.GetObject(OpenMode.ForWrite)
                as LabelStyleLineComponent;
            component.General.StartAnchorPoint.Value = AnchorPointType.MiddleCenter;
        }

        private void addPointNumberComponent(LabelStyle style)
        {
            ObjectId id = style.AddComponent("PN", 
                LabelStyleComponentType.Text);
            LabelStyleTextComponent component = id.GetObject(OpenMode.ForWrite)
                as LabelStyleTextComponent;
            component.Text.Attachment.Value = LabelTextAttachmentType.MiddleLeft;
            component.Text.Contents.Value = _pointNumber;
            component.General.AnchorComponent.Value = "Leader";
            component.General.AnchorLocation.Value = AnchorPointType.End;
        }

        private void addLocationComponent(LabelStyle style)
        {
            ObjectId id = style.AddComponent("Location", 
                LabelStyleComponentType.Text);
            LabelStyleTextComponent component = id.GetObject(OpenMode.ForWrite)
                as LabelStyleTextComponent;
            component.Text.Attachment.Value = LabelTextAttachmentType.TopLeft;
            string value = String.Format("({0}, {1}, {2})", 
                _northing, _easting, _elevation);
            component.Text.Contents.Value = value;
            component.General.AnchorComponent.Value = "PN";
            component.General.AnchorLocation.Value = AnchorPointType.BottomLeft;
        }

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

        private readonly string _pointNumber = "<[Point Number(Sn)]>";
        private readonly string _northing = 
            "<[Northing(Uft|P4|RN|AP|GC|UN|Sn|OF)]>";
        private readonly string _easting =
            "<[Easting(Uft|P4|RN|AP|GC|UN|Sn|OF)]>";
        private readonly string _elevation =
            "<[Point Elevation(Uft|P2|RN|AP|Sn|OF)]>";
    }
}
