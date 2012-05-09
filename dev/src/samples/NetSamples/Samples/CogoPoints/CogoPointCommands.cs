using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.CogoPointCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class CogoPointCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_CreateRandomPoints")]
        public void CDS_CreateRandomPoints()
        {
            RandomCoordinateGenerator generator = 
                new RandomCoordinateGenerator();
            CogoPointCollection points = _civildoc.CogoPoints;

            // From Point3d
            Point3d coordinate = generator.GetCoordinate();
            ObjectId pointId = points.Add(coordinate);
            write("\nSingle Point from coordinate.");
            display(pointId);

            coordinate = generator.GetCoordinate();
            pointId = points.Add(coordinate, "Sample description.");
            write("\nSingle Point from coordinate and description.");
            display(pointId);

            coordinate = generator.GetCoordinate();
            pointId = points.Add(coordinate, "Sample description",  
                true, false);
            write("\nSingle Point from coordinate with description, "
                + "using description key, and not matching parameters.");
            display(pointId);

            // From Point3dCollection
            Point3dCollection coordinates = generator.GetCoordinates(10);
            ObjectIdCollection pointIds = points.Add(coordinates);
            write("\nPoints from coordinate collection.");
            display(pointIds);

            coordinates = generator.GetCoordinates(5);
            pointIds = points.Add(coordinates, "Group of 5");
            write("\nPoints from coordinate collection with description.");
            display(pointIds);

            coordinates = generator.GetCoordinates(7);
            pointIds = points.Add(coordinates, "Group of 7", true, true);
            write("\nPoints from coordinate collection with description,"
                + "using description key, and not matching parameters.");
            display(pointIds);
        }

        private void display(ObjectId pointId)
        {
            using (Transaction tr = startTransaction())
            {
                CogoPoint point = pointId.GetObject(OpenMode.ForRead) 
                    as CogoPoint;
                displayPointInfo(point);
            }
        }

        private void display(ObjectIdCollection pointIds)
        {
            using (Transaction tr = startTransaction())
            {
                foreach (ObjectId pointId in pointIds)
                {
                    CogoPoint point = pointId.GetObject(OpenMode.ForRead)
                        as CogoPoint;
                    displayPointInfo(point);
                }
            }
        }

        private void displayPointInfo(CogoPoint point)
        {
            write("\nPoint Number: " + point.PointNumber.ToString());
            write("\n- Location: " + point.Location.ToString());
            write("\n- - Northing: " + point.Northing.ToString());
            write("\n- - Easting: " + point.Easting.ToString());
            write("\n- - Elevation: " + point.Elevation.ToString());
            write("\n- Description: " + point.FullDescription);
            write("\n- Raw Description: " + point.RawDescription);
        }
    }
}