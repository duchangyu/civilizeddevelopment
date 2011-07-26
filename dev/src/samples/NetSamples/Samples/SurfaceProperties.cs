using System;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.Land.DatabaseServices;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.SurfacePropertiesDemo))]

namespace Autodesk.CivilizedDevelopment
{
    public class SurfacePropertiesDemo
    {
        [CommandMethod("CDS_TinSurfacePropertiesDemo")]
        public void CDS_TinSurfacePropertiesDemo()
        {
            ObjectId surfaceId = promptForTinSurface();
            if (ObjectId.Null == surfaceId)
            {
                _write("\nNo TIN Surface object was selected.");
                return; // We don't have a surface; we can't continue.
            }

            using (Transaction tr = startTransaction())
            {
                TinSurface surface = surfaceId.GetObject(
                    OpenMode.ForRead) as TinSurface;
                _write("\nInformation for TIN Surface: " + surface.Name);
                writeGeneralProperites(surface.GetGeneralProperties());
                writeTerrainProperties(surface.GetTerrainProperties());
                writeTinSurfaceProperties(surface.GetTinProperties());
            }
        }

        /// <summary>
        /// Prompts the user to select a TIN Surface, and returns its
        /// ObjectId.
        /// </summary>
        /// <returns>The method returns the ObjectId of the selected
        /// surface, or ObjectId.Null if no surface was selected (usually,
        /// because the user canceled the operation).
        /// </returns>
        private ObjectId promptForTinSurface()
        {
            PromptEntityOptions options = new PromptEntityOptions(
                "\nSelect a TIN Surface: ");
            options.SetRejectMessage(
                "\nThe selected object is not a TIN Surface.");
            options.AddAllowedClass(typeof(TinSurface), true);

            PromptEntityResult result = _editor.GetEntity(options);
            if (result.Status == PromptStatus.OK)
            {
                // Everything is cool; we return the selected
                // surface ObjectId.
                return result.ObjectId;
            }
            return ObjectId.Null;   // Indicating error.
        }

        /// <summary>
        /// Displays general properties that apply to all Surface objects.
        /// </summary>
        /// <param name="props">General properties from surface.</param>
        private void writeGeneralProperites(GeneralSurfaceProperties p)
        {
            _write("\nGeneral Properties:");
            _write("\n-------------------");
            _write("\nMin X: " + p.MinimumCoordinateX);
            _write("\nMin Y: " + p.MinimumCoordinateY);
            _write("\nMin Z: " + p.MinimumElevation);
            _write("\nMax X: " + p.MaximumCoordinateX);
            _write("\nMax Y: " + p.MaximumCoordinateY);
            _write("\nMax Z: " + p.MaximumElevation);
            _write("\nMean Elevation: " + p.MeanElevation);
            _write("\nNumber of Points: " + p.NumberOfPoints);
            _write("\n--");
        }

        private void writeTerrainProperties(TerrainSurfaceProperties p)
        {
            _write("\nTerrain Surface Properties:");
            _write("\n---------------------------");
            _write("\nMin Grade/Slope: " + p.MinimumGradeOrSlope);
            _write("\nMax Grade/Slope: " + p.MaximumGradeOrSlope);
            _write("\nMean Grade/Slope: " + p.MeanGradeOrSlope);
            _write("\n2D Area: " + p.SurfaceArea2D);
            _write("\n3D Area: " + p.SurfaceArea3D);
            _write("\n--");

        }

        private void writeTinSurfaceProperties(TinSurfaceProperties p)
        {
            _write("\nTIN Surface Properties:");
            _write("\n-----------------------");
            _write("\nMin Triangle Area: " + p.MinimumTriangleArea);
            _write("\nMin Triangle Length: " + p.MinimumTriangleLength);
            _write("\nMax Triangle Area: " + p.MaximumTriangleArea);
            _write("\nMax Triangle Length: " + p.MaximumTriangleLength);
            _write("\nNumber of Triangles: " + p.NumberOfTriangles);
            _write("\n--");
        }

        /// <summary>
        /// Returns an instance to the document that initiated the
        /// command.
        /// </summary>
        private Document _document
        {
            get
            {
                if (null == m_ActiveDocument)
                {
                    m_ActiveDocument = 
                        Application.DocumentManager.MdiActiveDocument;
                }
                return m_ActiveDocument;
            }
        }

        /// <summary>
        /// Returns the document's editor instance.
        /// </summary>
        private Editor _editor
        {
            get
            {
                return _document.Editor;
            }
        }

        /// <summary>
        /// Creates a new database transaction that allows to open,
        /// read, and modified objects in the database.
        /// </summary>
        /// <returns>Returns a new database transaction.</returns>
        private Transaction startTransaction()
        {
            return _document.TransactionManager.StartTransaction();
        }

        /// <summary>
        /// Writes the specified message to the AutoCAD command-line
        /// output.
        /// </summary>
        /// <param name="msg">String output.</param>
        private void _write(string msg)
        {
            _editor.WriteMessage(msg);
        }

        private Document m_ActiveDocument = null;

    }
}