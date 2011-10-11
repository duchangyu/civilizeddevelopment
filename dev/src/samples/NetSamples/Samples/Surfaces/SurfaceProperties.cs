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
    public class SurfacePropertiesDemo : SimpleDrawingCommand
    {
        [CommandMethod("CDS_TinSurfacePropertiesDemo")]
        public void CDS_TinSurfacePropertiesDemo()
        {
            ObjectId surfaceId = promptForTinSurface();
            if (ObjectId.Null == surfaceId)
            {
                write("\nNo TIN Surface object was selected.");
                return; // We don't have a surface; we can't continue.
            }

            using (Transaction tr = startTransaction())
            {
                TinSurface surface = surfaceId.GetObject(
                    OpenMode.ForRead) as TinSurface;
                write("\nInformation for TIN Surface: " + surface.Name);
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
            write("\nGeneral Properties:");
            write("\n-------------------");
            write("\nMin X: " + p.MinimumCoordinateX);
            write("\nMin Y: " + p.MinimumCoordinateY);
            write("\nMin Z: " + p.MinimumElevation);
            write("\nMax X: " + p.MaximumCoordinateX);
            write("\nMax Y: " + p.MaximumCoordinateY);
            write("\nMax Z: " + p.MaximumElevation);
            write("\nMean Elevation: " + p.MeanElevation);
            write("\nNumber of Points: " + p.NumberOfPoints);
            write("\n--");
        }

        private void writeTerrainProperties(TerrainSurfaceProperties p)
        {
            write("\nTerrain Surface Properties:");
            write("\n---------------------------");
            write("\nMin Grade/Slope: " + p.MinimumGradeOrSlope);
            write("\nMax Grade/Slope: " + p.MaximumGradeOrSlope);
            write("\nMean Grade/Slope: " + p.MeanGradeOrSlope);
            write("\n2D Area: " + p.SurfaceArea2D);
            write("\n3D Area: " + p.SurfaceArea3D);
            write("\n--");

        }

        private void writeTinSurfaceProperties(TinSurfaceProperties p)
        {
            write("\nTIN Surface Properties:");
            write("\n-----------------------");
            write("\nMin Triangle Area: " + p.MinimumTriangleArea);
            write("\nMin Triangle Length: " + p.MinimumTriangleLength);
            write("\nMax Triangle Area: " + p.MaximumTriangleArea);
            write("\nMax Triangle Length: " + p.MaximumTriangleLength);
            write("\nNumber of Triangles: " + p.NumberOfTriangles);
            write("\n--");
        }
    }
}