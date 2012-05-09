using System;

using Autodesk.AutoCAD.Geometry;

namespace Autodesk.CivilizedDevelopment
{
    public class RandomCoordinateGenerator
    {
        public RandomCoordinateGenerator()
        {
            MaxX = 1000.0;
            MaxY = 1000.0;
            MaxZ = 100.0;
            initGenerator();
        }

        public RandomCoordinateGenerator(double maxx, double maxy, double maxz)
        {
            MaxX = maxx;
            MaxY = maxy;
            MaxZ = maxz;
            initGenerator();
        }

        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double MaxZ { get; set; }

        public Point3d GetCoordinate()
        {
            double x = m_Generator.NextDouble() * MaxX;
            double y = m_Generator.NextDouble() * MaxY;
            double z = m_Generator.NextDouble() * MaxZ;
            return new Point3d(x, y, z);
        }

        public Point3dCollection GetCoordinates(int count)
        {
            Point3dCollection result = new Point3dCollection();
            for (int i = 0; i < count; i++)
            {
                result.Add(GetCoordinate());
            }

            return result;
        }


        private void initGenerator()
        {
            int seed = (int)DateTime.Now.Ticks;
            m_Generator = new Random(seed);
        }

        private Random m_Generator;
    }
}