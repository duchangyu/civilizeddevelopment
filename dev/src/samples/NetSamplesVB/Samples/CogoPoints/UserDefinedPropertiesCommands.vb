Imports System.Collections.Generic

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.DatabaseServices

<Assembly: CommandClass(GetType(Autodesk.CivilizedDevelopment.UserDefinedPropertiesCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class UserDefinedPropertiesCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_UDPDemo")> _
    Public Sub CDS_UDPDemo()
      createDemoPointsRepresentingWells()
      createDemoProperties()
      createDemoPointGroup()
      setDemoPointProperties()
    End Sub

    Private Sub createDemoPointsRepresentingWells()
      Dim generator As New RandomCoordinateGenerator()
      _civildoc.CogoPoints.Add(generator.GetCoordinates(10), "WELL")
    End Sub

    Private Sub createDemoProperties()
      _classification = createClassification("CivilizedDevelopment")
      addUDPs(_classification)
    End Sub

    Private Sub createDemoPointGroup()

      Dim id As ObjectId = _civildoc.PointGroups.Add("CivilizedDevelopment")
      Using tr As Transaction = startTransaction()
        customizePointGroup(id)
        tr.Commit()
      End Using

    End Sub

    Private Sub setDemoPointProperties()
      Using tr As Transaction = startTransaction()
        For Each pointId As ObjectId In _civildoc.CogoPoints
          customizePoint(pointId)
        Next

        tr.Commit()
      End Using
    End Sub

    Private Sub customizePointGroup(id As ObjectId)
      Dim group As PointGroup = TryCast(id.GetObject(OpenMode.ForWrite), 
        PointGroup)
      Dim query As New CustomPointGroupQuery()
      query.QueryString = "RawDescription='WELL*'"
      group.SetQuery(query)
      group.UseCustomClassification(_classification)
    End Sub

    Private Sub customizePoint(id As ObjectId)
      Dim point As CogoPoint = TryCast(id.GetObject(OpenMode.ForWrite), 
        CogoPoint)
      point.SetUDPValue(_potable, True)
      point.SetUDPValue(_chloride, 150)
      point.SetUDPValue(_laboratory, "CivilDev Labs, Inc.")
      point.SetUDPValue(_hardness, "Carbonate (Temporary)")
      point.SetUDPValue(_alkalinity, 7.5)
    End Sub

    Private Function createClassification(name As String) As UDPClassification
      Dim classifications As UDPClassificationCollection =
        UDPClassificationCollection.GetPointUDPClassifications(_database)
      Return classifications.Add(name)
    End Function

    Private Sub addUDPs(classification As UDPClassification)
      _potable = classification.CreateUDP(
        makeBoolDefinition("Potable",
          "Indicates if water is suitable for consumption."))
      _chloride = classification.CreateUDP(
        makeIntDefinition("Chloride", "Amount of chloride (mg/l)."))
      _laboratory = classification.CreateUDP(
        makeStringDefinition("Laboratory",
          "Name of the laboratory that performed analysis."))
      _hardness = classification.CreateUDP(
        makeEnumDefinition("Hardness",
          "Hardness with respect to anions in metallic cations."))
      _alkalinity = classification.CreateUDP(
        makePlainDoubleDefinition("Alkalinity",
          "Ability to resist sudden changes in pH."))
    End Sub

    Private Function makeBoolDefinition(name As String, description As String) _
        As AttributeTypeInfoBool

      Dim definition As New AttributeTypeInfoBool(name)
      definition.Description = description
      definition.DefaultValue = False
      definition.UseDefaultValue = True
      Return definition
    End Function

    Private Function makeIntDefinition(name As String, description As String) _
        As AttributeTypeInfoInt

      Dim definition As New AttributeTypeInfoInt(name)
      definition.Description = description
      definition.DefaultValue = 0
      definition.LowerBoundValue = Int32.MinValue
      definition.LowerBoundInclusive = True
      definition.UpperBoundValue = Int32.MaxValue
      definition.UpperBoundInclusive = True
      definition.UseDefaultValue = True
      Return definition
    End Function

    Private Function makeStringDefinition(name As String,
        description As String) As AttributeTypeInfoString

      Dim definition As New AttributeTypeInfoString(name)
      definition.Description = description
      definition.DefaultValue = [String].Empty
      definition.UseDefaultValue = True
      definition.Description = description
      Return definition
    End Function

    Private Function makeEnumDefinition(name As String,
        description As String) As AttributeTypeInfoEnum

      Dim valueDefinitions As New EnumDefinition()
      Dim definition As New AttributeTypeInfoEnum(name, valueDefinitions.Values)
      definition.Description = description
      definition.DefaultValue = valueDefinitions.DefaultValue
      definition.UseDefaultValue = True
      Return definition
    End Function

    Private Function makePlainDoubleDefinition(name As String,
        description As String) As AttributeTypeInfoDouble

      Dim definition As New AttributeTypeInfoDouble(name)
      definition.Description = description
      definition.DefaultValue = 0.0
      definition.UseDefaultValue = True
      definition.LowerBoundValue = 0.0
      definition.LowerBoundInclusive = True
      definition.UpperBoundValue = [Double].MaxValue
      definition.UpperBoundInclusive = True
      definition.UseDefaultValue = True
      Return definition
    End Function

    Private _classification As UDPClassification
    Private _potable As UDPBoolean
    Private _chloride As UDPInteger
    Private _laboratory As UDPString
    Private _hardness As UDPEnumeration
    Private _alkalinity As UDPDouble

    Private Class EnumDefinition
      Public Sub New()
        _values = New List(Of String)()
        _values.Add("Carbonate (Temporary)")
        _values.Add("Non-Carbonate (Permanent)")
      End Sub

      Public ReadOnly Property Values() As IEnumerable(Of String)
        Get
          Return _values
        End Get
      End Property

      Public ReadOnly Property DefaultValue() As String
        Get
          Return _values(0)
        End Get
      End Property

      Private _values As List(Of String)
    End Class
  End Class
End Namespace
