Imports System

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil
Imports Autodesk.Civil.DatabaseServices

Imports CivilSurface = Autodesk.Civil.DatabaseServices.Surface
Imports AcadEntity = Autodesk.AutoCAD.DatabaseServices.Entity

<Assembly: CommandClass(
  GetType(Autodesk.CivilizedDevelopment.SurfaceExtractContoursCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class SurfaceExtractContoursCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_ExtractSurfceContours")> _
    Public Sub CDS_ExtractSurfceContours()
      Dim surfaceId As ObjectId = promptForTinSurface()
      If surfaceId.IsNull Then
        write(vbLf & "No Surface selected.")
        Return
      End If

      Using tr As Transaction = startTransaction()
        Dim surface As CivilSurface =
          TryCast(surfaceId.GetObject(OpenMode.ForRead), CivilSurface)
        showGeneralProperties(surface.Name,
                              surface.GetGeneralProperties())
        Dim terrainSurface As ITerrainSurface =
          TryCast(surface, ITerrainSurface)
        If terrainSurface IsNot Nothing Then
          extractMajorContours(terrainSurface)
          extractMinorContours(terrainSurface)
        End If

        tr.Commit()
      End Using
    End Sub



    Private Sub showGeneralProperties(name As String,
                                      props As GeneralSurfaceProperties)
      _editor.WriteMessage(vbLf & "Surface name: " + name)
      _editor.WriteMessage(vbLf & "- Max elevation: " +
                           props.MaximumElevation)
      _editor.WriteMessage(vbLf & "- Min elevation: " +
                           props.MinimumElevation)
    End Sub



    Private Sub extractMajorContours(surface As ITerrainSurface)
      Dim contours As ObjectIdCollection =
        surface.ExtractMajorContours(SurfaceExtractionSettingsType.Model)
      Dim blue As AutoCAD.Colors.Color =
        AutoCAD.Colors.Color.FromRgb(0, 0, 255)
      customizeContours(contours, blue)
    End Sub



    Private Sub extractMinorContours(surface As ITerrainSurface)
      Dim contours As ObjectIdCollection =
        surface.ExtractMinorContours(SurfaceExtractionSettingsType.Model)
      Dim lightblue As AutoCAD.Colors.Color =
        AutoCAD.Colors.Color.FromRgb(0, 255, 255)
      customizeContours(contours, lightblue)
    End Sub



    Private Sub customizeContours(contours As ObjectIdCollection,
                                  color As AutoCAD.Colors.Color)
      For Each id As ObjectId In contours
        Dim entity As AcadEntity =
          TryCast(id.GetObject(OpenMode.ForWrite), AcadEntity)
        entity.Color = color
      Next
    End Sub




    Private Function promptForTinSurface() As ObjectId
      Dim options As New PromptEntityOptions(
        vbLf & "Select a TIN Surface: ")
      options.SetRejectMessage(
        vbLf & "The selected object is not a TIN Surface.")
      options.AddAllowedClass(GetType(CivilSurface), False)

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