using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.CogoPointLabelCommands))]

namespace Autodesk.CivilizedDevelopment
{
    class CogoPointLabelCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_CogoPointLabelDemo")]
        public void CDS_CogoPointLabelDemo()
        {
            createPointLabelStyle();
            createCogoPoints();
            createPointGroup();
        }

        private void createCogoPoints()
        {
            RandomCoordinateGenerator generator =
                new RandomCoordinateGenerator();
            _civildoc.CogoPoints.Add(generator.GetCoordinates(10), 
                "COORDINATE");
        }

        private void createPointGroup()
        {
            _pointGroupId = _civildoc.PointGroups.Add("Coordinates");
            using (Transaction tr = startTransaction())
            {
                customizePointGroup();
                tr.Commit();
            }
        }

        private void customizePointGroup()
        {
            CustomPointGroupQuery query = new CustomPointGroupQuery();
            query.QueryString = "RawDescription='COORD*'";
            PointGroup group = _pointGroupId.GetObject(OpenMode.ForWrite)
                as PointGroup;
            group.SetQuery(query);
            group.PointLabelStyleId = _pointLabelStyleId;
            group.IsPointLabelStyleOverridden = true;
        }

        private void createPointLabelStyle()
        {
            _pointLabelStyleId = _pointLabelStyles.Add("DemoPointStyle");
            customizePointLabelStyle();
        }

        private void customizePointLabelStyle()
        {
            using (Transaction tr = startTransaction())
            {
                makePointLabelCustomizations();
                tr.Commit();
            }
        }

        private void makePointLabelCustomizations()
        {
            LabelStyle style = _pointLabelStyleId.GetObject(OpenMode.ForWrite)
                as LabelStyle;
            
            ObjectId textId = style.AddComponent("Text", LabelStyleComponentType.Text);
            LabelStyleTextComponent text = textId.GetObject(OpenMode.ForWrite)
                as LabelStyleTextComponent;
            text.Text.Contents.Value = "SAMPLE TEXT";
        }


        private LabelStyleCollection _pointLabelStyles
        {
            get
            {
                return _civildoc.Styles.LabelStyles
                    .PointLabelStyles.LabelStyles;
            }
        }

        private ObjectId _pointLabelStyleId;
        private ObjectId _pointGroupId;
    }
}
