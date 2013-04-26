Imports System

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.DatabaseServices

<Assembly: CommandClass(
GetType(Autodesk.CivilizedDevelopment.CorridorCommands))> 

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
                Dim alignment As Alignment =
                    TryCast(AlignmentId.GetObject(
                            OpenMode.ForRead), Alignment)
                Return alignment.GetProfileIds()(0)
            End Get
        End Property

        Public Sub CreateCorridor(document As CivilDocument)
            _document = document
            createCorridorObject()
            createCorridorBaseline()
            createBaselineRegion()
            _corridor.Rebuild()
        End Sub



        Private Sub createCorridorObject()
            Dim id As ObjectId = _corridors.Add(CorridorName)
            _corridor = TryCast(id.GetObject(OpenMode.ForWrite), 
                Corridor)
        End Sub



        Private Sub createCorridorBaseline()
            _baseline = _corridor.Baselines.Add(BaselineName,
                AlignmentId, ProfileId)
        End Sub



        Private Sub createBaselineRegion()
            _baseline.BaselineRegions.Add(RegionName, AssemblyId)
        End Sub



        Private ReadOnly Property _corridors() _
                As CorridorCollection
            Get
                Return _document.CorridorCollection
            End Get
        End Property

        Private _document As CivilDocument
        Private _corridor As Corridor
        Private _baseline As Baseline
    End Class
End Namespace