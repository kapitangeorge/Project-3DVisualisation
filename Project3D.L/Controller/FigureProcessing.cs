using Project3D.L.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project3D.L.Controller
{
    public class FigureProcessing
    {
        public static Obj InternalFiling(Obj originalObj)
        {
            var index = 1;
            //var layers = new List<Vertex>();

            var layers = originalObj.Vertices.OrderBy(x => x.Coordinates.Z).ToList();
            while (index < layers.Count)
            {
                var oneLayer = GetOneLayer(layers, ref index);
                var minAndMaxVertex = FindMinAndMax(oneLayer);
                var center = GetCenter(minAndMaxVertex);
                ClockwiseComparer(oneLayer, center);
                GetProblemsPoint(oneLayer);
            }

            return originalObj;
        }

       

        private static List<Vertex> GetOneLayer(List<Vertex> layers, ref int index)
        {
            var oneLayer = new List<Vertex>() { layers[index - 1] };
            while (layers[index].Coordinates.Z == layers[index - 1].Coordinates.Z)
            {
                oneLayer.Add(layers[index]);
                index++;
            }
            return oneLayer;
        }

        private static MinAndMaxVertex FindMinAndMax(List<Vertex> oneLayer)
        {
            Vertex minX, maxX, minY, maxY;
            minX = maxX = minY = maxY = oneLayer[0];
            foreach (var vertex in oneLayer)
            {
                if (vertex.Coordinates.X > maxX.Coordinates.X) maxX = vertex;
                if (vertex.Coordinates.Y > maxY.Coordinates.Y) maxY = vertex;
                if (vertex.Coordinates.X < minX.Coordinates.X) minX = vertex;
                if (vertex.Coordinates.Y < minY.Coordinates.Y) minY = vertex;
            }
            return new MinAndMaxVertex(minX, maxX, minY, maxY);
        }

        private static Vertex GetCenter(MinAndMaxVertex minAndMaxVertex)
        {
            var minXLine = new StraightLine(minAndMaxVertex.MinX, new Vertex(minAndMaxVertex.MinX.Coordinates.X, 0, minAndMaxVertex.MinX.Coordinates.Z));
            var maxXLine = new StraightLine(minAndMaxVertex.MaxX, new Vertex(minAndMaxVertex.MaxX.Coordinates.X, 0, minAndMaxVertex.MaxX.Coordinates.Z));
            var minYLine = new StraightLine(minAndMaxVertex.MinY, new Vertex(0, minAndMaxVertex.MinY.Coordinates.Y, minAndMaxVertex.MinX.Coordinates.Z));
            var maxYLine = new StraightLine(minAndMaxVertex.MaxY, new Vertex(0, minAndMaxVertex.MaxY.Coordinates.Y, minAndMaxVertex.MaxX.Coordinates.Z));

            var minXAndminYVertex = Vertex.IntersectionPointOfTwoLines2D(minXLine, minYLine);
            var minXAndMaxYVertex = Vertex.IntersectionPointOfTwoLines2D(minXLine, maxYLine);
            var maxXAndminYVertex = Vertex.IntersectionPointOfTwoLines2D(maxXLine, minYLine);
            var maxXAndMaxYVertex = Vertex.IntersectionPointOfTwoLines2D(maxXLine, maxYLine);

            var center = Vertex.IntersectionPointOfTwoLines2D(minXAndMaxYVertex, maxXAndminYVertex, minXAndminYVertex, maxXAndMaxYVertex);

            return center;
        }
        private static List<Vertex> GetProblemsPoint(List<Vertex> oneLayer)
        {
            var problemPoints = new List<Vertex>();
            for (var i = 1; i < oneLayer.Count; i++)
            {
                var angleBetweenLines = GetAngle(oneLayer[i - 1], oneLayer[i], oneLayer[i + 1]);
                if (angleBetweenLines < 90) problemPoints.Add(oneLayer[i]);
            }
            return problemPoints;
        }

        private static double GetAngle(Vertex Point1, Vertex Point2, Vertex Point3)
        {
            var Vector1 = new Vertex(Point1.Coordinates.X - Point2.Coordinates.X, Point1.Coordinates.Y - Point2.Coordinates.Y, Point2.Coordinates.Z);
            var Vector2 = new Vertex(Point3.Coordinates.X - Point2.Coordinates.X, Point3.Coordinates.Y - Point2.Coordinates.Y, Point2.Coordinates.Z);
            var scalarProduct = Vector1.Coordinates.X * Vector2.Coordinates.X + Vector1.Coordinates.Y * Vector2.Coordinates.Y;
            var vectorModule1 = Math.Sqrt(Vector1.Coordinates.X * Vector1.Coordinates.X + Vector1.Coordinates.Y * Vector1.Coordinates.Y);
            var vectorModule2 = Math.Sqrt(Vector2.Coordinates.X * Vector2.Coordinates.X + Vector2.Coordinates.Y * Vector2.Coordinates.Y);
            var angle = scalarProduct / (vectorModule1 * vectorModule2);
            return Math.Acos(angle) * 180 / Math.PI;
        }



        public static List<Obj> ColorSeperation(Obj originalObj)
        {
            var ObjSortedByColor = new List<Obj>();
            foreach (var color in originalObj.Colors)
            {
                ObjSortedByColor.Add(new Obj());
            }

            foreach (var face in originalObj.Faces)
            {
                var triangleTops = new List<Vertex>();
                var normals = new List<Normal>();

                foreach (var id in face.Triangle)
                {
                    var i = (Int32)id;
                    triangleTops.Add(originalObj.Vertices[i - 1]);
                    normals.Add(originalObj.Normals[i - 1]);
                }


                int index = FindColorIndex(triangleTops);

                var indexOfVertex = new int[] { ObjSortedByColor[index].Vertices.IndexOf(triangleTops[0]),
                                                ObjSortedByColor[index].Vertices.IndexOf(triangleTops[1]),
                                                ObjSortedByColor[index].Vertices.IndexOf(triangleTops[2])};

                for (var i = 0; i < 3; i++)
                {
                    if (indexOfVertex[i] == -1)
                    {
                        ObjSortedByColor[index].Vertices.Add(triangleTops[i]);
                        indexOfVertex[i] = ObjSortedByColor[index].Vertices.Count;
                        ObjSortedByColor[index].Normals.Add(normals[i]);
                    }
                }
                ObjSortedByColor[index].Faces.Add(new Face(indexOfVertex));
            }

            return ObjSortedByColor;
        }

        private static int FindColorIndex(List<Vertex> triangleTops)
        {
            var index = 0;
            if (triangleTops[0].ColorNumber == triangleTops[1].ColorNumber ||
                triangleTops[0].ColorNumber == triangleTops[2].ColorNumber)
            {
                index = triangleTops[0].ColorNumber - 1;
            }
            else if (triangleTops[1].ColorNumber == triangleTops[2].ColorNumber)
            {
                index = triangleTops[1].ColorNumber - 1;
            }

            return index;
        }

        private static void ClockwiseComparer(List<Vertex> oneLayer, Vertex center)
        {
            var comparer = Comparer<Vertex>.Create((point1, point2) =>
            {
                if (point1.Coordinates.X - center.Coordinates.X >= 0 && point2.Coordinates.X - center.Coordinates.X < 0)
                    return 1;
                if (point1.Coordinates.X - center.Coordinates.X < 0 && point2.Coordinates.X - center.Coordinates.X >= 0)
                    return -1;
                if (point1.Coordinates.X - center.Coordinates.X == 0 && point2.Coordinates.X - center.Coordinates.X == 0)
                {
                    if (point1.Coordinates.Y - center.Coordinates.Y >= 0 || point2.Coordinates.Y - center.Coordinates.Y >= 0)
                        return point1.Coordinates.Y > point2.Coordinates.Y ? 1 : -1;
                    return point2.Coordinates.Y > point1.Coordinates.Y ? 1 : -1;
                }

                double det = (point1.Coordinates.X - center.Coordinates.X) * (point2.Coordinates.Y - center.Coordinates.Y) - (point2.Coordinates.X - center.Coordinates.X) * (point1.Coordinates.Y - center.Coordinates.Y);
                if (det < 0)
                    return 1;
                if (det > 0)
                    return -1;

                double d1 = (point1.Coordinates.X - center.Coordinates.X) * (point1.Coordinates.X - center.Coordinates.X) + (point1.Coordinates.Y - center.Coordinates.Y) * (point1.Coordinates.Y - center.Coordinates.Y);
                double d2 = (point2.Coordinates.X - center.Coordinates.X) * (point2.Coordinates.X - center.Coordinates.X) + (point2.Coordinates.Y - center.Coordinates.Y) * (point2.Coordinates.Y - center.Coordinates.Y);
                return d1 > d2 ? 1 : -1;
            });

            oneLayer.OrderBy(x => x, comparer);
        }
    }
}
