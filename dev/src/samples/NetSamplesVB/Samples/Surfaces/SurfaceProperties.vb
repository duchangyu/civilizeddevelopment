Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.Land.DatabaseServices

<Assembly: CommandClass(GetType(Autodesk.CivilizedDevelopment.SurfacePropertiesDemo))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class SurfacePropertiesDemo
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_TinSurfacePropertiesDemo")> _
    Public Sub CDS_TinSurfacePropertiesDemo()
      Dim surfaceId As ObjectId = promptForTinSurface()
      If ObjectId.Null = surfaceId Then
        write(vbLf & "No TIN Surface object was selected.")
        ' We don't have a surface; we can't continue.
        Return
      End If

      Using tr As Transaction = startTransaction()
        Dim surface As TinSurface =
          TryCast(surfaceId.GetObject(OpenMode.ForRead), TinSurface)
        write(vbLf & "Information for TIN Surface: " + surface.Name)
        writeGeneralProperites(surface.GetGeneralProperties())
        writeTerrainProperties(surface.GetTerrainProperties())
        writeTinSurfaceProperties(surface.GetTinProperties())
      End Using
    End Sub

    ''' <summary>
    ''' Prompts the user to select a TIN Surface, and returns its
    ''' ObjectId.
    ''' </summary>
    ''' <returns>The method returns the ObjectId of the selected
    ''' surface, or ObjectId.Null if no surface was selected (usually,
    ''' because the user canceled the operation).
    ''' </returns>
    Private Function promptForTinSurface() As ObjectId
      Dim options As New PromptEntityOptions(
        vbLf & "Select a TIN Surface: ")
      options.SetRejectMessage(
        vbLf & "The selected object is not a TIN Surface.")
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

    ''' <summary>
    ''' Displays general properties that apply to all Surface objects.
    ''' </summary>
    ''' <param name="props">General properties from surface.</param>
    Private Sub writeGeneralProperites(p As GeneralSurfaceProperties)
      write(vbLf & "General Properties:")
      write(vbLf & "-------------------")
      write(vbLf & "Min X: " & Convert.ToString(p.MinimumCoordinateX))
      write(vbLf & "Min Y: " & Convert.ToString(p.MinimumCoordinateY))
      write(vbLf & "Min Z: " & Convert.ToString(p.MinimumElevation))
      write(vbLf & "Max X: " & Convert.ToString(p.MaximumCoordinateX))
      write(vbLf & "Max Y: " & Convert.ToString(p.MaximumCoordinateY))
      write(vbLf & "Max Z: " & Convert.ToString(p.MaximumElevation))
      write(vbLf & "Mean Elevation: " &
            Convert.ToString(p.MeanElevation))
      write(vbLf & "Number of Points: " &
            Convert.ToString(p.NumberOfPoints))
      write(vbLf & "--")
    End Sub

    Private Sub writeTerrainProperties(p As TerrainSurfaceProperties)
      write(vbLf & "Terrain Surface Properties:")
      write(vbLf & "---------------------------")
      write(vbLf & "Min Grade/Slope: " &
            Convert.ToString(p.MinimumGradeOrSlope))
      write(vbLf & "Max Grade/Slope: " &
            Convert.ToString(p.MaximumGradeOrSlope))
      write(vbLf & "Mean Grade/Slope: " &
            Convert.ToString(p.MeanGradeOrSlope))
      write(vbLf & "2D Area: " &
            Convert.ToString(p.SurfaceArea2D))
      write(vbLf & "3D Area: " &
            Convert.ToString(p.SurfaceArea3D))
      write(vbLf & "--")

    End Sub

    Private Sub writeTinSurfaceProperties(p As TinSurfaceProperties)
      write(vbLf & "TIN Surface Properties:")
      write(vbLf & "-----------------------")
      write(vbLf & "Min Triangle Area: " &
            Convert.ToString(p.MinimumTriangleArea))
      write(vbLf & "Min Triangle Length: " &
            Convert.ToString(p.MinimumTriangleLength))
      write(vbLf & "Max Triangle Area: " &
            Convert.ToString(p.MaximumTriangleArea))
      write(vbLf & "Max Triangle Length: " &
            Convert.ToString(p.MaximumTriangleLength))
      write(vbLf & "Number of Triangles: " &
            Convert.ToString(p.NumberOfTriangles))
      write(vbLf & "--")
    End Sub
  End Class
End Namespace
