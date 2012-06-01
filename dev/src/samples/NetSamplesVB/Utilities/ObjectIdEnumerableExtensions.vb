Imports System.Collections.Generic

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Civil.DatabaseServices
Imports Autodesk.Civil.DatabaseServices.Styles

Namespace Autodesk.CivilizedDevelopment
  Module ObjectIdEnumerableExtensions
    <System.Runtime.CompilerServices.Extension()> _
    Public Function GetStyle(ids As IEnumerable(Of ObjectId),
                             name As String) As ObjectId
      For Each id As ObjectId In ids
        Dim style As StyleBase =
          TryCast(id.GetObject(OpenMode.ForRead), StyleBase)
        If style.Name = name Then
          Return id
        End If
      Next
      Return ObjectId.Null
    End Function
  End Module

End Namespace
