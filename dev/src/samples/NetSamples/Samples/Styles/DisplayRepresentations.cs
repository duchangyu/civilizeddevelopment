using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.Land.DatabaseServices;
using Autodesk.Civil.Land.DatabaseServices.Styles;

[assembly: CommandClass(typeof(Autodesk.CivilizedDevelopment.DisplayRepresentations))]

namespace Autodesk.CivilizedDevelopment
{
    public class DisplayRepresentations : SimpleDrawingCommand
    {
        [CommandMethod("CDS_TurnDisplayRepresentationsOn")]
        public void CDS_TurnDisplayRepresentationsOn()
        {
            ObjectId styleId = findSurfaceStyle("SampleStyle");
            if (styleId == ObjectId.Null)
            {
                styleId = _civildoc.Styles.SurfaceStyles.Add("SampleStyle");
            }

            using (Transaction tr = startTransaction())
            {
                SurfaceStyle style = styleId.GetObject(OpenMode.ForWrite) 
                    as SurfaceStyle;
                SurfaceDisplayStyleType[] settings =
                    new SurfaceDisplayStyleType[]
                    {
                        SurfaceDisplayStyleType.MajorContour,
                        SurfaceDisplayStyleType.MinorContour,
                        SurfaceDisplayStyleType.Points,
                        SurfaceDisplayStyleType.Triangles
                    };

                applySettings(style, settings);

                tr.Commit();
            }
        }

        private ObjectId findSurfaceStyle(string name)
        {
            using (Transaction tr = startTransaction())
            {
                foreach (ObjectId styleId in _civildoc.Styles.SurfaceStyles)
                {
                    SurfaceStyle style = styleId.GetObject(OpenMode.ForRead)
                        as SurfaceStyle;
                    if (style.Name == name)
                    {
                        return styleId;
                    }
                }
            }

            return ObjectId.Null;
        }

        private void applySettings(SurfaceStyle style, 
            IList<SurfaceDisplayStyleType> settings)
        {
            IEnumerable<SurfaceDisplayStyleType> displayTypes =
                Enum.GetValues(typeof(SurfaceDisplayStyleType)) 
                as IEnumerable<SurfaceDisplayStyleType>;

            foreach (SurfaceDisplayStyleType displayType in displayTypes)
            {
                bool state = settings.Contains(displayType);
                style.GetDisplayStylePlan(displayType).Visible = state;
                style.GetDisplayStyleModel(displayType).Visible = state;
            }
        }
    }
}