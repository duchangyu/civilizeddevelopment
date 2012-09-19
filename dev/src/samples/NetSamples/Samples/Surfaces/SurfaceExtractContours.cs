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
        [CommandMethod("CDS_ExtractSurfaceContoursAtElevation")]
        public void CDS_ExtractSurfaceContoursAtElevation()
        {
            using (Transaction tr = startTransaction())
            {
                ITerrainSurface surface = getSurface() as ITerrainSurface;
                if (surface == null) return;

                double elevation = getDouble("elevation");
                if (Double.IsNaN(elevation)) return;

                if (elevationInSurfaceRange(elevation, 
                    surface as CivilSurface))
                {
                    ObjectIdCollection contours = 
                        surface.ExtractContoursAt(elevation);
                    customizeContours(contours, _singleContourColor);
                }

                tr.Commit();
            }
        }

        private bool elevationInSurfaceRange(double elevation, 
            CivilSurface surface)
        {
            GeneralSurfaceProperties properties = 
                surface.GetGeneralProperties();
            if (elevation < properties.MinimumElevation || 
                elevation > properties.MaximumElevation)
            {
                _editor.WriteMessage(
                    "\nSpecified elevation not in surface range.");
                return false;
            }
            return true;

        }

        [CommandMethod("CDS_ExtractSurfaceContoursFromToElevationRange")]
        public void CDS_ExtractSurfaceContoursFromToElevationRange()
        {
            using (Transaction tr = startTransaction())
            {
                ITerrainSurface surface = getSurface() as ITerrainSurface;
                if (surface == null) return;
                
                double minElevation = getDouble("minimum elevation");
                if (Double.IsNaN(minElevation)) return;

                double maxElevation = getDouble("maximum elevation");
                if (Double.IsNaN(maxElevation)) return;

                double interval = getDouble("interval");
                if (Double.IsNaN(interval)) return;

                ObjectIdCollection contours =
                    surface.ExtractContours(
                        minElevation, maxElevation, interval);

                customizeContours(contours, _rangedContoursColor);
                    
                tr.Commit();
            }
        }

        [CommandMethod("CDS_ExtractSurfaceContoursAtInterval")]
        public void CDS_ExtractSurfaceContoursAtInterval()
        {
            using (Transaction tr = startTransaction())
            {
                ITerrainSurface surface = getSurface() as ITerrainSurface;
                if (surface == null) return;

                double interval = getDouble("interval");
                if (Double.IsNaN(interval)) return;

                ObjectIdCollection contours =
                    surface.ExtractContours(interval);

                customizeContours(contours, _intervalContoursColor);

                tr.Commit();
            }
        }

        [CommandMethod("CDS_ExtractSurfaceMajorAndMinorContours")]
        public void CDS_ExtractSurfaceMajorAndMinorContours()
        {
            using (Transaction tr = startTransaction())
            {
                ITerrainSurface surface = getSurface() as ITerrainSurface;
                if (surface == null) return;
                
                extractMajorContours(surface);
                extractMinorContours(surface);

                tr.Commit();
                
                
            }
        }

        

        private void extractMajorContours(ITerrainSurface surface)
        {
            ObjectIdCollection contours = surface.ExtractMajorContours(
                SurfaceExtractionSettingsType.Model);                
            customizeContours(contours, _majorContoursColor);
        }

        private void extractMinorContours(ITerrainSurface surface)
        {
            ObjectIdCollection contours = surface.ExtractMinorContours(
                SurfaceExtractionSettingsType.Model);
            customizeContours(contours, _minorContoursColor);
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

        private CivilSurface getSurface()
        {
            ObjectId surfaceId = promptForSurface();
            if (surfaceId.IsNull)
            {
                return null;
            }
            CivilSurface surface = surfaceId.GetObject(OpenMode.ForRead) 
                as CivilSurface;
            showGeneralProperties(surface);
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

        private void showGeneralProperties(CivilSurface surface)
        {
            _editor.WriteMessage("\nSurface name: " + surface.Name);
            GeneralSurfaceProperties properties = 
                surface.GetGeneralProperties();
            _editor.WriteMessage("\n- Max elevation: " +
                properties.MaximumElevation);
            _editor.WriteMessage("\n- Min elevation: " +
                properties.MinimumElevation);
        }

        private double getDouble(string valueName)
        {
            double result = Double.NaN;
            string msg = String.Format("\nEnter value for {0}:", valueName);
            PromptDoubleResult res = _editor.GetDouble(msg);
            if (res.Status == PromptStatus.OK)
            {
                result = res.Value;
            }
            return result;
        }

        private AutoCAD.Colors.Color _majorContoursColor =
            AutoCAD.Colors.Color.FromRgb(0, 0, 255);
        private AutoCAD.Colors.Color _minorContoursColor =
            AutoCAD.Colors.Color.FromRgb(0, 255, 255);
        private AutoCAD.Colors.Color _rangedContoursColor =
            AutoCAD.Colors.Color.FromRgb(255, 0, 0);
        private AutoCAD.Colors.Color _intervalContoursColor =
            AutoCAD.Colors.Color.FromRgb(0, 255, 0);
        private AutoCAD.Colors.Color _singleContourColor =
            AutoCAD.Colors.Color.FromRgb(255, 255, 0);
    }
}