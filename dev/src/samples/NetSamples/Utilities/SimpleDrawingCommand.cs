using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.ApplicationServices;

namespace Autodesk.CivilizedDevelopment
{
    /// <summary>
    /// Base class for simple command classes that implement commands
    /// that access a single drawing. This class implements an interface
    /// with basic, redundant functionality that derived classes can
    /// leverage to simplify coding.
    /// </summary>
    public class SimpleDrawingCommand
    {
        /// <summary>
        /// Returns the document object from where the command was
        /// launched.
        /// </summary>
        protected Document _document
        {
            get
            {
                return Application.DocumentManager.MdiActiveDocument;
            }
        }

        /// <summary>
        /// Return the Civil 3D Document instance.
        /// </summary>
        protected CivilDocument _civildoc
        {
            get
            {
                return CivilApplication.ActiveDocument;
            }
        }

        /// <summary>
        /// Returns the current database from where the command
        /// was launched.
        /// </summary>
        protected Database _database
        {
            get
            {
                return _document.Database;
            }
        }

        /// <summary>
        /// Returns the Editor instance for the current document.
        /// </summary>
        protected Editor _editor
        {
            get
            {
                return _document.Editor;
            }
        }

        /// <summary>
        /// Starts a new transaction in the current database.
        /// </summary>
        /// <returns></returns>
        protected Transaction startTransaction()
        {
            return _database.TransactionManager.StartTransaction();
        }

        /// <summary>
        /// Writes the specified message to the Editor output window.
        /// </summary>
        /// <param name="message"></param>
        protected void write(string message)
        {
            _editor.WriteMessage(message);
        }
    }
}