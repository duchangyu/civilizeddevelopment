Imports System

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil
Imports Autodesk.Civil.DatabaseServices

<Assembly: CommandClass(
GetType(Autodesk.CivilizedDevelopment.ProfileCreationCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class ProfileCreationCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_CreateMultipleProfileViews")> _
    Public Sub CDS_CreateMultipleProfileViews()
      Using tr As Transaction = startTransaction()
        Dim created As Boolean = createMultipleProfiles()
        If created Then
          tr.Commit()
        End If
      End Using
    End Sub



    Private Function createMultipleProfiles() As Boolean
      If noAlignmentSelected() Then
        Return False
      End If
      If noInsertionPointSelected() Then
        Return False
      End If
      createProfileViews()
      Return True
    End Function



    Private Function noAlignmentSelected() As Boolean
      _alignmentId = selectEntity(Of Alignment)(
        vbLf & "Select alignment:")
      Return _alignmentId.IsNull
    End Function



    Private Function noInsertionPointSelected() As Boolean
      Dim selected As Boolean = False
      Dim result As PromptPointResult = _editor.GetPoint(
        vbLf & "Select insertion point")
      If result.Status = PromptStatus.OK Then
        _insertionPoint = result.Value
        selected = True
      End If
      Return Not selected
    End Function



    Private Sub createProfileViews()
      ProfileView.CreateMultiple(_alignmentId, _insertionPoint,
                                 _creationOptions)
    End Sub



    Private ReadOnly Property _creationOptions() _
        As MultipleProfileViewsCreationOptions
      Get
        Dim options As New MultipleProfileViewsCreationOptions()
        options.DrawOrder = ProfileViewPlotType.ByRows
        options.GapBetweenViewsInColumn = 1000
        options.GapBetweenViewsInRow = 1000
        options.MaxViewInRowOrColumn = 3
        options.StartCorner = ProfileViewStartCornerType.UpperLeft
        Return options
      End Get
    End Property

    Private Function selectEntity(Of T)(prompt As String) As ObjectId
      Dim options As New PromptEntityOptions(prompt)
      options.SetRejectMessage(vbLf & "Incorrect entity type.")
      options.AddAllowedClass(GetType(T), True)
      Dim result As PromptEntityResult = _editor.GetEntity(options)
      If result.Status = PromptStatus.OK Then
        Return result.ObjectId
      End If
      Return ObjectId.Null
    End Function




    Private _alignmentId As ObjectId
    Private _insertionPoint As Point3d
  End Class
End Namespace