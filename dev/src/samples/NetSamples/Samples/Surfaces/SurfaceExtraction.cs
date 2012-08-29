using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;


[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.SurfaceExtractionCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class SurfaceExtractionCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_ExtractGrid")]
        public void CDS_ExtractGrid()
        {
            ObjectId surfaceId = promptForTinSurface();
            if (surfaceId == ObjectId.Null)
            {
                write("\nNo TIN Surface selected.");
                return;
            }

            using (Transaction tr = startTransaction())
            {
                TinSurface surface = surfaceId.GetObject(OpenMode.ForRead)
                    as TinSurface;
                ObjectIdCollection ids = surface.ExtractGridded(
                    SurfaceExtractionSettingsType.Model);
                
                foreach (ObjectId id in ids)
                {
                    Polyline3d polyline =
                        id.GetObject(OpenMode.ForWrite) as Polyline3d;
                    if (polyline != null)
                    {                        
                        using (polyline)
                        {
                            polyline.Color = 
                                AutoCAD.Colors.Color.FromRgb(255, 0, 0);
                        }
                    }
                }
                tr.Commit();
            }
        }

        [CommandMethod("CDS_FindClosestPointOnSurface")]
        public void CDS_FindClosestPointOnSurface()
        {
            ObjectId surfaceId = promptForTinSurface();
            if (surfaceId.IsNull)
            {
                write("\nNo TIN Surface selected.");
                return;
            }

            PromptPointResult result = _editor.GetPoint(
                "\nSelect point outside surface: ");
            if (result.Status != PromptStatus.OK)
            {
                write("\nNo point selected.");
                return;
            }

            Point3d selectedPoint = result.Value;
            Point3d closestPointFound = Point3d.Origin;
            double shortestDistanceSoFar = Double.MaxValue;

            using (Transaction tr = startTransaction())
            {
                TinSurface surface = surfaceId.GetObject(OpenMode.ForRead) 
                    as TinSurface;
                ObjectIdCollection borders = surface.ExtractBorder(
                    SurfaceExtractionSettingsType.Model);
                foreach (ObjectId borderId in borders)
                {
                    
                    Polyline3d border = borderId.GetObject(OpenMode.ForRead) 
                        as Polyline3d;
                    Point3d closestToBorder = 
                        border.GetClosestPointTo(selectedPoint, false);
                    double distance = selectedPoint.DistanceTo(closestToBorder);
                    if (distance < shortestDistanceSoFar)
                    {
                        closestPointFound = closestToBorder;
                        shortestDistanceSoFar = distance;
                    }
                }
            }

            using (Transaction tr = startTransaction())
            {
                BlockTableRecord btr = tr.GetObject(_database.CurrentSpaceId, 
                    OpenMode.ForWrite) as BlockTableRecord;
                Line line = new Line(selectedPoint, closestPointFound);
                btr.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                tr.Commit();
            }
        }

        private ObjectId promptForTinSurface()
        {
            PromptEntityOptions options = new PromptEntityOptions(
                "\nSelect a TIN Surface: ");
            options.SetRejectMessage(
                "\nThe selected object is not a TIN Surface.");
            options.AddAllowedClass(typeof(TinSurface), true);

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