Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime

<Assembly: CommandClass( _
    GetType(Autodesk.CivilizedDevelopment.DatabaseTypeCommand))> 

Namespace Autodesk.CivilizedDevelopment
    Public Class DatabaseTypeCommand
        Inherits SimpleDrawingCommand
        <CommandMethod("CDS_ShowDatabaseType")> _
        Public Sub CDS_ShowDatabaseType()
            If isCivilDatabase(_database) Then
                write(vbLf & "Current document is a Civil 3D drawing.")
            Else
                write(vbLf & "Current document is an AutoCAD drawing.")
            End If
        End Sub

        Public Function isCivilDatabase(db As Database) As Boolean
            Using tr As Transaction = startTransaction()
                Dim namedObjectDict As DBDictionary = _
                    TryCast(db.NamedObjectsDictionaryId.GetObject( _
                            OpenMode.ForRead), DBDictionary)
                Return namedObjectDict.Contains("Root")
            End Using
        End Function
    End Class
End Namespace
