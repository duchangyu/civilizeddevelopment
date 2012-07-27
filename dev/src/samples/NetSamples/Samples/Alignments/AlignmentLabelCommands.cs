using System;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.AlignmentLabelCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class AlignmentLabelCommands : SimpleDrawingCommand
    {
        
    }
}