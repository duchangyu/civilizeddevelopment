Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Civil.DatabaseServices

<Assembly: CommandClass(
GetType(Autodesk.CivilizedDevelopment.DescriptionKeyCommands))> 

Namespace Autodesk.CivilizedDevelopment
  Public Class DescriptionKeyCommands
    Inherits SimpleDrawingCommand
    <CommandMethod("CDS_CreateCivilDevDescriptionKeys")> _
    Public Sub CDS_CreateCivilDevDescriptionKeys()
      Using tr As Transaction = startTransaction()
        Dim keySetId As ObjectId =
          createOrRetrieveDescriptionKeySet("CivilDev")
        Dim keySet As PointDescriptionKeySet =
          TryCast(keySetId.GetObject(OpenMode.ForWrite), 
            PointDescriptionKeySet)

        Dim treesKeyId As ObjectId = createDescriptionKey(keySet, "TREE")
        Dim key As PointDescriptionKey =
          TryCast(treesKeyId.GetObject(OpenMode.ForWrite), PointDescriptionKey)
        customizeDescriptionKey(key, "Tree", "Description Only", "$0 is a $1")

        Dim wellsKeyId As ObjectId = createDescriptionKey(keySet, "WELL")
        key =
          TryCast(wellsKeyId.GetObject(OpenMode.ForWrite), PointDescriptionKey)
        customizeDescriptionKey(key,
                                "Well", "Point Number Only", "The $0 is $1")

        tr.Commit()
      End Using
    End Sub

    <CommandMethod("CDS_CreateCivilDevDescriptionKeysAndForce")> _
    Public Sub CDS_CreateCivilDevDescriptionKeysAndForce()
      Using tr As Transaction = startTransaction()
        Dim keySetId As ObjectId =
          createOrRetrieveDescriptionKeySetAndForce("CivilDev")
        Dim keySet As PointDescriptionKeySet =
          TryCast(keySetId.GetObject(OpenMode.ForWrite), 
            PointDescriptionKeySet)

        Dim treesKeyId As ObjectId = createDescriptionKey(keySet, "TREE")
        Dim key As PointDescriptionKey =
          TryCast(treesKeyId.GetObject(OpenMode.ForWrite), 
            PointDescriptionKey)
        customizeDescriptionKey(key,
                                "Tree", "Description Only", "$0 is a $1")

        Dim wellsKeyId As ObjectId = createDescriptionKey(keySet, "WELL")
        key = TryCast(wellsKeyId.GetObject(OpenMode.ForWrite), 
          PointDescriptionKey)
        customizeDescriptionKey(key,
                                "Well", "Point Number Only", "The $0 is $1")

        tr.Commit()
      End Using
    End Sub

    <CommandMethod("CDS_CreateCivilDevPoints")> _
    Public Sub CDS_CreateCivilDevPoints()
      Dim generator As New RandomCoordinateGenerator()
      Dim points As CogoPointCollection = _civildoc.CogoPoints

      Dim coordinates As Point3dCollection = generator.GetCoordinates(10)
      points.Add(coordinates, "TREE MAPLE", True, True)

      coordinates = generator.GetCoordinates(10)
      points.Add(coordinates, "WELL DEEP", True, True)
    End Sub

    Private Function createOrRetrieveDescriptionKeySet(name As String) _
        As ObjectId
      Dim keySets As PointDescriptionKeySetCollection =
        PointDescriptionKeySetCollection.
          GetPointDescriptionKeySets(_database)
      If keySets.Contains(name) Then
        Return keySets(name)
      End If
      Return keySets.Add(name)
    End Function

    Private Function createOrRetrieveDescriptionKeySetAndForce(
        name As String) As ObjectId
      Dim keySets As PointDescriptionKeySetCollection =
        PointDescriptionKeySetCollection.
          GetPointDescriptionKeySets(_database)
      Dim searchOrder As ObjectIdCollection = keySets.SearchOrder
      If keySets.Contains(name) Then
        Return keySets(name)
      End If
      Dim newKeySetId As ObjectId = keySets.Add(name)
      searchOrder.Insert(0, newKeySetId)
      keySets.SearchOrder = searchOrder
      Return newKeySetId
    End Function

    Private Function createDescriptionKey(keySet As PointDescriptionKeySet,
                                          code As String) As ObjectId
      If keySet.Contains(code) Then
        Return keySet(code)
      End If
      Return keySet.Add(code)
    End Function

    Private Sub customizeDescriptionKey(key As PointDescriptionKey,
                                        pointStyle As String,
                                        labelStyle As String,
                                        format As String)
      key.StyleId = getPointStyle(pointStyle)
      key.LabelStyleId = getPointLabelStyle(labelStyle)
      key.Format = format
      key.ApplyStyleId = True
      key.ApplyLabelStyleId = True
    End Sub

    Private Function getPointStyle(name As String) As ObjectId
      Return _civildoc.Styles.PointStyles.GetStyle(name)
    End Function

    Private Function getPointLabelStyle(name As String) As ObjectId
      Return _civildoc.Styles.LabelStyles.
        PointLabelStyles.LabelStyles.GetStyle(name)
    End Function
  End Class
End Namespace
