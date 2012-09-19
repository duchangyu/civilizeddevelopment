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


    <CommandMethod("CDS_ExtractSurfaceContoursAtElevation")> _
    Public Sub CDS_ExtractSurfaceContoursAtElevation()
      Using tr As Transaction = startTransaction()
        Dim surface As ITerrainSurface = TryCast(getSurface(), 
          ITerrainSurface)
        If surface Is Nothing Then
          Return
        End If

        Dim elevation As Double = getDouble("elevation")
        If [Double].IsNaN(elevation) Then
          Return
        End If

        If elevationInSurfaceRange(elevation,
                                   TryCast(surface, CivilSurface)) Then
          Dim contours As ObjectIdCollection =
            surface.ExtractContoursAt(elevation)
          customizeContours(contours, _singleContourColor)
        End If

        tr.Commit()
      End Using
    End Sub



    Private Function elevationInSurfaceRange(elevation As Double,
        surface As CivilSurface) As Boolean
      Dim properties As GeneralSurfaceProperties =
        surface.GetGeneralProperties()
      If elevation < properties.MinimumElevation Or
           elevation > properties.MaximumElevation Then
        _editor.WriteMessage(
          vbLf & "Specified elevation not in surface range.")
        Return False
      End If
      Return True

    End Function

    <CommandMethod("CDS_ExtractSurfaceContoursFromToElevationRange")> _
    Public Sub CDS_ExtractSurfaceContoursFromToElevationRange()
      Using tr As Transaction = startTransaction()
        Dim surface As ITerrainSurface = TryCast(getSurface(), 
          ITerrainSurface)
        If surface Is Nothing Then
          Return
        End If

        Dim minElevation As Double = getDouble("minimum elevation")
        If [Double].IsNaN(minElevation) Then
          Return
        End If

        Dim maxElevation As Double = getDouble("maximum elevation")
        If [Double].IsNaN(maxElevation) Then
          Return
        End If

        Dim interval As Double = getDouble("interval")
        If [Double].IsNaN(interval) Then
          Return
        End If

        Dim contours As ObjectIdCollection = surface.ExtractContours(
          minElevation, maxElevation, interval)

        customizeContours(contours, _rangedContoursColor)

        tr.Commit()
      End Using
    End Sub



    <CommandMethod("CDS_ExtractSurfaceContoursAtInterval")> _
    Public Sub CDS_ExtractSurfaceContoursAtInterval()
      Using tr As Transaction = startTransaction()
        Dim surface As ITerrainSurface = TryCast(getSurface(), 
          ITerrainSurface)
        If surface Is Nothing Then
          Return
        End If

        Dim interval As Double = getDouble("interval")
        If [Double].IsNaN(interval) Then
          Return
        End If

        Dim contours As ObjectIdCollection = surface.ExtractContours(
          interval)

        customizeContours(contours, _intervalContoursColor)

        tr.Commit()
      End Using
    End Sub



    <CommandMethod("CDS_ExtractSurfaceMajorAndMinorContours")> _
    Public Sub CDS_ExtractSurfaceMajorAndMinorContours()
      Using tr As Transaction = startTransaction()
        Dim surface As ITerrainSurface = TryCast(getSurface(), 
          ITerrainSurface)
        If surface Is Nothing Then
          Return
        End If

        extractMajorContours(surface)
        extractMinorContours(surface)



        tr.Commit()
      End Using
    End Sub





    Private Sub extractMajorContours(surface As ITerrainSurface)
      Dim contours As ObjectIdCollection = surface.ExtractMajorContours(
        SurfaceExtractionSettingsType.Model)
      customizeContours(contours, _majorContoursColor)
    End Sub



    Private Sub extractMinorContours(surface As ITerrainSurface)
      Dim contours As ObjectIdCollection = surface.ExtractMinorContours(
        SurfaceExtractionSettingsType.Model)
      customizeContours(contours, _minorContoursColor)
    End Sub



    Private Sub customizeContours(contours As ObjectIdCollection,
                                  color As AutoCAD.Colors.Color)
      For Each id As ObjectId In contours
        Dim entity As AcadEntity =
          TryCast(id.GetObject(OpenMode.ForWrite), AcadEntity)
        entity.Color = color
      Next
    End Sub



    Private Function getSurface() As CivilSurface
      Dim surfaceId As ObjectId = promptForSurface()
      If surfaceId.IsNull Then
        Return Nothing
      End If
      Dim surface As CivilSurface =
        TryCast(surfaceId.GetObject(OpenMode.ForRead), CivilSurface)
      showGeneralProperties(surface)
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



    Private Sub showGeneralProperties(surface As CivilSurface)
      _editor.WriteMessage(vbLf & "Surface name: " + surface.Name)
      Dim properties As GeneralSurfaceProperties =
        surface.GetGeneralProperties()
      _editor.WriteMessage(
        vbLf & "- Max elevation: " + properties.MaximumElevation)
      _editor.WriteMessage(
        vbLf & "- Min elevation: " + properties.MinimumElevation)
    End Sub



    Private Function getDouble(valueName As String) As Double
      Dim result As Double = [Double].NaN
      Dim msg As String = [String].Format(
        vbLf & "Enter value for {0}:", valueName)
      Dim res As PromptDoubleResult = _editor.GetDouble(msg)
      If res.Status = PromptStatus.OK Then
        result = res.Value
      End If
      Return result
    End Function



    Private _majorContoursColor As AutoCAD.Colors.Color =
      AutoCAD.Colors.Color.FromRgb(0, 0, 255)
    Private _minorContoursColor As AutoCAD.Colors.Color =
      AutoCAD.Colors.Color.FromRgb(0, 255, 255)
    Private _rangedContoursColor As AutoCAD.Colors.Color =
      AutoCAD.Colors.Color.FromRgb(255, 0, 0)
    Private _intervalContoursColor As AutoCAD.Colors.Color =
      AutoCAD.Colors.Color.FromRgb(0, 255, 0)
    Private _singleContourColor As AutoCAD.Colors.Color =
      AutoCAD.Colors.Color.FromRgb(255, 255, 0)
  End Class
End Namespace