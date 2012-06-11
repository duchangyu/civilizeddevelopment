﻿Imports System.Collections.Generic

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.DatabaseServices

<Assembly: CommandClass(
GetType(Autodesk.CivilizedDevelopment.PointGroupCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class PointGroupCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_CreateCivilDevPointGroups")> _
    Public Sub CDS_CreateCivilDevPointGroups()
      Using tr As Transaction = startTransaction()
        createPointGroup("Trees", "TREE*")
        createPointGroup("Wells", "WELL*")
        tr.Commit()
      End Using
    End Sub

    <CommandMethod("CDS_ReversPointGroupDrawOrder")> _
    Public Sub CDS_ReversPointGroupDrawOrder()
      Dim drawPriority As ObjectIdCollection = _pointGroups.DrawOrder
      Dim reversed As New ObjectIdCollection()
      For i As Integer = drawPriority.Count - 1 To 0 Step -1
        Dim current As ObjectId = drawPriority(i)
        reversed.Add(current)
      Next
      _pointGroups.DrawOrder = reversed
    End Sub

    <CommandMethod("CDS_RenumberPointsForGroup")> _
    Public Sub CDS_RenumberPointsForGroup()
      Dim pointGroupName As String = getPointGroupName()
      If pointGroupName = [String].Empty Then
        Return
      End If

      Dim baseNumber As Integer = getNewBaseNumber()
      If baseNumber = _kNoNumber Then
        Return
      End If

      renumberPointsForGroup(pointGroupName, baseNumber)
    End Sub

    Private Sub createPointGroup(name As String,
                                 includeRawDescription As String)
      If _pointGroups.Contains(name) Then
        Return
      End If
      Dim groupId As ObjectId = _pointGroups.Add(name)
      Dim query As New StandardPointGroupQuery()
      query.IncludeRawDescriptions = includeRawDescription
      Dim group As PointGroup =
        TryCast(groupId.GetObject(OpenMode.ForRead), PointGroup)
      group.SetQuery(query)
    End Sub

    Private ReadOnly Property _pointGroups() As PointGroupCollection
      Get
        Return _civildoc.PointGroups
      End Get
    End Property

    Private Function getPointGroupName() As String
      Dim result As PromptResult =
        _editor.GetString(vbLf & "Enter point group name: ")
      If result.Status = PromptStatus.OK Then
        Return result.StringResult
      End If
      Return [String].Empty
    End Function

    Private Function getNewBaseNumber() As Integer
      Dim result As PromptIntegerResult =
        _editor.GetInteger(vbLf & "Enter new base number: ")
      If result.Status = PromptStatus.OK Then
        Return result.Value
      End If

      Return _kNoNumber
    End Function

    Private Sub renumberPointsForGroup(groupName As String,
                                       baseNumber As Integer)
      Using tr As Transaction = startTransaction()
        Dim pointGroupId As ObjectId = getPointGroupIdByName(groupName)
        Dim group As PointGroup =
          TryCast(pointGroupId.GetObject(OpenMode.ForRead), PointGroup)
        renumberPoints(group, baseNumber)
        tr.Commit()
      End Using
    End Sub

    Private Function getPointGroupIdByName(groupName As String) As ObjectId
      Return _pointGroups(groupName)
    End Function

    Private Sub renumberPoints(group As PointGroup, baseNumber As Integer)
      Dim pointNumbers As UInteger() = group.GetPointNumbers()
      Dim firstNumber As Integer = CInt(pointNumbers(0))
      Dim factor As Integer = baseNumber - firstNumber
      _civildoc.CogoPoints.SetPointNumber(
        ToEnumerableObjectId(group.GetPointNumbers()), factor)
      group.Update()
    End Sub

    Private Function ToEnumerableObjectId(numbers As UInteger()) _
        As IEnumerable(Of ObjectId)
      Dim points As List(Of ObjectId) = New List(Of ObjectId)
      For Each number As UInteger In numbers
        Dim id = _civildoc.CogoPoints.GetPointByPointNumber(number)
        points.Add(id)
      Next
      Return points
    End Function

    Private Const _kNoNumber As Integer = -1
  End Class
End Namespace
