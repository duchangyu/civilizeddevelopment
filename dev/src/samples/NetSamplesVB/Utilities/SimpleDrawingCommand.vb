Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Civil.ApplicationServices

Namespace Autodesk.CivilizedDevelopment
    ''' <summary>
    ''' Base class for simple command classes that implement commands
    ''' that access a single drawing. This class implements an interface
    ''' with basic, redundant functionality that derived classes can
    ''' leverage to simplify coding.
    ''' </summary>
    Public Class SimpleDrawingCommand
        ''' <summary>
        ''' Returns the document object from where the command was
        ''' launched.
        ''' </summary>
        Protected ReadOnly Property _document() As Document
            Get
                Return Application.DocumentManager.MdiActiveDocument
            End Get
        End Property

        ''' <summary>
        ''' Return the Civil 3D Document instance.
        ''' </summary>
        Protected ReadOnly Property _civildoc() As CivilDocument
            Get
                Return CivilApplication.ActiveDocument
            End Get
        End Property

        ''' <summary>
        ''' Returns the current database from where the command
        ''' was launched.
        ''' </summary>
        Protected ReadOnly Property _database() As Database
            Get
                Return _document.Database
            End Get
        End Property

        ''' <summary>
        ''' Returns the Editor instance for the current document.
        ''' </summary>
        Protected ReadOnly Property _editor() As Editor
            Get
                Return _document.Editor
            End Get
        End Property

        ''' <summary>
        ''' Starts a new transaction in the current database.
        ''' </summary>
        ''' <returns></returns>
        Protected Function startTransaction() As Transaction
            Return _database.TransactionManager.StartTransaction()
        End Function

        ''' <summary>
        ''' Writes the specified message to the Editor output window.
        ''' </summary>
        ''' <param name="message"></param>
        Protected Sub write(message As String)
            _editor.WriteMessage(message)
        End Sub

        Protected Sub logException(ex As Exception)
            While ex IsNot Nothing
                _editor.WriteMessage(ex.Message + ControlChars.Lf)
                ex = ex.InnerException
            End While
        End Sub
    End Class
End Namespace
