Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
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

    <CommandMethod("CDS_FindClosestPointOnSurface")> _
    Public Sub CDS_FindClosestPointOnSurface()
      Dim surfaceId As ObjectId = promptForTinSurface()
      If surfaceId.IsNull Then
        write(vbLf & "No TIN Surface selected.")
        Return
      End If

      Dim result As PromptPointResult = _editor.GetPoint(
        vbLf & "Select point outside surface: ")
      If result.Status <> PromptStatus.OK Then
        write(vbLf & "No point selected.")
        Return
      End If

      Dim selectedPoint As Point3d = result.Value
      Dim closestPointFound As Point3d = Point3d.Origin
      Dim shortestDistanceSoFar As Double = [Double].MaxValue

      Using tr As Transaction = startTransaction()
        Dim surface As TinSurface = TryCast(
          surfaceId.GetObject(OpenMode.ForRead), TinSurface)
        write(vbLf & "Selected surface: " & surface.Name)
        write(vbLf & "Selected point: " & selectedPoint.ToString())
        Dim borders As ObjectIdCollection = surface.ExtractBorder(
          SurfaceExtractionSettingsType.Model)
        For Each borderId As ObjectId In borders

          Dim border As Polyline3d = TryCast(
            borderId.GetObject(OpenMode.ForRead), Polyline3d)
          Dim closestToBorder As Point3d =
            border.GetClosestPointTo(selectedPoint, False)
          Dim distance As Double = selectedPoint.DistanceTo(closestToBorder)
          If distance < shortestDistanceSoFar Then
            closestPointFound = closestToBorder
            shortestDistanceSoFar = distance
          End If
        Next
      End Using

      write(vbLf & "Closest point found: " & closestPointFound.ToString())

      Using tr As Transaction = startTransaction()
        Dim btr As BlockTableRecord = TryCast(tr.GetObject(
            _database.CurrentSpaceId, OpenMode.ForWrite), BlockTableRecord)
        Dim line As New Line(selectedPoint, closestPointFound)
        btr.AppendEntity(line)
        tr.AddNewlyCreatedDBObject(line, True)
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
