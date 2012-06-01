using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.DescriptionKeyCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class DescriptionKeyCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_CreateCivilDevDescriptionKeys")]
        public void CDS_CreateCivilDevDescriptionKeys()
        {
            using (Transaction tr = startTransaction())
            {
                ObjectId keySetId = createOrRetrieveDescriptionKeySet("CivilDev");
                PointDescriptionKeySet keySet = 
                    keySetId.GetObject(OpenMode.ForWrite) 
                    as PointDescriptionKeySet;

                ObjectId treesKeyId = createDescriptionKey(keySet, "TREE");
                PointDescriptionKey key = 
                    treesKeyId.GetObject(OpenMode.ForWrite) 
                    as PointDescriptionKey;
                customizeDescriptionKey(key, "Tree", "Description Only",
                    "$0 is a $1");

                ObjectId wellsKeyId = createDescriptionKey(keySet, "WELL");
                key = wellsKeyId.GetObject(OpenMode.ForWrite) 
                    as PointDescriptionKey;
                customizeDescriptionKey(key, "Well", "Point Number Only",
                    "The $0 is $1");

                tr.Commit();
            }
        }

        [CommandMethod("CDS_CreateCivilDevDescriptionKeysAndForce")]
        public void CDS_CreateCivilDevDescriptionKeysAndForce()
        {
            using (Transaction tr = startTransaction())
            {
                ObjectId keySetId = createOrRetrieveDescriptionKeySetAndForce("CivilDev");
                PointDescriptionKeySet keySet =
                    keySetId.GetObject(OpenMode.ForWrite)
                    as PointDescriptionKeySet;

                ObjectId treesKeyId = createDescriptionKey(keySet, "TREE");
                PointDescriptionKey key =
                    treesKeyId.GetObject(OpenMode.ForWrite)
                    as PointDescriptionKey;
                customizeDescriptionKey(key, "Tree", "Description Only",
                    "$0 is a $1");

                ObjectId wellsKeyId = createDescriptionKey(keySet, "WELL");
                key = wellsKeyId.GetObject(OpenMode.ForWrite)
                    as PointDescriptionKey;
                customizeDescriptionKey(key, "Well", "Point Number Only",
                    "The $0 is $1");

                tr.Commit();
            }
        }

        [CommandMethod("CDS_CreateCivilDevPoints")]
        public void CDS_CreateCivilDevPoints()
        {
            RandomCoordinateGenerator generator =
                new RandomCoordinateGenerator();
            CogoPointCollection points = _civildoc.CogoPoints;

            Point3dCollection coordinates = generator.GetCoordinates(10);
            points.Add(coordinates, "TREE MAPLE", true, true);

            coordinates = generator.GetCoordinates(10);
            points.Add(coordinates, "WELL DEEP", true, true);
        }

        private ObjectId createOrRetrieveDescriptionKeySet(string name)
        {
            PointDescriptionKeySetCollection keySets =
                PointDescriptionKeySetCollection
                .GetPointDescriptionKeySets(_database);
            if (keySets.Contains(name))
            {
                return keySets[name];
            }
            return keySets.Add(name);
        }

        private ObjectId createOrRetrieveDescriptionKeySetAndForce(string name)
        {
            PointDescriptionKeySetCollection keySets =
                PointDescriptionKeySetCollection
                .GetPointDescriptionKeySets(_database);
            ObjectIdCollection searchOrder = keySets.SearchOrder;
            if (keySets.Contains(name))
            {
                return keySets[name];
            }
            ObjectId newKeySetId = keySets.Add(name);
            searchOrder.Insert(0, newKeySetId);
            keySets.SearchOrder = searchOrder;
            return newKeySetId;
        }

        private ObjectId createDescriptionKey(
            PointDescriptionKeySet keySet, string code)
        {
            if (keySet.Contains(code))
            {
                return keySet[code];
            }
            return keySet.Add(code);
        }

        private void customizeDescriptionKey(PointDescriptionKey key,
            string pointStyle, string labelStyle, string format)
        {
            key.StyleId = getPointStyle(pointStyle);
            key.LabelStyleId = getPointLabelStyle(labelStyle);
            key.Format = format;
            key.ApplyStyleId = true;
            key.ApplyLabelStyleId = true;
        }

        private ObjectId getPointStyle(string name)
        {
            return _civildoc.Styles.PointStyles.GetStyle(name);
        }

        private ObjectId getPointLabelStyle(string name)
        {
            return _civildoc.Styles.LabelStyles
                .PointLabelStyles.LabelStyles.GetStyle(name);
        }
    }
}