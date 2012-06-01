using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace Autodesk.CivilizedDevelopment
{
    internal static class ObjectIdEnumerableExtensions
    {
        public static ObjectId GetStyle(this IEnumerable<ObjectId> ids, 
            string name)
        {
            foreach (ObjectId id in ids)
            {
                StyleBase style = id.GetObject(OpenMode.ForRead) 
                    as StyleBase;
                if (style.Name == name)
                {
                    return id;
                }
            }
            return ObjectId.Null;
        }
    }
}