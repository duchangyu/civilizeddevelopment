Imports System.Collections.Generic

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.Land.DatabaseServices
Imports Autodesk.Civil.Land.DatabaseServices.Styles

<Assembly: CommandClass(GetType(Autodesk.CivilizedDevelopment.DisplayRepresentations))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class DisplayRepresentations
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_TurnDisplayRepresentationsOn")> _
    Public Sub CDS_TurnDisplayRepresentationsOn()
      Dim styleId As ObjectId = findSurfaceStyle("SampleStyle")
      If styleId = ObjectId.Null Then
        styleId = _civildoc.Styles.SurfaceStyles.Add("SampleStyle")
      End If

      Using tr As Transaction = startTransaction()
        Dim style As SurfaceStyle =
          TryCast(styleId.GetObject(OpenMode.ForWrite), SurfaceStyle)
        Dim settings As SurfaceDisplayStyleType() =
          New SurfaceDisplayStyleType() _
          {
            SurfaceDisplayStyleType.MajorContour,
            SurfaceDisplayStyleType.MinorContour,
            SurfaceDisplayStyleType.Points,
            SurfaceDisplayStyleType.Triangles
          }

        applySettings(style, settings)

        tr.Commit()
      End Using
    End Sub

    Private Function findSurfaceStyle(name As String) As ObjectId
      Using tr As Transaction = startTransaction()
        For Each styleId As ObjectId In _civildoc.Styles.SurfaceStyles
          Dim style As SurfaceStyle = TryCast(styleId.GetObject(OpenMode.ForRead), SurfaceStyle)
          If style.Name = name Then
            Return styleId
          End If
        Next
      End Using

      Return ObjectId.Null
    End Function

    Private Sub applySettings(style As SurfaceStyle, _
                              settings As IList(Of SurfaceDisplayStyleType))
      Dim displayTypes As IEnumerable(Of SurfaceDisplayStyleType) =
        TryCast([Enum].GetValues(GetType(SurfaceDisplayStyleType)), 
          IEnumerable(Of SurfaceDisplayStyleType))

      For Each displayType As SurfaceDisplayStyleType In displayTypes
        Dim state As Boolean = settings.Contains(displayType)
        style.GetDisplayStylePlan(displayType).Visible = state
        style.GetDisplayStyleModel(displayType).Visible = state
      Next
    End Sub
  End Class
End Namespace
