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
                ObjectId groupId = _civildoc.PointGroups.Add("Trees");
                PointGroup group = groupId.GetObject(OpenMode.ForRead)
                    as PointGroup;
                StandardPointGroupQuery query = new StandardPointGroupQuery();
                query.IncludeRawDescriptions = "TREE*";
                group.SetQuery(query);

                groupId = _civildoc.PointGroups.Add("Wells");
                group = groupId.GetObject(OpenMode.ForRead) as PointGroup;
                query.IncludeRawDescriptions = "WELL*";
                group.SetQuery(query);

                tr.Commit();
            }
        }
    }
}