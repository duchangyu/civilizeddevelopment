Imports Autodesk.AutoCAD.Colors
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.Land
Imports Autodesk.Civil.DatabaseServices

<Assembly: CommandClass( _
  GetType(Autodesk.CivilizedDevelopment.CalculateWaterdrops))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class CalculateWaterdrops
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_CalculateWaterdrops")> _
    Public Sub CDS_CalculateWaterdrops()
      Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
      Using ts As Transaction = startTransaction()
        Dim surfaceId As ObjectId = promptForTinSurface()
        Dim surface As TinSurface = _
          TryCast(surfaceId.GetObject(OpenMode.ForWrite), TinSurface)


        Dim drops As New ObjectIdCollection()
        For Each triangle As TinSurfaceTriangle In surface.Triangles
          Dim centroid As Point2d = getTriangleCentroid(triangle)
          ' calculate water-drop for the centroid
          Dim oid As ObjectIdCollection = _
            surface.Analysis.CreateWaterdrop( _
              centroid, WaterdropObjectType.Polyline3D)
          ' Save all the water-drops
          For Each id As ObjectId In oid
            drops.Add(id)
          Next
        Next

        ' calculate the longest waterdrop, then filter out 
        ' anything that is 25% the longest or less. Those
        ' are the sinks.
        Dim longest As Double = calculateLongestWaterDrop(drops)

        ' Filter out the sinks
        Dim sinks As Point3dCollection = filterSinks(drops, longest)
        markSinks(ts, sinks)

        ts.Commit()
      End Using

    End Sub

    Private Function getTriangleCentroid(triangle As TinSurfaceTriangle) _
      As Point2d

      ' The centroid is calculated from the cx and cy being:
      ' cx = (v1x + v2x + v3x) / 3
      ' cy = (v1y + v2y + v3y) / 3
      Dim cx As Double = (triangle.Vertex1.Location.X + _
                          triangle.Vertex2.Location.X + _
                          triangle.Vertex3.Location.X) / 3
      Dim cy As Double = (triangle.Vertex1.Location.Y + _
                          triangle.Vertex2.Location.Y + _
                          triangle.Vertex3.Location.Y) / 3
      Return New Point2d(cx, cy)
    End Function

    Private Function calculateLongestWaterDrop(drops As ObjectIdCollection) _
      As Double

      Dim longest As Double = 0
      For Each id As ObjectId In drops
        Dim drop As Polyline3d = TryCast(id.GetObject(OpenMode.ForRead),  _
          Polyline3d)
        longest = If(drop.Length > longest, drop.Length, longest)
      Next
      Return longest
    End Function

    Private Function filterSinks(drops As ObjectIdCollection, _
                                 longest As Double) As Point3dCollection
      Dim sinks As New Point3dCollection()
      For Each id As ObjectId In drops
        Dim drop As Polyline3d = TryCast(id.GetObject(OpenMode.ForRead),  _
          Polyline3d)
        If drop.Length > (longest * 0.25) Then
          sinks.Add(drop.EndPoint)
          Dim msg As String = _
            [String].Format("Sink located at: ({0},{1})" & _
                            vbLf, drop.EndPoint.X, drop.EndPoint.Y)
          write(msg)
        End If
      Next

      Return sinks
    End Function

    Private Sub markSinks(ts As Transaction, sinks As Point3dCollection)
      ' now lets mark each endpoint
      Dim acBlkTbl As BlockTable = _
        TryCast(_database.BlockTableId.GetObject(OpenMode.ForRead),  _
          BlockTable)
      Dim acBlkTblRec As BlockTableRecord = _
        TryCast(acBlkTbl(BlockTableRecord.ModelSpace).GetObject( _
            OpenMode.ForWrite), BlockTableRecord)
      ' set the point style
      _document.Database.Pdmode = 35
      _document.Database.Pdsize = 10
      For Each sink As Point3d In sinks
        Dim sinkPoint As New DBPoint(sink)
        sinkPoint.Color = Color.FromRgb(0, 255, 255)
        sinkPoint.SetDatabaseDefaults()
        acBlkTblRec.AppendEntity(sinkPoint)
        ts.AddNewlyCreatedDBObject(sinkPoint, True)
      Next
    End Sub

    Private Function promptForTinSurface() As ObjectId
      Dim promptMsg As String = vbLf & "Select TIN Surface: "
      Dim rejectMsg As String = vbLf & _
        "Selected entity is not a TIN Surface."
      Dim opts As New PromptEntityOptions(promptMsg)
      opts.SetRejectMessage(rejectMsg)
      opts.AddAllowedClass(GetType(TinSurface), False)
      Dim result As PromptEntityResult = _editor.GetEntity(opts)
      If result.Status = PromptStatus.OK Then
        Return result.ObjectId
      End If
      Return ObjectId.Null
    End Function
  End Class
End Namespace
