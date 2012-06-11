Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.DatabaseServices

<Assembly: CommandClass(GetType(Autodesk.CivilizedDevelopment.PointGroupCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class PointGroupCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_CreatePointGroups")> _
    Public Sub CDS_CreatePointGroups()
      Using tr As Transaction = startTransaction()
        Dim groupId As ObjectId = _civildoc.PointGroups.Add("Trees")
        Dim group As PointGroup = TryCast(groupId.GetObject(OpenMode.ForRead), PointGroup)
        Dim query As New StandardPointGroupQuery()
        query.IncludeRawDescriptions = "TREE*"
        group.SetQuery(query)

        groupId = _civildoc.PointGroups.Add("Wells")
        group = TryCast(groupId.GetObject(OpenMode.ForRead), PointGroup)
        query.IncludeRawDescriptions = "WELL*"
        group.SetQuery(query)

        tr.Commit()
      End Using
    End Sub
  End Class
End Namespace
