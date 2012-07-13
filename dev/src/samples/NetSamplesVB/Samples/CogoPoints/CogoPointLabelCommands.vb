Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.DatabaseServices
Imports Autodesk.Civil.DatabaseServices.Styles

<Assembly: CommandClass(GetType(Autodesk.CivilizedDevelopment.CogoPointLabelCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Class CogoPointLabelCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_ShowCogoPointLabelProperties")> _
    Public Sub CDS_ShowCogoPointLabelProperties()
      Dim pointId As ObjectId = selectCogoPoint()
      If pointId = ObjectId.Null Then
        Return
      End If
      showLabelProperties(pointId)
    End Sub

    <CommandMethod("CDS_OverrideLabelsForGroup")> _
    Public Sub CDS_OverrideLabelsForGroup()
      Using tr As Transaction = startTransaction()
        Dim pointGroupId As ObjectId = promptForPointGroup()
        If pointGroupId = ObjectId.Null Then
          Return
        End If
        overrideLabelsForPointsIn(pointGroupId)
        tr.Commit()
      End Using
    End Sub

    Private Function selectCogoPoint() As ObjectId
      Dim options As New PromptEntityOptions("Select COGO point: ")
      options.SetRejectMessage(vbLf & "Invalid COGO point selected.")
      options.AddAllowedClass(GetType(CogoPoint), True)
      Dim result As PromptEntityResult = _editor.GetEntity(options)
      If result.Status = PromptStatus.OK Then
        Return result.ObjectId
      End If
      Return ObjectId.Null
    End Function

    Private Sub showLabelProperties(pointId As ObjectId)
      Using tr As Transaction = startTransaction()
        Dim point As CogoPoint = TryCast(pointId.GetObject(OpenMode.ForRead), 
          CogoPoint)
        showLabelPropertiesFor(point)
      End Using
    End Sub

    Private Sub showLabelPropertiesFor(point As CogoPoint)
      write(vbLf & "Point Label Properties:")
      write(vbLf & "- Style: " & getLabelStyleName(point.LabelStyleId))
      write(vbLf & "- Style override: " _
            & getLabelStyleName(point.LabelStyleIdOverride))
      write(vbLf & "- Visible: " & point.IsLabelVisible.ToString())
      write(vbLf & "- Location: " & point.LabelLocation.ToString())
      write(vbLf & "- Rotation: " & point.LabelRotation.ToString())
      write(vbLf & "- Dragged: " & point.IsLabelDragged.ToString())
      write(vbLf & "- Pinned: " & point.IsLabelPinned.ToString())
    End Sub

    Private Function getLabelStyleName(id As ObjectId) As String
      Dim style As LabelStyle = TryCast(id.GetObject(OpenMode.ForRead), 
        LabelStyle)
      Return style.Name
    End Function

    Private Function promptForPointGroup() As ObjectId
      Dim result As PromptResult = _editor.GetString(vbLf _
        & "Enter point group name: ")
      If result.Status = PromptStatus.OK Then
        Return findGroup(result.StringResult)
      End If
      Return ObjectId.Null
    End Function

    Private Sub overrideLabelsForPointsIn(pointGroupId As ObjectId)
      Dim group As PointGroup = TryCast(
        pointGroupId.GetObject(OpenMode.ForWrite), PointGroup)
      group.IsPointLabelStyleOverridden = True
    End Sub

    Private Function findGroup(name As String) As ObjectId
      For Each id As ObjectId In _civildoc.PointGroups
        Dim group As PointGroup = TryCast(id.GetObject(OpenMode.ForRead), 
          PointGroup)
        If group.Name = name Then
          Return id
        End If
      Next
      Return ObjectId.Null
    End Function
  End Class
End Namespace
