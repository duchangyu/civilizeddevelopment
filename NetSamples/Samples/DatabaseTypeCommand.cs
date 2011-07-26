using System;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Autodesk.CivilizedDevelopment.DatabaseTypeCommand))]

namespace Autodesk.CivilizedDevelopment
{
    public class DatabaseTypeCommand
    {
        [CommandMethod("CDS_ShowDatabaseType")]
        public void CDS_ShowDatabaseType()
        {
            if (isCivilDatabase(_currentDb))
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

        private Document _currentDoc
        {
            get
            {
                return Application.DocumentManager.MdiActiveDocument;
            }
        }

        private Database _currentDb
        {
            get
            {
                return _currentDoc.Database;
            }
        }

        private Editor _editor
        {
            get
            {
                return _currentDoc.Editor;
            }
        }

        private Transaction startTransaction()
        {
            return _currentDb.TransactionManager.StartTransaction();
        }

        private void write(string message)
        {
            _editor.WriteMessage(message);
        }
    }
}