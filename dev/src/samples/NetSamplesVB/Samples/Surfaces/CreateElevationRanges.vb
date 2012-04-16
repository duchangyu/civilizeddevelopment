Imports Autodesk.AutoCAD.ApplicationServices
Imports AcadDb = Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.DatabaseServices
Imports Autodesk.AutoCAD.Colors

<Assembly: CommandClass(GetType(Autodesk.CivilizedDevelopment.CreateElevationRanges))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class CreateElevationRanges
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_CreateElevationRanges")> _
    Public Sub CDS_CreateElevationRanges()
      Dim surfaceId As AcadDb.ObjectId = promptForTinSurface()
      If surfaceId = AcadDb.ObjectId.Null Then
        Return
      End If

      Using tr As AcadDb.Transaction = startTransaction()
        Dim surface As TinSurface =
          TryCast(surfaceId.GetObject(AcadDb.OpenMode.ForWrite), TinSurface)

        ' Get the existing analysis, if any.
        Dim data As SurfaceAnalysisElevationData() =
          surface.Analysis.GetElevationData()
        _editor.WriteMessage(
          vbLf & "Existing analysis length: {0}", data.Length)
        Dim newData As SurfaceAnalysisElevationData() =
          CreateElevationRegions(surface, 10, 100)
        surface.Analysis.SetElevationData(newData)


        tr.Commit()
      End Using
    End Sub

    Private Function CreateElevationRegions(surface As Surface,
                                            steps As Integer,
                                            startColor As Short) _
                                          As SurfaceAnalysisElevationData()
      Dim props As GeneralSurfaceProperties = surface.GetGeneralProperties()
      Dim minElevation As Double = props.MinimumElevation
      Dim maxElevation As Double = props.MaximumElevation
      Dim increment As Double = (maxElevation - minElevation) / steps
      Dim newData As SurfaceAnalysisElevationData() =
        New SurfaceAnalysisElevationData(steps - 1) {}
      For i As Integer = 0 To steps - 1
        Dim color__1 As Color =
          Color.FromColorIndex(ColorMethod.ByLayer, CShort(100 + (i * 2)))
        newData(i) =
          New SurfaceAnalysisElevationData(minElevation + (increment * i),
                                    minElevation + (increment * (i + 1)),
                                    color__1)
      Next
      Return newData
    End Function

    Private Function promptForTinSurface() As AcadDb.ObjectId
      Dim promptMsg As String = vbLf & "Select TIN Surface: "
      Dim rejectMsg As String =
        vbLf & "Selected entity is not a TIN Surface."
      Dim opts As New PromptEntityOptions(promptMsg)
      opts.SetRejectMessage(rejectMsg)
      opts.AddAllowedClass(GetType(TinSurface), False)
      Dim result As PromptEntityResult = _editor.GetEntity(opts)
      If result.Status = PromptStatus.OK Then
        Return result.ObjectId
      End If
      Return AcadDb.ObjectId.Null
    End Function
  End Class
End Namespace
