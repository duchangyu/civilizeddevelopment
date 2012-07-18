Imports System.Collections.Generic

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil
Imports Autodesk.Civil.DatabaseServices
Imports Autodesk.Civil.DatabaseServices.Styles

<Assembly: CommandClass(
  GetType(Autodesk.CivilizedDevelopment.CogoPointLabelCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Class CogoPointLabelCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_CreateDemoPointLabelStyle")> _
    Public Sub CDS_CreateDemoPointLabelStyle()

      createPointLabelStyle("Demo")
    End Sub

    Private Sub createPointLabelStyle(name As String)
      Dim styleId As ObjectId = _pointLabelStyles.Add(name)
      removeAllComponents(styleId)
      customizeStyle(styleId)
    End Sub

    Private ReadOnly Property _pointLabelStyles() As LabelStyleCollection
      Get
        Return _civildoc.Styles.LabelStyles.PointLabelStyles.LabelStyles
      End Get
    End Property

    Private Sub removeAllComponents(styleId As ObjectId)
      Dim componentNames As IEnumerable(Of String) = getTextComponentNames(
        styleId)
      removeComponents(styleId, componentNames)
    End Sub

    Private Function getTextComponentNames(styleId As ObjectId) _
        As IEnumerable(Of String)
      Dim names As New List(Of String)()
      Using tr As Transaction = startTransaction()
        Dim style As LabelStyle = TryCast(styleId.GetObject(OpenMode.ForRead), 
          LabelStyle)
        For Each id As ObjectId In style.GetComponents(
            LabelStyleComponentType.Text)
          Dim component As LabelStyleComponent = TryCast(
            id.GetObject(OpenMode.ForRead), LabelStyleComponent)
          names.Add(component.Name)
        Next
      End Using
      Return names
    End Function

    Private Sub removeComponents(styleId As ObjectId, componentNames _
        As IEnumerable(Of String))
      Using tr As Transaction = startTransaction()
        Dim style As LabelStyle = TryCast(styleId.GetObject(OpenMode.ForWrite), 
          LabelStyle)
        For Each name As String In componentNames
          style.RemoveComponent(name)
        Next

        tr.Commit()
      End Using
    End Sub

    Private Sub customizeStyle(styleId As ObjectId)
      Using tr As Transaction = startTransaction()
        addStyleComponents(styleId)
        tr.Commit()
      End Using
    End Sub

    Private Sub addStyleComponents(styleId As ObjectId)
      Dim style As LabelStyle = TryCast(styleId.GetObject(OpenMode.ForWrite), 
        LabelStyle)
      addLeaderComponent(style)
      addPointNumberComponent(style)
      addLocationComponent(style)
    End Sub

    Private Sub addLeaderComponent(style As LabelStyle)
      Dim id As ObjectId = style.AddComponent("Leader",
        LabelStyleComponentType.Line)
      Dim component As LabelStyleLineComponent = TryCast(
        id.GetObject(OpenMode.ForWrite), LabelStyleLineComponent)
      component.General.StartAnchorPoint.Value = AnchorPointType.MiddleCenter
    End Sub

    Private Sub addPointNumberComponent(style As LabelStyle)
      Dim id As ObjectId = style.AddComponent("PN",
        LabelStyleComponentType.Text)
      Dim component As LabelStyleTextComponent = TryCast(
        id.GetObject(OpenMode.ForWrite), LabelStyleTextComponent)
      component.Text.Attachment.Value = LabelTextAttachmentType.MiddleLeft
      component.Text.Contents.Value = _pointNumber
      component.General.AnchorComponent.Value = "Leader"
      component.General.AnchorLocation.Value = AnchorPointType.[End]
    End Sub

    Private Sub addLocationComponent(style As LabelStyle)
      Dim id As ObjectId = style.AddComponent("Location",
        LabelStyleComponentType.Text)
      Dim component As LabelStyleTextComponent = TryCast(
        id.GetObject(OpenMode.ForWrite), LabelStyleTextComponent)
      component.Text.Attachment.Value = LabelTextAttachmentType.TopLeft
      Dim value As String = [String].Format("({0}, {1}, {2})",
        _northing, _easting, _elevation)
      component.Text.Contents.Value = value
      component.General.AnchorComponent.Value = "PN"
      component.General.AnchorLocation.Value = AnchorPointType.BottomLeft
    End Sub

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
      write(vbLf & "- Style override: " & getLabelStyleName(
        point.LabelStyleIdOverride))
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
      Dim result As PromptResult = _editor.GetString(
        vbLf & "Enter point group name: ")
      If result.Status = PromptStatus.OK Then
        Return findGroup(result.StringResult)
      End If
      Return ObjectId.Null
    End Function

    Private Sub overrideLabelsForPointsIn(pointGroupId As ObjectId)
      Dim group As PointGroup = TryCast(pointGroupId.GetObject(OpenMode.ForWrite), 
        PointGroup)
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

    Private ReadOnly _pointNumber As String = "<[Point Number(Sn)]>"
    Private ReadOnly _northing As String =
      "<[Northing(Uft|P4|RN|AP|GC|UN|Sn|OF)]>"
    Private ReadOnly _easting As String =
      "<[Easting(Uft|P4|RN|AP|GC|UN|Sn|OF)]>"
    Private ReadOnly _elevation As String =
      "<[Point Elevation(Uft|P2|RN|AP|Sn|OF)]>"
  End Class
End Namespace
