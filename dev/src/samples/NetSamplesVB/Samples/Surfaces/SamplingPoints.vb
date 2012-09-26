Imports System

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.DatabaseServices

Imports CivilSurface = Autodesk.Civil.DatabaseServices.Surface

<Assembly: CommandClass(
GetType(Autodesk.CivilizedDevelopment.SamplingPointsCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class SamplingPointsCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_SamplePointsOnSurface")> _
    Public Sub CDS_SamplePointsOnSurface()
      Using tr As Transaction = startTransaction()
        Dim surface As ITerrainSurface = TryCast(getSurface(), 
          ITerrainSurface)
        If surface Is Nothing Then
          Return
        End If
        Dim curveId As ObjectId = getCurveId()

        displayPoints(surface.SampleElevations(curveId))
      End Using
    End Sub



    Private Function getSurface() As CivilSurface
      Dim surfaceId As ObjectId = promptForSurface()
      If surfaceId.IsNull Then
        Return Nothing
      End If
      Dim surface As CivilSurface =
        TryCast(surfaceId.GetObject(OpenMode.ForRead), CivilSurface)
      Return surface
    End Function



    Private Function promptForSurface() As ObjectId
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



    Private Function getCurveId() As ObjectId
      Dim options As New PromptEntityOptions(vbLf & "Select entity: ")
      options.SetRejectMessage(
        vbLf & "The selected entity is not of the valid type.")
      Dim result As PromptEntityResult = _editor.GetEntity(options)
      If result.Status = PromptStatus.OK Then
        Return result.ObjectId
      End If
      Return ObjectId.Null
    End Function



    Private Sub displayPoints(points As Point3dCollection)
      For Each point As Point3d In points
        _editor.WriteMessage(vbLf + point.ToString())
      Next
    End Sub


  End Class
End Namespace