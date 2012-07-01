using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;

[assembly: CommandClass(
    typeof(Autodesk.CivilizedDevelopment.UserDefinedPropertiesCommands))]

namespace Autodesk.CivilizedDevelopment
{
    public class UserDefinedPropertiesCommands : SimpleDrawingCommand
    {
        [CommandMethod("CDS_UDPDemo")]
        public void CDS_UDPDemo()
        {
            createDemoPointsRepresentingWells();
            createDemoProperties();
            createDemoPointGroup();
            setDemoPointProperties();
        }

        private void createDemoPointsRepresentingWells()
        {
            RandomCoordinateGenerator generator = 
                new RandomCoordinateGenerator();
            _civildoc.CogoPoints.Add(generator.GetCoordinates(10), "WELL");
        }

        private void createDemoProperties()
        {
            _classification = createClassification("CivilizedDevelopment");
            addUDPs(_classification);
        }

        private void createDemoPointGroup()
        {
            
            ObjectId id = _civildoc.PointGroups.Add("CivilizedDevelopment");
            using (Transaction tr = startTransaction())
            {
                customizePointGroup(id);
                tr.Commit();
            }
            
        }

        private void setDemoPointProperties()
        {
            using (Transaction tr = startTransaction())
            {
                foreach (ObjectId pointId in _civildoc.CogoPoints)
                {
                    customizePoint(pointId);
                }

                tr.Commit();
            }
        }

        private void customizePointGroup(ObjectId id)
        {
            PointGroup group = id.GetObject(OpenMode.ForWrite) as PointGroup;
            CustomPointGroupQuery query = new CustomPointGroupQuery();
            query.QueryString = "RawDescription='WELL*'";
            group.SetQuery(query);
            group.UseCustomClassification(_classification);
        }

        private void customizePoint(ObjectId id)
        {
            CogoPoint point = id.GetObject(OpenMode.ForWrite) as CogoPoint;
            point.SetUDPValue(_potable, true);
            point.SetUDPValue(_chloride, 150);
            point.SetUDPValue(_laboratory, "CivilDev Labs, Inc.");
            point.SetUDPValue(_hardness, "Carbonate (Temporary)");
            point.SetUDPValue(_alkalinity, 7.5);
        }

        private UDPClassification createClassification(string name)
        {
            UDPClassificationCollection classifications =
                UDPClassificationCollection
                .GetPointUDPClassifications(_database);
            return classifications.Add(name);
        }

        private void addUDPs(UDPClassification classification)
        {
            _potable = classification.CreateUDP(makeBoolDefinition("Potable", 
                "Indicates if water is suitable for consumption."));
            _chloride = classification.CreateUDP(makeIntDefinition("Chloride", 
                "Amount of chloride (mg/l)."));
            _laboratory = classification.CreateUDP(
                makeStringDefinition("Laboratory", 
                "Name of the laboratory that performed analysis."));
            _hardness = classification.CreateUDP(makeEnumDefinition("Hardness", 
                "Hardness with respect to anions in metallic cations."));
            _alkalinity = classification.CreateUDP(
                makeDoubleDefinition("Alkalinity", 
                "Ability to resist sudden changes in pH."));
        }

        private AttributeTypeInfoBool makeBoolDefinition(
            string name, string description)
        {
            AttributeTypeInfoBool definition = new AttributeTypeInfoBool(name);
            definition.Description = description;
            definition.DefaultValue = false;
            definition.UseDefaultValue = true;
            return definition;
        }

        private AttributeTypeInfoInt makeIntDefinition(
            string name, string description)
        {
            AttributeTypeInfoInt definition = new AttributeTypeInfoInt(name);
            definition.Description = description;
            definition.DefaultValue = 0;
            definition.LowerBoundValue = Int32.MinValue;
            definition.LowerBoundInclusive = true;
            definition.UpperBoundValue = Int32.MaxValue;
            definition.UpperBoundInclusive = true;
            definition.UseDefaultValue = true;
            return definition;
        }

        private AttributeTypeInfoString makeStringDefinition(
            string name, string description)
        {
            AttributeTypeInfoString definition = 
                new AttributeTypeInfoString(name);
            definition.Description = description;
            definition.DefaultValue = String.Empty;
            definition.UseDefaultValue = true;
            return definition;
        }

        private AttributeTypeInfoEnum makeEnumDefinition(
            string name, string description)
        {
            EnumDefinition valueDefinitions = new EnumDefinition();
            AttributeTypeInfoEnum definition = 
                new AttributeTypeInfoEnum(name, valueDefinitions.Values);
            definition.Description = description;
            definition.DefaultValue = valueDefinitions.DefaultValue;
            definition.UseDefaultValue = true;
            return definition;
        }

        private AttributeTypeInfoDouble makeDoubleDefinition(
            string name, string description)
        {
            AttributeTypeInfoDouble definition =
                new AttributeTypeInfoDouble(name);
            definition.Description = description;
            definition.DefaultValue = 0.0;
            definition.UseDefaultValue = true;
            definition.LowerBoundValue = 0.0;
            definition.LowerBoundInclusive = true;
            definition.UpperBoundValue = Double.MaxValue;
            definition.UpperBoundInclusive = true;
            definition.UseDefaultValue = true;
            definition.DataType = AttributeTypeInfoDoubleDataType.Percent;
            return definition;
        }

        private UDPClassification _classification;
        private UDPBoolean _potable;
        private UDPInteger _chloride;
        private UDPString _laboratory;
        private UDPEnumeration _hardness;
        private UDPDouble _alkalinity;

        class EnumDefinition
        {
            public EnumDefinition()
            {
                _values = new List<string>();
                _values.Add("Carbonate (Temporary)");
                _values.Add("Non-Carbonate (Permanent)");
            }

            public IEnumerable <string> Values
            {
                get
                {
                    return _values;
                }
            }

            public string DefaultValue
            {
                get
                {
                    return _values[0];
                }
            }

            private List<string> _values;
        }
    }
}