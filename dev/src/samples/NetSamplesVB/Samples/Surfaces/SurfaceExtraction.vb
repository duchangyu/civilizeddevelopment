Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil
Imports Autodesk.Civil.DatabaseServices


<Assembly: CommandClass(
  GetType(Autodesk.CivilizedDevelopment.SurfaceExtractionCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class SurfaceExtractionCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_ExtractGrid")> _
    Public Sub CDS_ExtractGrid()
      Dim surfaceId As ObjectId = promptForTinSurface()
      If surfaceId = ObjectId.Null Then
        write(vbLf & "No TIN Surface selected.")
        Return
      End If

      Using tr As Transaction = startTransaction()
        Dim surface As TinSurface =
          TryCast(surfaceId.GetObject(OpenMode.ForRead), TinSurface)
        Dim ids As ObjectIdCollection = surface.ExtractGridded(
          SurfaceExtractionSettingsType.Model)
        For Each id As ObjectId In ids
          Dim polyline As Polyline3d =
            TryCast(id.GetObject(OpenMode.ForWrite), Polyline3d)
          If polyline IsNot Nothing Then
            Using polyline
              polyline.Color = AutoCAD.Colors.Color.FromRgb(255, 0, 0)
            End Using
          End If
        Next
        tr.Commit()
      End Using
    End Sub

    Private Function promptForTinSurface() As ObjectId
      Dim options As New PromptEntityOptions(vbLf & "Select a TIN Surface: ")
      options.SetRejectMessage(vbLf & _
                               "The selected object is not a TIN Surface.")
      options.AddAllowedClass(GetType(TinSurface), True)

      Dim result As PromptEntityResult = _editor.GetEntity(options)
      If result.Status = PromptStatus.OK Then
        ' Everything is cool; we return the selected
        ' surface ObjectId.
        Return result.ObjectId
      End If
      Return ObjectId.Null
      ' Indicating error.
    End Function
  End Class
End Namespace
