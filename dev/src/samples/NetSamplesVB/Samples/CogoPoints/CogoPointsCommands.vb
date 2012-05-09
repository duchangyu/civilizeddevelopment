Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil
Imports Autodesk.Civil.DatabaseServices

<Assembly: CommandClass(GetType(Autodesk.CivilizedDevelopment.CogoPointCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class CogoPointCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_CreateRandomPoints")> _
    Public Sub CDS_CreateRandomPoints()
      Dim generator As New RandomCoordinateGenerator()
      Dim points As CogoPointCollection = _civildoc.CogoPoints

      ' From Point3d
      Dim coordinate As Point3d = generator.GetCoordinate()
      Dim pointId As ObjectId = points.Add(coordinate)
      write(vbLf & "Single Point from coordinate.")
      display(pointId)

      coordinate = generator.GetCoordinate()
      pointId = points.Add(coordinate, "Sample description.")
      write(vbLf & "Single Point from coordinate and description.")
      display(pointId)

      coordinate = generator.GetCoordinate()
      pointId = points.Add(coordinate, "Sample description", True, False)
      write(vbLf & "Single Point from coordinate with description, " _
            & "using description key, and not matching parameters.")
      display(pointId)

      ' From Point3dCollection
      Dim coordinates As Point3dCollection = generator.GetCoordinates(10)
      Dim pointIds As ObjectIdCollection = points.Add(coordinates)
      write(vbLf & "Points from coordinate collection.")
      display(pointIds)

      coordinates = generator.GetCoordinates(5)
      pointIds = points.Add(coordinates, "Group of 5")
      write(vbLf & "Points from coordinate collection with description.")
      display(pointIds)

      coordinates = generator.GetCoordinates(7)
      pointIds = points.Add(coordinates, "Group of 7", True, True)
      write(vbLf & "Points from coordinate collection with description," _
            & "using description key, and not matching parameters.")
      display(pointIds)
    End Sub

    <CommandMethod("CDS_OffsetPointElevations")> _
    Public Sub CDS_OffsetPointElevations()
      Dim result As PromptDoubleResult = _editor.GetDouble(vbLf & "Enter elevation offset: ")
      If result.Status = PromptStatus.OK Then
        Dim offset As Double = result.Value
        Dim points As CogoPointCollection = _civildoc.CogoPoints
        points.SetElevationByOffset(points, offset)
      End If
    End Sub


    Private Sub display(pointId As ObjectId)
      Using tr As Transaction = startTransaction()
        Dim point As CogoPoint =
          TryCast(pointId.GetObject(OpenMode.ForRead), CogoPoint)
        displayPointInfo(point)
      End Using
    End Sub

    Private Sub display(pointIds As ObjectIdCollection)
      Using tr As Transaction = startTransaction()
        For Each pointId As ObjectId In pointIds
          Dim point As CogoPoint =
            TryCast(pointId.GetObject(OpenMode.ForRead), CogoPoint)
          displayPointInfo(point)
        Next
      End Using
    End Sub

    Private Sub displayPointInfo(point As CogoPoint)
      write(vbLf & "Point Number: " & point.PointNumber.ToString())
      write(vbLf & "- Location: " & point.Location.ToString())
      write(vbLf & "- - Northing: " & point.Northing.ToString())
      write(vbLf & "- - Easting: " & point.Easting.ToString())
      write(vbLf & "- - Elevation: " & point.Elevation.ToString())
      write(vbLf & "- Description: " & _
            Convert.ToString(point.FullDescription))
      write(vbLf & "- Raw Description: " & _
            Convert.ToString(point.RawDescription))
    End Sub
  End Class
End Namespace
