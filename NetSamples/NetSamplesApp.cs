using System;

using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(Autodesk.CivilizedDevelopment.NetSamplesApp))]

namespace Autodesk.CivilizedDevelopment
{
    public class NetSamplesApp : IExtensionApplication
    {
        public void Initialize()
        { }

        public void Terminate()
        { }
    }
}