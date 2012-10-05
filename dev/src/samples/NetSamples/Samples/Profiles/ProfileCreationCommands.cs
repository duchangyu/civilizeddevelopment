using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.ProfileCreationCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class ProfileCreationCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_CreateMultipleProfileViews")]
        public void CDS_CreateMultipleProfileViews()
        {
            using (Transaction tr = startTransaction())
            {
                bool created = createMultipleProfiles();
                if (created)
                {
                    tr.Commit();
                }
            }
        }

        private bool createMultipleProfiles()
        {
            if (noAlignmentSelected()) return false;
            if (noInsertionPointSelected()) return false;
            createProfileViews();
            return true;
        }

        private bool noAlignmentSelected()
        {
            _alignmentId = selectEntity<Alignment>("\nSelect alignment:");
            return _alignmentId.IsNull;
        }        

        private bool noInsertionPointSelected()
        {
            bool selected = false;
            PromptPointResult result = 
                _editor.GetPoint("\nSelect insertion point");
            if (result.Status == PromptStatus.OK)
            {
                _insertionPoint = result.Value;
                selected = true;
            }
            return !selected;
        }

        private void createProfileViews()
        {
            ProfileView.CreateMultiple(_alignmentId, _insertionPoint,
                _creationOptions);
        }

        private MultipleProfileViewsCreationOptions _creationOptions
        {
            get
            {
                MultipleProfileViewsCreationOptions options =
                    new MultipleProfileViewsCreationOptions();
                options.DrawOrder = ProfileViewPlotType.ByRows;
                options.GapBetweenViewsInColumn = 1000;
                options.GapBetweenViewsInRow = 1000;
                options.MaxViewInRowOrColumn = 3;
                options.StartCorner = 
                    ProfileViewStartCornerType.UpperLeft;
                return options;
            }
        }

        private ObjectId selectEntity<T>(string prompt)
        {
            PromptEntityOptions options =
                new PromptEntityOptions(prompt);
            options.SetRejectMessage("\nIncorrect entity type.");
            options.AddAllowedClass(typeof(T), true);
            PromptEntityResult result = _editor.GetEntity(options);
            if (result.Status == PromptStatus.OK)
            {
                return result.ObjectId;
            }
            return ObjectId.Null;
        }


        ObjectId _alignmentId;
        Point3d _insertionPoint;
    }
}