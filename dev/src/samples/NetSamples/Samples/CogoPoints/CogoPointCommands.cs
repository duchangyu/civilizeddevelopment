using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Settings;

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

        [CommandMethod("CDS_OffsetPointElevations")]
        public void CDS_OffsetPointElevations()
        {
            PromptDoubleResult result = _editor.GetDouble(
                "\nEnter elevation offset: ");
            if (result.Status == PromptStatus.OK)
            {
                double offset = result.Value;
                CogoPointCollection points = _civildoc.CogoPoints;
                points.SetElevationByOffset(points, offset);
            }
        }

        [CommandMethod("CDS_NaiveRenumberPoint")]
        public void CDS_NaiveRenumberPoint()
        {
            PromptIntegerResult result = _editor.GetInteger(
                "\nEnter point to renumber:");
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            uint currentPointNumber = (uint)result.Value;

            result = _editor.GetInteger("\nEnter new point number:");
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            uint newPointNumber = (uint)result.Value;

            CogoPointCollection points = _civildoc.CogoPoints;
            ObjectId pointId = points.GetPointByPointNumber(currentPointNumber);
            points.SetPointNumber(pointId, newPointNumber);
        }
        
        [CommandMethod("CDS_RenumberPoint")]
        public void CDS_RenumberPoint()
        {
            PromptIntegerResult result = _editor.GetInteger(
                "\nEnter point to renumber:");
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            uint currentPointNumber = (uint)result.Value;

            result = _editor.GetInteger("\nEnter new point number (hint):");
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            uint pointNumberHint = (uint)result.Value;

            try
            {
                CogoPointCollection points = _civildoc.CogoPoints;
                ObjectId pointId = 
                    points.GetPointByPointNumber(currentPointNumber);
                points.SetPointNumber(pointId, getNextPointNumberAvailable(pointNumberHint));
            }
            catch (ArgumentException ex)
            {
                _editor.WriteMessage(ex.Message);
            }
           
        }

        [CommandMethod("CDS_CreateRandomPointsAtSpecifiedNumber")]
        public void CDS_CreateRandomPointsAtSpecifiedNumber()
        {
            PromptIntegerResult result = _editor.GetInteger(
                "\nEnter number of points to generate: ");
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            int numberOfPoints = result.Value;

            result = _editor.GetInteger(
                "\nEnter base number for first point: ");
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            uint basePoint = (uint)result.Value;

            ObjectIdCollection createdIds = createPoints(numberOfPoints);
            renumberPoints(createdIds, basePoint);
        }

        [CommandMethod("CDS_CreateRandomPointsAtSpecifiedNumberByFactor")]
        public void CDS_CreateRandomPointsAtSpecifiedNumberByFactor()
        {
            PromptIntegerResult result = _editor.GetInteger(
                "\nEnter number of points to generate: ");
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            int numberOfPoints = result.Value;

            result = _editor.GetInteger(
                "\nEnter base number for first point: ");
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            uint basePoint = (uint)result.Value;

            ObjectIdCollection createdIds = createPoints(numberOfPoints);
            uint firstCreatedPointNumber = getPointNumberFor(createdIds[0]);
            int additiveFactor = (int)(basePoint - firstCreatedPointNumber);
            CogoPointCollection points = _civildoc.CogoPoints;
            points.SetPointNumber(ToEnumerable(createdIds), additiveFactor);
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
        
        private uint getNextPointNumberAvailable(uint hint)
        {
            uint suggested = hint;
            CogoPointCollection points = _civildoc.CogoPoints;
            while (points.Contains(suggested) && suggested < _maxPointNumber)
            {
                suggested++;
            }

            if (suggested == _maxPointNumber)
            {
                string msg = String.Format(
                    "No available point number at {0} or greater value.", 
                    hint);
                throw new ArgumentException(msg);
            }

            return suggested;
        }

        private ObjectIdCollection createPoints(int numberOfPoints)
        {
            
            RandomCoordinateGenerator generator = 
                new RandomCoordinateGenerator();
            Point3dCollection coordinates = 
                generator.GetCoordinates(numberOfPoints);
            CogoPointCollection points = _civildoc.CogoPoints;
            _creationSet++;
            string description = String.Format("Creation {0}", _creationSet);
            
            return points.Add(coordinates, description);
        }

        private void renumberPoints(ObjectIdCollection pointIds, uint basePoint)
        {
            CogoPointCollection points = _civildoc.CogoPoints;
            uint suggested = basePoint;
            foreach (ObjectId pointId in pointIds)
            {
                suggested = getNextPointNumberAvailable(suggested);
                points.SetPointNumber(pointId, suggested);
                suggested++;
            }
        }

        private uint getPointNumberFor(ObjectId pointId)
        {
            using (Transaction tr = startTransaction())
            {
                CogoPoint point = pointId.GetObject(OpenMode.ForRead)
                    as CogoPoint;
                return point.PointNumber;
            }
        }

        private IEnumerable<ObjectId> ToEnumerable(ObjectIdCollection ids)
        {
            foreach (ObjectId id in ids)
            {
                yield return id;
            }
        }

        private readonly uint _maxPointNumber = UInt32.MaxValue;
        static int _creationSet = 0;
    }
}