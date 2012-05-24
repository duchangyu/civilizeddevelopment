Imports Autodesk.AutoCAD.Geometry

Namespace Autodesk.CivilizedDevelopment
  Public Class RandomCoordinateGenerator
    Public Sub New()
      MaxX = 1000.0
      MaxY = 1000.0
      MaxZ = 100.0
      initGenerator()
    End Sub

    Public Sub New(maxx__1 As Double, maxy__2 As Double, maxz__3 As Double)
      MaxX = maxx__1
      MaxY = maxy__2
      MaxZ = maxz__3
      initGenerator()
    End Sub

    Public Property MaxX() As Double
      Get
        Return m_MaxX
      End Get
      Set(value As Double)
        m_MaxX = Value
      End Set
    End Property
    Private m_MaxX As Double
    Public Property MaxY() As Double
      Get
        Return m_MaxY
      End Get
      Set(value As Double)
        m_MaxY = Value
      End Set
    End Property
    Private m_MaxY As Double
    Public Property MaxZ() As Double
      Get
        Return m_MaxZ
      End Get
      Set(value As Double)
        m_MaxZ = Value
      End Set
    End Property
    Private m_MaxZ As Double

    Public Function GetCoordinate() As Point3d
      Dim x As Double = m_Generator.NextDouble() * MaxX
      Dim y As Double = m_Generator.NextDouble() * MaxY
      Dim z As Double = m_Generator.NextDouble() * MaxZ
      Return New Point3d(x, y, z)
    End Function

    Public Function GetCoordinates(count As Integer) As Point3dCollection
      Dim result As New Point3dCollection()
      For i As Integer = 0 To count - 1
        result.Add(GetCoordinate())
      Next

      Return result
    End Function


    Private Sub initGenerator()
      'Dim seed As Integer = CInt(DateTime.Now.Ticks)
      m_Generator = New Random()
    End Sub

    Private m_Generator As Random
  End Class
End Namespace
