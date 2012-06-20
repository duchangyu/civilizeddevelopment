using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;


[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.PointGroupCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class PointGroupCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_CreateCivilDevPointGroups")]
        public void CDS_CreateCivilDevPointGroups()
        {
            using (Transaction tr = startTransaction())
            {
                createPointGroup("Trees", "TREE*");
                createPointGroup("Wells", "WELL*");
                tr.Commit();
            }
        }

        [CommandMethod("CDS_ReversPointGroupDrawOrder")]
        public void CDS_ReversPointGroupDrawOrder()
        {
            ObjectIdCollection drawPriority = _pointGroups.DrawOrder;
            ObjectIdCollection reversed = new ObjectIdCollection();
            for (int i = drawPriority.Count - 1; i >= 0; i-- )
            {
                ObjectId current = drawPriority[i];
                reversed.Add(current);
            }
            _pointGroups.DrawOrder = reversed;
        }

        [CommandMethod("CDS_RenumberPointsForGroup")]
        public void CDS_RenumberPointsForGroup()
        {
            string pointGroupName = getPointGroupName();
            if (pointGroupName == String.Empty)
            {
                return;
            }

            int baseNumber = getNewBaseNumber();
            if (baseNumber == _kNoNumber)
            {
                return;
            }

            renumberPointsForGroup(pointGroupName, baseNumber);
        }

        [CommandMethod("CDS_CreatePointGroupsWithQueries")]
        public void CDS_CreatePointGroupsWithQueries()
        {
            using (Transaction tr = startTransaction())
            {
                createStandardPointGroup();
                createCustomPointGroup();
                tr.Commit();
            }
        }

        private void createStandardPointGroup()
        {
            StandardPointGroupQuery standard =
                    new StandardPointGroupQuery();
            standard.IncludeElevations = ">100.5";
            standard.IncludeFullDescriptions = "Contains*";
            standard.IncludeNames = "Tree Cedar*";
            standard.IncludeNumbers = "<1000";
            standard.IncludeRawDescriptions = "TREE*";
            standard.ExcludeElevations = ">300.0";
            standard.ExcludeFullDescriptions = "Contains POLE*";
            standard.ExcludeNames = "Tree maples*";
            standard.ExcludeNumbers = "<200";
            standard.ExcludeRawDescriptions = "POLE*";
            createPointGroup("Standard Group", standard);
        }

        private void createCustomPointGroup()
        {
            string queryString =
                    @"(PointNumber>=100 AND PointNumber<200) OR 
                        (FullDescription='Contains*' OR RawDescription='WE*')";
            CustomPointGroupQuery custom = new CustomPointGroupQuery();
            custom.QueryString = queryString;
            createPointGroup("Custom Group", custom);
        }

        private void createPointGroup(string name, PointGroupQuery query)
        {
            if (_pointGroups.Contains(name))
            {
                return;
            }
            ObjectId groupId = _pointGroups.Add(name);
            PointGroup group = groupId.GetObject(OpenMode.ForRead)
                as PointGroup;
            group.SetQuery(query);
        }

        private void createPointGroup(string name, 
            string includeRawDescription)
        {
            if (_pointGroups.Contains(name))
            {
                return;
            }
            ObjectId groupId = _pointGroups.Add(name);
            StandardPointGroupQuery query = new StandardPointGroupQuery();
            query.IncludeRawDescriptions = includeRawDescription;
            PointGroup group = groupId.GetObject(OpenMode.ForWrite) 
                as PointGroup;
            group.SetQuery(query);
            // NOTE:    Setting a description throws an exception. The
            //          issue is being researched by the API team.
            // group.Description = "Set a description";
        }

        private PointGroupCollection _pointGroups
        {
            get
            {
                return _civildoc.PointGroups;
            }
        }

        private string getPointGroupName()
        {
            PromptResult result = _editor.GetString(
                "\nEnter point group name: ");
            if (result.Status == PromptStatus.OK)
            {
                return result.StringResult;
            }
            return String.Empty;
        }

        private int getNewBaseNumber()
        {
            PromptIntegerResult result = _editor.GetInteger(
                "\nEnter new base number: ");
            if (result.Status == PromptStatus.OK)
            {
                return result.Value;
            }

            return _kNoNumber;
        }

        private void renumberPointsForGroup(string groupName, int baseNumber)
        {
            using (Transaction tr = startTransaction())
            {
                ObjectId pointGroupId = getPointGroupIdByName(groupName);
                PointGroup group = pointGroupId.GetObject(OpenMode.ForRead)
                    as PointGroup;
                renumberPoints(group, baseNumber);
                tr.Commit();
            }
        }

        private ObjectId getPointGroupIdByName(string groupName)
        {
            return _pointGroups[groupName];
        }

        private void renumberPoints(PointGroup group, int baseNumber)
        {
            uint[] pointNumbers = group.GetPointNumbers();
            int firstNumber = (int)pointNumbers[0];
            int factor = baseNumber - firstNumber;
            _civildoc.CogoPoints.SetPointNumber(
                ToEnumerableObjectId(group.GetPointNumbers()), factor);
            group.Update();
        }

        private IEnumerable<ObjectId> ToEnumerableObjectId(uint[] numbers)
        {
            foreach(uint number in numbers)
            {
                yield return _civildoc.CogoPoints
                    .GetPointByPointNumber(number);
            }
        }

        private const int _kNoNumber = -1;
    }
};