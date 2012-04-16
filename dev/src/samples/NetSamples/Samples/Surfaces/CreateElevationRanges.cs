using System;

using Autodesk.AutoCAD.ApplicationServices;
using AcadDb = Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.AutoCAD.Colors;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.CreateElevationRanges))]

namespace Autodesk.CivilizedDevelopment
{
    public class CreateElevationRanges : SimpleDrawingCommand
    {
        [CommandMethod("CDS_CreateElevationRanges")]
        public void CDS_CreateElevationRanges()
        {
            AcadDb.ObjectId surfaceId = promptForTinSurface();
            if (surfaceId == AcadDb.ObjectId.Null)
            {
                return;
            }

            using (AcadDb.Transaction tr = startTransaction())
            {
                TinSurface surface =
                    surfaceId.GetObject(AcadDb.OpenMode.ForWrite) 
                    as TinSurface;

                // Get the existing analysis, if any.
                SurfaceAnalysisElevationData[] data =
                    surface.Analysis.GetElevationData();
                _editor.WriteMessage("\nExisting analysis length: {0}",
                    data.Length);
                SurfaceAnalysisElevationData[] newData =
                    CreateElevationRegions(surface, 10, 100);
                surface.Analysis.SetElevationData(newData);

                tr.Commit();

            }
        }

        private SurfaceAnalysisElevationData[] CreateElevationRegions(
            Surface surface, int steps, short startColor)
        {
            GeneralSurfaceProperties props = surface.GetGeneralProperties();
            double minElevation = props.MinimumElevation;
            double maxElevation = props.MaximumElevation;
            double increment = (maxElevation - minElevation) / steps;
            SurfaceAnalysisElevationData[] newData =
                new SurfaceAnalysisElevationData[steps];
            for (int i = 0; i < steps; i++)
            {
                Color color = Color.FromColorIndex(ColorMethod.ByLayer,
                    (short)(100 + (i * 2)));
                newData[i] = new SurfaceAnalysisElevationData(
                    minElevation + (increment * i),
                    minElevation + (increment * (i + 1)),
                    color);
            }
            return newData;
        }

        private AcadDb.ObjectId promptForTinSurface()
        {
            string promptMsg = "\nSelect TIN Surface: ";
            string rejectMsg = "\nSelected entity is not a TIN Surface.";
            PromptEntityOptions opts = new PromptEntityOptions(promptMsg);
            opts.SetRejectMessage(rejectMsg);
            opts.AddAllowedClass(typeof(TinSurface), false);
            PromptEntityResult result = _editor.GetEntity(opts);
            if (result.Status == PromptStatus.OK)
            {
                return result.ObjectId;
            }
            return AcadDb.ObjectId.Null;
        }
    }
}