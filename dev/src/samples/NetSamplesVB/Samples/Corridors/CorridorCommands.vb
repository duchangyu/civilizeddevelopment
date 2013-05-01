Imports System

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.DatabaseServices

<Assembly: CommandClass(GetType(Autodesk.CivilizedDevelopment.CorridorCommands))> 

Namespace Autodesk.CivilizedDevelopment
    Public Class CorridorCommands
        Inherits SimpleDrawingCommand
        <CommandMethod("CDS_CreateCorridorFromScratch")> _
        Public Sub CDS_CreateCorridorFromScratch()
            Try

                executeCreateCorridorFromScratch()
            Catch ex As System.Exception
                logException(ex)
            End Try
        End Sub



        Private Sub executeCreateCorridorFromScratch()
            Using tr As Transaction = startTransaction()
                Dim creator As New CorridorCreator() With { _
                    .CorridorName = promptForString(
                        vbLf & "Enter corridor name: "), _
                    .BaselineName = promptForString(
                        vbLf & "Enter baseline name: "), _
                    .RegionName = promptForString(
                        vbLf & "Enter region name: "), _
                    .AlignmentId = _desiredAlignmentId, _
                    .AssemblyId = _desiredAssemblyId _
                }
                creator.CreateCorridor(_civildoc)

                tr.Commit()
            End Using
        End Sub



        Private Function promptForString(prompt As String) _
                As String
            Dim result As PromptResult = _editor.GetString(prompt)
            If result.Status = PromptStatus.OK Then
                Return result.StringResult
            End If
            Return [String].Empty
        End Function



        Private ReadOnly Property _desiredAlignmentId() _
                As ObjectId
            Get
                Return _civildoc.GetAlignmentIds()(0)
            End Get
        End Property

        Private ReadOnly Property _desiredAssemblyId() _
                As ObjectId
            Get
                Return _civildoc.AssemblyCollection(0)
            End Get
        End Property
    End Class

    Friend Class CorridorCreator
        Public Property CorridorName() As String
        Public Property BaselineName() As String
        Public Property RegionName() As String
        Public Property AlignmentId() As ObjectId
        Public Property AssemblyId() As ObjectId

        Public ReadOnly Property ProfileId() As ObjectId
            Get
                Dim alignment As Alignment = TryCast(
                    AlignmentId.GetObject(OpenMode.ForRead), 
                    Alignment)
                Return alignment.GetProfileIds()(0)
            End Get
        End Property

        Public Sub CreateCorridor(document As CivilDocument)
            _document = document
            createCorridorObject()
            createCorridorBaseline()
            createBaselineRegion()
            assignTargets()
            _corridor.Rebuild()
        End Sub



        Private Sub createCorridorObject()
            Dim id As ObjectId = _corridors.Add(CorridorName)
            _corridor = TryCast(id.GetObject(OpenMode.ForWrite), 
                Corridor)
        End Sub



        Private Sub createCorridorBaseline()
            _baseline = _corridor.Baselines.Add(BaselineName,
                                                AlignmentId,
                                                ProfileId)
        End Sub



        Private Sub createBaselineRegion()
            _region = _baseline.BaselineRegions.Add(RegionName,
                                                    AssemblyId)
        End Sub



        Private Sub assignTargets()
            ' These will return empty collections because the
            ' Corridor object has not been built yet.
            '
            ' SubassemblyTargetInfoCollection targets = 
            '      _corridor.GetTargets();
            ' SubassemblytargetInfoCollection targets = 
            '      _baseline.GetTargets();

            ' Getting the targets from the BaselineRegion 
            ' works because it access the information from
            ' the specified Assembly object when creating the
            ' BaselineRegion.
            '
            Dim targets As SubassemblyTargetInfoCollection =
                _region.GetTargets()
            For Each target As SubassemblyTargetInfo In targets

                assignTarget(target)
            Next
            ' The collection is empty if retrieved from the
            ' Corridor or Baseline object; therefore these
            ' calls will do nothing.
            ' 
            ' _corridor.SetTargets(targets);
            ' _baseline.SetTargets(targets);

            ' Regions allow you to specify the targets.
            '
            _region.SetTargets(targets)
        End Sub



        Private Sub assignTarget(target As SubassemblyTargetInfo)
            Select Case target.TargetType
                Case SubassemblyLogicalNameType.Surface
                    assignSurfaceTarget(target)
                    Exit Select

                Case SubassemblyLogicalNameType.Elevation
                    assignElevationTarget(target)
                    Exit Select

                Case SubassemblyLogicalNameType.Offset
                    assignOffsetTarget(target)
                    Exit Select
            End Select
        End Sub



        Private Sub assignSurfaceTarget(
                target As SubassemblyTargetInfo)
            ' The 'Add()' method of 'ObjectIdCollection'
            ' knows nothing about subassembly targets; therefore
            ' this call doesn't work.
            '
            ' target.TargetIds.Add(surfaceId);

            target.TargetIds = _targetSurfaces

            ' Alternatively, you can get the collection,
            ' manipulate it, and then set it again.
            '
            ' ObjectIdCollection ids = target.TargetIds;
            ' ... do whatever manipulations (add/remove targets)
            ' target.TargetIds = ids;
            '
            ' This will work, but trust me, my way (starting
            ' clean) it is easier most of the time.
        End Sub



        ' The following 2 methods are implemented under the
        ' assumption that the subassembly name contains a
        ' substring "Right" or "Left" depending on its side.
        ' This is a big assumption that works on this
        ' particular example, but you will have to get more
        ' creative.
        '
        Private Sub assignElevationTarget(
                target As SubassemblyTargetInfo)
            If isRightSide(target.SubassemblyName) Then
                target.TargetIds = _targetRightElevation
            Else
                target.TargetIds = _targetLeftElevation
            End If
        End Sub



        Private Sub assignOffsetTarget(
                target As SubassemblyTargetInfo)
            If isRightSide(target.SubassemblyName) Then
                target.TargetIds = _targetRightOffset
            Else
                target.TargetIds = _targetLeftOffset
            End If
        End Sub



        Private Function isRightSide(value As String) As Boolean
            Return value.Contains("Right")
        End Function



        Private ReadOnly Property _corridors() _
                As CorridorCollection
            Get
                Return _document.CorridorCollection
            End Get
        End Property

        Private ReadOnly Property _targetSurfaces() _
                As ObjectIdCollection
            Get
                Return _document.GetSurfaceIds()
            End Get
        End Property

        Private ReadOnly Property _targetRightOffset() _
                As ObjectIdCollection
            Get
                If _rightOffsetAlignmentId = ObjectId.Null Then
                    resolveAllTargetIds()
                End If
                Return insideCollection(_rightOffsetAlignmentId)
            End Get
        End Property

        Private ReadOnly Property _targetLeftOffset() _
                As ObjectIdCollection
            Get
                If _leftOffsetAlignmentId = ObjectId.Null Then
                    resolveAllTargetIds()
                End If
                Return insideCollection(_leftOffsetAlignmentId)
            End Get
        End Property

        Private ReadOnly Property _targetRightElevation() _
                As ObjectIdCollection
            Get
                If _rightProfileId = ObjectId.Null Then
                    resolveAllTargetIds()
                End If
                Return insideCollection(_rightProfileId)
            End Get
        End Property

        Private ReadOnly Property _targetLeftElevation() _
                As ObjectIdCollection
            Get
                If _leftProfileId = ObjectId.Null Then
                    resolveAllTargetIds()
                End If
                Return insideCollection(_leftProfileId)
            End Get
        End Property

        Private Sub resolveAllTargetIds()
            Dim alignment As Alignment = TryCast(
                AlignmentId.GetObject(OpenMode.ForRead), 
                Alignment)
            Dim offsetAlignments As ObjectIdCollection =
                alignment.GetChildOffsetAlignmentIds()
            For Each offsetId As ObjectId In offsetAlignments
                resolveTargetIds(offsetId)
            Next
        End Sub




        ' I also make the assumption here that the specified
        ' alignment has the string "Left" or "Right" in the
        ' name depending on its side from the centerline.
        '
        Private Sub resolveTargetIds(alignmentId As ObjectId)
            Dim alignment As Alignment = TryCast(
                alignmentId.GetObject(OpenMode.ForRead), 
                Alignment)
            If isRightSide(alignment.Name) Then
                _rightOffsetAlignmentId = alignmentId

                _rightProfileId = alignment.GetProfileIds()(0)
            Else
                _leftOffsetAlignmentId = alignmentId
                _leftProfileId = alignment.GetProfileIds()(0)
            End If
        End Sub



        Private Function insideCollection(id As ObjectId) _
                As ObjectIdCollection
            Dim col As New ObjectIdCollection()
            col.Add(id)
            Return col
        End Function



        Private _document As CivilDocument
        Private _corridor As Corridor
        Private _baseline As Baseline
        Private _region As BaselineRegion
        Private _leftOffsetAlignmentId As ObjectId =
            ObjectId.Null
        Private _rightOffsetAlignmentId As ObjectId =
            ObjectId.Null
        Private _leftProfileId As ObjectId = ObjectId.Null
        Private _rightProfileId As ObjectId = ObjectId.Null
    End Class
End Namespace