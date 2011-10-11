Imports Autodesk.AutoCAD.Runtime

<Assembly: ExtensionApplication( _
    GetType(Autodesk.CivilizedDevelopment.NetSamplesApp))> 

Namespace Autodesk.CivilizedDevelopment
    Public Class NetSamplesApp
        Implements IExtensionApplication
        Public Sub Initialize() _
            Implements IExtensionApplication.Initialize

        End Sub

        Public Sub Terminate() _
            Implements IExtensionApplication.Terminate

        End Sub
    End Class
End Namespace
