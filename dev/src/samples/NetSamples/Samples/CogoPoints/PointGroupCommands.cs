using System;

using Autodesk.AutoCAD.DatabaseServices;
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

        private void createPointGroup(string name, string includeRawDescription)
        {
            ObjectId groupId = _pointGroups.Add(name);
            StandardPointGroupQuery query = new StandardPointGroupQuery();
            query.IncludeRawDescriptions = includeRawDescription;
            PointGroup group = groupId.GetObject(OpenMode.ForRead) 
                as PointGroup;
            group.SetQuery(query);
        }

        private PointGroupCollection _pointGroups
        {
            get
            {
                return _civildoc.PointGroups;
            }
        }
    }
};