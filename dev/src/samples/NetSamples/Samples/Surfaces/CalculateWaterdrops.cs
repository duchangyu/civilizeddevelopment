using System;

using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.Land.DatabaseServices;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.CalculateWaterdrops))]

namespace Autodesk.CivilizedDevelopment
{
    public class CalculateWaterdrops : SimpleDrawingCommand
    {
        [CommandMethod("CDS_CalculateWaterdrops")]
        public void CDS_CalculateWaterdrops()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            using (Transaction ts = startTransaction())
            {
                ObjectId surfaceId = promptForTinSurface();
                TinSurface surface = surfaceId.GetObject(OpenMode.ForWrite) 
                    as TinSurface;

                
                ObjectIdCollection drops = new ObjectIdCollection();
                foreach (TinSurfaceTriangle triangle in surface.Triangles)
                {
                    Point2d centroid = getTriangleCentroid(triangle);
                    // calculate water-drop for the centroid
                    ObjectIdCollection oid = 
                        surface.Analysis.CreateWaterdrop(centroid, 
                        Autodesk.Civil.Land.WaterdropObjectType.Polyline3D);
                    // Save all the water-drops
                    foreach (ObjectId id in oid)
                    {
                        drops.Add(id);
                    }
                }

                // calculate the longest waterdrop, then filter out 
                // anything that is 25% the longest or less. Those
                // are the sinks.
                double longest = calculateLongestWaterDrop(drops);
                
                // Filter out the sinks
                Point3dCollection sinks = filterSinks(drops, longest);
                markSinks(ts, sinks);
                
            ts.Commit();
            }

        }

        private Point2d getTriangleCentroid(TinSurfaceTriangle triangle)
        {
            // The centroid is calculated from the cx and cy being:
            // cx = (v1x + v2x + v3x) / 3
            // cy = (v1y + v2y + v3y) / 3
            double cx = (triangle.Vertex1.Location.X 
                + triangle.Vertex2.Location.X 
                + triangle.Vertex3.Location.X) 
                / 3;
            double cy = (triangle.Vertex1.Location.Y 
                + triangle.Vertex2.Location.Y 
                + triangle.Vertex3.Location.Y) 
                / 3;
            return new Point2d(cx, cy);
        }

        private double calculateLongestWaterDrop(ObjectIdCollection drops)
        {
            double longest = 0;
            foreach (ObjectId id in drops)
            {
                Polyline3d drop = id.GetObject(OpenMode.ForRead) 
                    as Polyline3d;
                longest = drop.Length > longest ? drop.Length : longest;
            }
            return longest;
        }

        private Point3dCollection filterSinks(ObjectIdCollection drops, 
            double longest)
        {
            Point3dCollection sinks = new Point3dCollection();
            foreach (ObjectId id in drops)
            {
                Polyline3d drop = id.GetObject(OpenMode.ForRead) 
                    as Polyline3d;
                if (drop.Length > (longest * .25))
                {
                    sinks.Add(drop.EndPoint);
                    string msg = String.Format(
                        "Sink located at: ({0},{1})\n", 
                        drop.EndPoint.X, drop.EndPoint.Y);
                    write(msg);
                }
            }

            return sinks;
        }

        private void markSinks(Transaction ts, Point3dCollection sinks)
        {
            // now lets mark each endpoint
            BlockTable acBlkTbl = _database.BlockTableId
                .GetObject(OpenMode.ForRead) as BlockTable;
            BlockTableRecord acBlkTblRec = 
                acBlkTbl[BlockTableRecord.ModelSpace]
                .GetObject(OpenMode.ForWrite) as BlockTableRecord;
            // set the point style
            _document.Database.Pdmode = 35;
            _document.Database.Pdsize = 10;
            foreach (Point3d sink in sinks)
            {
                DBPoint sinkPoint = new DBPoint(sink);
                sinkPoint.Color = Color.FromRgb(0, 255, 255);
                sinkPoint.SetDatabaseDefaults();
                acBlkTblRec.AppendEntity(sinkPoint);
                ts.AddNewlyCreatedDBObject(sinkPoint, true);
            }
        }

        private ObjectId promptForTinSurface()
        {
            string promptMsg = "\nSelect TIN Surface: ";
            string rejectMsg = "\nSelected entity is not a TIN Surface.";
            PromptEntityOptions opts = new PromptEntityOptions(promptMsg);
            opts.SetRejectMessage(rejectMsg);
            opts.AddAllowedClass(typeof(TinSurface), false);
            PromptEntityResult result = _editor.GetEntity(opts);
            if (result.Status == PromptStatus.OK)
            {
                return result.ObjectId;
            }
            return ObjectId.Null;
        }
    }
}