using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;

using CivilSurface = Autodesk.Civil.DatabaseServices.Surface;
using AcadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.SurfaceExtractContoursCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class SurfaceExtractContoursCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_ExtractSurfceContours")]
        public void CDS_ExtractSurfceContours()
        {
            ObjectId surfaceId = promptForTinSurface();
            if (surfaceId.IsNull)
            {
                write("\nNo Surface selected.");
                return;
            }

            using (Transaction tr = startTransaction())
            {
                CivilSurface surface = surfaceId.GetObject(OpenMode.ForRead)
                    as CivilSurface;
                showGeneralProperties(surface.Name, 
                    surface.GetGeneralProperties());
                ITerrainSurface terrainSurface = surface as ITerrainSurface;
                if (terrainSurface != null)
                {
                    extractMajorContours(terrainSurface);
                    extractMinorContours(terrainSurface);
                }

                tr.Commit();
            }
        }

        private void showGeneralProperties(string name, 
            GeneralSurfaceProperties props)
        {
            _editor.WriteMessage("\nSurface name: " + name);
            _editor.WriteMessage("\n- Max elevation: " + 
                props.MaximumElevation);
            _editor.WriteMessage("\n- Min elevation: " + 
                props.MinimumElevation);
        }

        private void extractMajorContours(ITerrainSurface surface)
        {
            ObjectIdCollection contours = surface.ExtractMajorContours(
                SurfaceExtractionSettingsType.Model);
            AutoCAD.Colors.Color blue = 
                AutoCAD.Colors.Color.FromRgb(0, 0, 255);
            customizeContours(contours, blue);
        }

        private void extractMinorContours(ITerrainSurface surface)
        {
            ObjectIdCollection contours = surface.ExtractMinorContours(
                SurfaceExtractionSettingsType.Model);
            AutoCAD.Colors.Color lightblue =
                AutoCAD.Colors.Color.FromRgb(0, 255, 255);
            customizeContours(contours, lightblue);
        }

        private void customizeContours(ObjectIdCollection contours, 
            AutoCAD.Colors.Color color)
        {
            foreach (ObjectId id in contours)
            {
                AcadEntity entity = id.GetObject(OpenMode.ForWrite) 
                    as AcadEntity;
                entity.Color = color;
            }
        }


        private ObjectId promptForTinSurface()
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
    }
}