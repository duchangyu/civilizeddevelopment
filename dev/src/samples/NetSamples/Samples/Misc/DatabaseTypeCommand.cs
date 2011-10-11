using System;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Autodesk.CivilizedDevelopment.DatabaseTypeCommand))]

namespace Autodesk.CivilizedDevelopment
{
    public class DatabaseTypeCommand : SimpleDrawingCommand
    {
        [CommandMethod("CDS_ShowDatabaseType")]
        public void CDS_ShowDatabaseType()
        {
            if (isCivilDatabase(_database))
            {
                write("\nCurrent document is a Civil 3D drawing.");
            }
            else
            {
                write("\nCurrent document is an AutoCAD drawing.");
            }
        }

        public bool isCivilDatabase(Database db)
        {
            using (Transaction tr = startTransaction())
            {
                DBDictionary namedObjectDict = db.NamedObjectsDictionaryId
                    .GetObject(OpenMode.ForRead) as DBDictionary;
                return namedObjectDict.Contains("Root");
            }
        }
    }
}