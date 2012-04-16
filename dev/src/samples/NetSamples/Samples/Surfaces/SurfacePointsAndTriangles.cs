using System;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;

using Colibra;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.SurfacePointsAndTriangles))]

namespace Autodesk.CivilizedDevelopment
{
    public class SurfacePointsAndTriangles : SimpleDrawingCommand
    {
        [CommandMethod("CDS_SurfaceTrianglesAndPointsDemo")]
        public void CDS_SurfaceTrianglesAndPointsDemo()
        {
            Colibra.Document doc = DocumentManager.ActiveDocument;
            ObjectId surfaceId = promptForSurface(doc);
            if (surfaceId == ObjectId.Null)
            {
                write("\nNo TIN surface selected.");
                return;
            }

            using(Colibra.Transaction tr = doc.StartTransaction())
            {
                TinSurface surface = surfaceId.GetObject(OpenMode.ForRead) 
                    as TinSurface;

                foreach (TinSurfaceVertex v in surface.Vertices)
                {
                    displayVertex(v);
                }
            }
        }


        private ObjectId promptForSurface(Colibra.Document doc)
        {
            SingleObjectSelector<TinSurface> selector =
                new SingleObjectSelector<TinSurface>();
            selector.PromptMessage = "\nSelect TIN surface: ";
            selector.RejectMessage =
                "\nThe selected object is not a TIN surface.";
            selector.Select(doc);
            return selector.SelectedId;
        }

        private void displayVertex(TinSurfaceVertex vertex)
        {
            string msg = String.Format("\nVertex: ({0},{1},{2})",
                vertex.Location.X, vertex.Location.Y, vertex.Location.Z);
            write(msg);
        }
    }
}