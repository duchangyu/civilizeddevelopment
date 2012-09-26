using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;

using CivilSurface = Autodesk.Civil.DatabaseServices.Surface;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.SamplingPointsCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class SamplingPointsCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_SamplePointsOnSurface")]
        public void CDS_SamplePointsOnSurface()
        {
            using (Transaction tr = startTransaction())
            {
                ITerrainSurface surface = getSurface() as ITerrainSurface;
                if (surface == null) return;
                ObjectId curveId = getCurveId();

                displayPoints(surface.SampleElevations(curveId));
            }
        }

        private CivilSurface getSurface()
        {
            ObjectId surfaceId = promptForSurface();
            if (surfaceId.IsNull)
            {
                return null;
            }
            CivilSurface surface = surfaceId.GetObject(OpenMode.ForRead)
                as CivilSurface;
            return surface;
        }

        private ObjectId promptForSurface()
        {
            PromptEntityOptions options = new PromptEntityOptions(
                "\nSelect a TIN Surface: ");
            options.SetRejectMessage(
                "\nThe selected object is not a TIN Surface.");
            options.AddAllowedClass(typeof(CivilSurface), false);

            PromptEntityResult result = _editor.GetEntity(options);
            if (result.Status == PromptStatus.OK)
            {
                // Everything is cool; we return the selected
                // surface ObjectId.
                return result.ObjectId;
            }
            return ObjectId.Null;   // Indicating error.
        }

        private ObjectId getCurveId()
        {
            PromptEntityOptions options = new PromptEntityOptions(
                "\nSelect entity: ");
            options.SetRejectMessage(
                "\nThe selected entity is not of the valid type.");
            PromptEntityResult result = _editor.GetEntity(options);
            if (result.Status == PromptStatus.OK)
            {
                return result.ObjectId;
            }
            return ObjectId.Null;
        }

        private void displayPoints(Point3dCollection points)
        {
            foreach (Point3d point in points)
            {
                _editor.WriteMessage("\n" + point.ToString());
            }
        }
    }
}