Imports System.Collections.Generic
Imports System.Linq


Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil
Imports Autodesk.Civil.DatabaseServices
Imports Autodesk.Civil.Settings

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
      Dim result As PromptDoubleResult = _editor.GetDouble(
        vbLf & "Enter elevation offset: ")
      If result.Status = PromptStatus.OK Then
        Dim offset As Double = result.Value
        Dim points As CogoPointCollection = _civildoc.CogoPoints
        points.SetElevationByOffset(points, offset)
      End If
    End Sub

    <CommandMethod("CDS_NaiveRenumberPoint")> _
    Public Sub CDS_NaiveRenumberPoint()
      Dim result As PromptIntegerResult = _editor.GetInteger(
        vbLf & "Enter point to renumber:")
      If result.Status <> PromptStatus.OK Then
        Return
      End If
      Dim currentPointNumber As UInteger = CUInt(result.Value)

      result = _editor.GetInteger(vbLf & "Enter new point number:")
      If result.Status <> PromptStatus.OK Then
        Return
      End If
      Dim newPointNumber As UInteger = CUInt(result.Value)

      Dim points As CogoPointCollection = _civildoc.CogoPoints
      Dim pointId As ObjectId = points.GetPointByPointNumber(
        currentPointNumber)
      points.SetPointNumber(pointId, newPointNumber)
    End Sub

    <CommandMethod("CDS_RenumberPoint")> _
    Public Sub CDS_RenumberPoint()
      Dim result As PromptIntegerResult = _editor.GetInteger(
        vbLf & "Enter point to renumber:")
      If result.Status <> PromptStatus.OK Then
        Return
      End If
      Dim currentPointNumber As UInteger = CUInt(result.Value)

      result = _editor.GetInteger(vbLf & "Enter new point number (hint):")
      If result.Status <> PromptStatus.OK Then
        Return
      End If
      Dim pointNumberHint As UInteger = CUInt(result.Value)

      Try
        Dim points As CogoPointCollection = _civildoc.CogoPoints
        Dim pointId As ObjectId =
          points.GetPointByPointNumber(currentPointNumber)
        points.SetPointNumber(pointId,
                              getNextPointNumberAvailable(pointNumberHint))
      Catch ex As ArgumentException
        _editor.WriteMessage(ex.Message)
      End Try

    End Sub

    <CommandMethod("CDS_CreateRandomPointsAtSpecifiedNumber")> _
    Public Sub CDS_CreateRandomPointsAtSpecifiedNumber()
      Dim result As PromptIntegerResult = _editor.GetInteger(
        vbLf & "Enter number of points to generate: ")
      If result.Status <> PromptStatus.OK Then
        Return
      End If
      Dim numberOfPoints As Integer = result.Value

      result = _editor.GetInteger(
        vbLf & "Enter base number for first point: ")
      If result.Status <> PromptStatus.OK Then
        Return
      End If
      Dim basePoint As UInteger = CUInt(result.Value)

      Dim createdIds As ObjectIdCollection = createPoints(numberOfPoints)
      renumberPoints(createdIds, basePoint)
    End Sub

    <CommandMethod("CDS_CreateRandomPointsAtSpecifiedNumberByFactor")> _
    Public Sub CDS_CreateRandomPointsAtSpecifiedNumberByFactor()
      Dim result As PromptIntegerResult = _editor.GetInteger(
        vbLf & "Enter number of points to generate: ")
      If result.Status <> PromptStatus.OK Then
        Return
      End If
      Dim numberOfPoints As Integer = result.Value

      result = _editor.GetInteger(
        vbLf & "Enter base number for first point: ")
      If result.Status <> PromptStatus.OK Then
        Return
      End If
      Dim basePoint As UInteger = CUInt(result.Value)

      Dim createdIds As ObjectIdCollection = createPoints(numberOfPoints)
      Dim firstCreatedPointNumber As UInteger =
        getPointNumberFor(createdIds(0))
      Dim additiveFactor As Integer =
        CInt(basePoint - firstCreatedPointNumber)
      Dim points As CogoPointCollection = _civildoc.CogoPoints
      points.SetPointNumber(ToEnumerable(createdIds), additiveFactor)
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
      write(vbLf & "- Description: " & Convert.ToString(point.FullDescription))
      write(vbLf & "- Raw Description: " _
            & Convert.ToString(point.RawDescription))
    End Sub

    Private Function getNextPointNumberAvailable(hint As UInteger) As UInteger
      Dim suggested As UInteger = hint
      Dim points As CogoPointCollection = _civildoc.CogoPoints
      While points.Contains(suggested) AndAlso suggested < _maxPointNumber
        suggested += 1
      End While

      If suggested = _maxPointNumber Then
        Dim msg As String = [String].Format(
          "No available point number at {0} or greater value.", hint)
        Throw New ArgumentException(msg)
      End If

      Return suggested
    End Function

    Private Function createPoints(numberOfPoints As Integer) As ObjectIdCollection

      Dim generator As New RandomCoordinateGenerator()
      Dim coordinates As Point3dCollection =
        generator.GetCoordinates(numberOfPoints)
      Dim points As CogoPointCollection = _civildoc.CogoPoints
      _creationSet += 1
      Dim description As String = [String].Format(
        "Creation {0}", _creationSet)

      Return points.Add(coordinates, description)
    End Function

    Private Sub renumberPoints(pointIds As ObjectIdCollection,
                               basePoint As UInteger)
      Dim points As CogoPointCollection = _civildoc.CogoPoints
      Dim suggested As UInteger = basePoint
      For Each pointId As ObjectId In pointIds
        suggested = getNextPointNumberAvailable(suggested)
        points.SetPointNumber(pointId, suggested)
        suggested += 1
      Next
    End Sub

    Private Function getPointNumberFor(pointId As ObjectId) As UInteger
      Using tr As Transaction = startTransaction()
        Dim point As CogoPoint =
          TryCast(pointId.GetObject(OpenMode.ForRead), CogoPoint)
        Return point.PointNumber
      End Using
    End Function

    Private Function ToEnumerable(ids As ObjectIdCollection) _
      As IEnumerable(Of ObjectId)
      Return ids.Cast(Of ObjectId)()
    End Function

    Private ReadOnly _maxPointNumber As UInteger = UInt32.MaxValue
    Shared _creationSet As Integer = 0
  End Class
End Namespace
