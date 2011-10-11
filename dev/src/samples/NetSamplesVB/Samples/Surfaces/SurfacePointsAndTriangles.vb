Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.Land.DatabaseServices

Imports Colibra

<Assembly: CommandClass(GetType(Autodesk.CivilizedDevelopment.SurfacePointsAndTriangles))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class SurfacePointsAndTriangles
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_SurfaceTrianglesAndPointsDemo")> _
    Public Sub CDS_SurfaceTrianglesAndPointsDemo()
      Dim doc As Colibra.Document = DocumentManager.ActiveDocument
      Dim surfaceId As ObjectId = promptForSurface(doc)
      If surfaceId = ObjectId.Null Then
        write(vbLf & "No TIN surface selected.")
        Return
      End If

      Using tr As Colibra.Transaction = doc.StartTransaction()
        Dim surface As TinSurface =
          TryCast(surfaceId.GetObject(OpenMode.ForRead), TinSurface)

        For Each v As TinSurfaceVertex In surface.Vertices
          displayVertex(v)
        Next
      End Using
    End Sub


    Private Function promptForSurface(doc As Colibra.Document) As ObjectId
      Dim selector As New SingleObjectSelector(Of TinSurface)()
      selector.PromptMessage = vbLf & "Select TIN surface: "
      selector.RejectMessage = vbLf &
        "The selected object is not a TIN surface."
      selector.[Select](doc)
      Return selector.SelectedId
    End Function

    Private Sub displayVertex(vertex As TinSurfaceVertex)
      Dim msg As String =
        [String].Format(vbLf & "Vertex: ({0},{1},{2})",
                        vertex.Location.X, vertex.Location.Y,
                        vertex.Location.Z)
      write(msg)
    End Sub
  End Class
End Namespace
