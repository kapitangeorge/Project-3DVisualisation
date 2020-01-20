using Project3D.L.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project3D.L.Controller
{
    public class FigureProcessing
    {
        public static Obj ConnectionOfLayers(Obj originalObj)
        {
            var index = 1;
            var colorsCount = originalObj.Colors.Count;
            var layers = originalObj.Vertices.OrderBy(x => x.Coordinates.Z).ToList();
            var countLayers = layers.Count;
            while (index < countLayers)
            {
                var oneLayer = GetOneLayer(layers, ref index);
                var nextI = index++;
                var oneLayerNext = GetOneLayer(layers, ref nextI);
                //var br = 0;
                if (oneLayer.Count >= 3 && oneLayerNext.Count >= 3)
                {
                    var incidLine = new List<StraightLine>();

                    var internalBorders = new List<StraightLine>();
                    var internalPoints = new List<Vertex>();
                    InternalFiling(originalObj, oneLayer, internalBorders, internalPoints);

                    var internalBordersNext = new List<StraightLine>();
                    var internalPointsNext = new List<Vertex>();
                    InternalFiling(originalObj, oneLayerNext, internalBordersNext, internalPointsNext);

                    int[,] matrix = new int[oneLayer.Count, oneLayerNext.Count];
                    IncidenceMatrix(originalObj, matrix, oneLayer, oneLayerNext);

                    //создание внутренних отрезков, если у внешних точек есть ребро(инцидентные вершины)
                    for (var i = 0; i < internalPoints.Count; i++)
                        for (var j = 0; j < internalPointsNext.Count; j++)
                        {
                            if (matrix[i, j] == 1)
                                incidLine.Add(new StraightLine()
                                {
                                    Number1 = internalPoints[i].Number,
                                    Number2 = internalPointsNext[j].Number,
                                    Point1 = internalPoints[i],
                                    Point2 = internalPointsNext[j]
                                });
                        }

                    Vertex defaultPoint;
                    Vertex defaultPoint2;
                    Vertex defaultPoint3;
                    //создание внутренних полигонов
                    for (var i = 0; i < incidLine.Count - 1; i++)
                    {
                        if (incidLine[i].Number1 == incidLine[i + 1].Number1)
                        {
                            originalObj.Faces.Add(new Face(new int[] { incidLine[i].Number1, incidLine[i].Number2, incidLine[i + 1].Number2 }));
                            defaultPoint = CreateDefault(originalObj, incidLine[i], colorsCount, 1);
                            defaultPoint2 = CreateDefault(originalObj, incidLine[i], colorsCount, 2);
                            defaultPoint3 = CreateDefault(originalObj, incidLine[i + 1], colorsCount, 2);
                            originalObj.Faces.Add(new Face(new int[] { defaultPoint.Number, defaultPoint2.Number, defaultPoint3.Number }));
                        }
                        else if (incidLine[i].Number1 == incidLine[i + 1].Number2)
                        {
                            originalObj.Faces.Add(new Face(new int[] { incidLine[i].Number1, incidLine[i].Number2, incidLine[i + 1].Number1 }));
                            defaultPoint = CreateDefault(originalObj, incidLine[i], colorsCount, 1);
                            defaultPoint2 = CreateDefault(originalObj, incidLine[i], colorsCount, 2);
                            defaultPoint3 = CreateDefault(originalObj, incidLine[i + 1], colorsCount, 1);
                            originalObj.Faces.Add(new Face(new int[] { defaultPoint.Number, defaultPoint2.Number, defaultPoint3.Number }));
                        }
                        else if (incidLine[i].Number2 == incidLine[i + 1].Number1)
                        {
                            originalObj.Faces.Add(new Face(new int[] { incidLine[i].Number1, incidLine[i].Number2, incidLine[i + 1].Number2 }));
                            defaultPoint = CreateDefault(originalObj, incidLine[i], colorsCount, 1);
                            defaultPoint2 = CreateDefault(originalObj, incidLine[i], colorsCount, 2);
                            defaultPoint3 = CreateDefault(originalObj, incidLine[i + 1], colorsCount, 2);
                            originalObj.Faces.Add(new Face(new int[] { defaultPoint.Number, defaultPoint2.Number, defaultPoint3.Number }));
                        }
                        else if (incidLine[i].Number2 == incidLine[i + 1].Number2)
                        {
                            originalObj.Faces.Add(new Face(new int[] { incidLine[i].Number1, incidLine[i].Number2, incidLine[i + 1].Number1 }));
                            defaultPoint = CreateDefault(originalObj, incidLine[i], colorsCount, 1);
                            defaultPoint2 = CreateDefault(originalObj, incidLine[i], colorsCount, 2);
                            defaultPoint3 = CreateDefault(originalObj, incidLine[i + 1], colorsCount, 1);
                            originalObj.Faces.Add(new Face(new int[] { defaultPoint.Number, defaultPoint2.Number, defaultPoint3.Number }));
                        }
                    }


                    //создание перегородок
                    if (internalBorders.Count == internalBordersNext.Count)
                        for (var border = 0; border < internalBorders.Count; border++)
                        {
                            var colorVert = internalBorders[border].Point1.ColorNumber;
                            var colorVertNext = internalBordersNext[border].Point1.ColorNumber;
                            if (colorVert == colorVertNext)
                            {
                                originalObj.Faces.Add(new Face(new int[] { internalBorders[border].Number1,
                                                                           internalBordersNext[border].Number2,
                                                                           internalBorders[border].Number2 }));
                                originalObj.Faces.Add(new Face(new int[] { internalBordersNext[border].Number1,
                                                                           internalBorders[border].Number1,
                                                                           internalBordersNext[border].Number2}));
                            }
                        }
                    else
                    {
                        if (internalBorders.Count != 0)
                        {
                            if (internalBorders[0].Point1.ColorNumber == internalBorders[3].Point1.ColorNumber)
                            {
                                originalObj.Faces.Add(new Face(new int[] { internalBorders[0].Number1,
                                                                           internalBorders[0].Number2,
                                                                           internalBorders[3].Number2 }));
                                originalObj.Faces.Add(new Face(new int[] { internalBorders[3].Number1,
                                                                           internalBorders[3].Number2,
                                                                           internalBorders[0].Number1 }));
                            }
                            else if (internalBorders[1].Point1.ColorNumber == internalBorders[2].Point1.ColorNumber)
                            {
                                originalObj.Faces.Add(new Face(new int[] { internalBorders[1].Number1,
                                                                           internalBorders[1].Number2,
                                                                           internalBorders[2].Number2 }));
                                originalObj.Faces.Add(new Face(new int[] { internalBorders[2].Number1,
                                                                           internalBorders[2].Number2,
                                                                           internalBorders[1].Number1 }));
                            }
                        }
                        else
                            if (internalBordersNext[0].Point1.ColorNumber == internalBordersNext[internalBordersNext.Count - 1].Point1.ColorNumber)
                        {
                            originalObj.Faces.Add(new Face(new int[] { internalBordersNext[0].Number1,
                                                                           internalBordersNext[0].Number2,
                                                                           internalBordersNext[3].Number2 }));
                            originalObj.Faces.Add(new Face(new int[] { internalBordersNext[3].Number1,
                                                                           internalBordersNext[3].Number2,
                                                                           internalBordersNext[0].Number1 }));
                        }
                        else if (internalBordersNext[1].Point1.ColorNumber == internalBordersNext[2].Point1.ColorNumber)
                        {
                            originalObj.Faces.Add(new Face(new int[] { internalBordersNext[1].Number1,
                                                                           internalBordersNext[1].Number2,
                                                                           internalBordersNext[2].Number2 }));
                            originalObj.Faces.Add(new Face(new int[] { internalBordersNext[2].Number1,
                                                                           internalBordersNext[2].Number2,
                                                                           internalBordersNext[1].Number1 }));
                        }

                    }

                    //br++;
                }
                /*if (br == 1)
                    break;*/
            }
            return originalObj;
        }

        private static Vertex CreateDefault(Obj originalObj, StraightLine segm, int colorsCount, int den)
        {
            Vertex defaultPoint;
            if (den == 1)
            {
                defaultPoint = new Vertex(originalObj.Vertices[segm.Number1].Coordinates.X,
                                                      originalObj.Vertices[segm.Number1].Coordinates.Y,
                                                      originalObj.Vertices[segm.Number1].Coordinates.Z,
                                                      colorsCount + 1,
                                                      originalObj.Vertices.Count);
                originalObj.Vertices.Add(defaultPoint);
                originalObj.Normals.Add(new Normal(originalObj.Vertices[segm.Number1].Coordinates.X,
                                                   originalObj.Vertices[segm.Number1].Coordinates.Y,
                                                   originalObj.Vertices[segm.Number1].Coordinates.Z,
                                                   defaultPoint.Number));
            }
            else
            {
                defaultPoint = new Vertex(originalObj.Vertices[segm.Number2].Coordinates.X,
                                                      originalObj.Vertices[segm.Number2].Coordinates.Y,
                                                      originalObj.Vertices[segm.Number2].Coordinates.Z,
                                                      colorsCount + 1,
                                                      originalObj.Vertices.Count);
                originalObj.Vertices.Add(defaultPoint);
                originalObj.Normals.Add(new Normal(originalObj.Vertices[segm.Number2].Coordinates.X,
                                                   originalObj.Vertices[segm.Number2].Coordinates.Y,
                                                   originalObj.Vertices[segm.Number2].Coordinates.Z,
                                                   defaultPoint.Number));
            }
            return defaultPoint;
        }

        private static void InternalFiling(Obj originalObj, List<Vertex> oneLayer, List<StraightLine> internalBorders, List<Vertex> internalPoints)
        {
            var minAndMaxVertex = FindMinAndMax(oneLayer);
            var centerPoint = GetCenter(minAndMaxVertex);
            ClockwiseComparer(oneLayer, centerPoint);
            var problemPoints = GetProblemsPoint(oneLayer);
            var normals = originalObj.Normals;
            var colorsCount = originalObj.Colors.Count;
            var vertexBorder = new Vertex[2 * colorsCount];

            //запоминаем границы цвета
            for (var i = 0; i < oneLayer.Count - 1; i++)
            {
                if (oneLayer[i].ColorNumber != oneLayer[i + 1].ColorNumber)
                    for (var e = 0; e < vertexBorder.Length - 1; e++)
                    {
                        vertexBorder[e] = oneLayer[i];
                        vertexBorder[e + 1] = oneLayer[i + 1];
                    }
                else if (oneLayer[i].ColorNumber != oneLayer[oneLayer.Count - 1].ColorNumber)
                    for (var e = 0; e < vertexBorder.Length - 1; e++)
                    {
                        vertexBorder[e] = oneLayer[oneLayer.Count - 1];
                        vertexBorder[e + 1] = oneLayer[i];
                    }
            }

            //если невыпуклая фигура
            if (problemPoints.Count == 2)
            {
                for (var i = 0; i < oneLayer.Count; i++)
                {
                    if (oneLayer[i] == problemPoints[0] || oneLayer[i] == problemPoints[1])
                    {

                        if (i == 0)
                        {
                            var cross1 = IntersectionOfLines(oneLayer[i], oneLayer[i + 1], ReflectNormal(normals[i]), ReflectNormal(normals[i + 1]));
                            var cross2 = IntersectionOfLines(oneLayer[i], oneLayer[oneLayer.Count - 1], ReflectNormal(normals[i]), ReflectNormal(normals[oneLayer.Count - 1]));
                            var len1 = LengthOfSegment(oneLayer[i], oneLayer[i + 1], normals[i], normals[i + 1]);
                            var len2 = LengthOfSegment(oneLayer[i], oneLayer[oneLayer.Count - 1], normals[i], normals[oneLayer.Count - 1]);
                            var nearestPoint = NearestIntersection(len1, len2, cross1, cross2, oneLayer[i]);

                            internalPoints.Add(CreateInnerVertices(originalObj, nearestPoint, oneLayer[i], oneLayer[i].ColorNumber, originalObj.Vertices.Count, 1));
                            for (var e = 0; e < vertexBorder.Length; e++)
                            {
                                if (oneLayer[i] == vertexBorder[e])
                                    internalBorders.Add(new StraightLine()
                                    {
                                        Point1 = vertexBorder[e],
                                        Point2 = originalObj.Vertices[originalObj.Vertices.Count - 1],
                                        Number1 = vertexBorder[e].Number,
                                        Number2 = originalObj.Vertices.Count - 1
                                    });
                            }

                        }
                        else if ((i >= 1) && (i < oneLayer.Count - 1))
                        {
                            var cross1 = IntersectionOfLines(oneLayer[i], oneLayer[i + 1], ReflectNormal(normals[i]), ReflectNormal(normals[i + 1]));
                            var cross2 = IntersectionOfLines(oneLayer[i], oneLayer[i - 1], ReflectNormal(normals[i]), ReflectNormal(normals[i - 1]));
                            var len1 = LengthOfSegment(oneLayer[i], oneLayer[i + 1], normals[i], normals[i + 1]);
                            var len2 = LengthOfSegment(oneLayer[i], oneLayer[i - 1], normals[i], normals[i - 1]);
                            var nearestPoint = NearestIntersection(len1, len2, cross1, cross2, oneLayer[i]);

                            internalPoints.Add(CreateInnerVertices(originalObj, nearestPoint, oneLayer[i], oneLayer[i].ColorNumber, originalObj.Vertices.Count, 1));
                            for (var e = 0; e < vertexBorder.Length; e++)
                            {
                                if (oneLayer[i] == vertexBorder[e])
                                    internalBorders.Add(new StraightLine()
                                    {
                                        Point1 = vertexBorder[e],
                                        Point2 = originalObj.Vertices[originalObj.Vertices.Count - 1],
                                        Number1 = vertexBorder[e].Number,
                                        Number2 = originalObj.Vertices.Count - 1
                                    });
                            }
                        }
                        else if (i == oneLayer.Count - 1)
                        {
                            var cross1 = IntersectionOfLines(oneLayer[i], oneLayer[0], ReflectNormal(normals[i]), ReflectNormal(normals[0]));
                            var cross2 = IntersectionOfLines(oneLayer[i], oneLayer[oneLayer.Count - 2], ReflectNormal(normals[i]), ReflectNormal(normals[oneLayer.Count - 2]));
                            var len1 = LengthOfSegment(oneLayer[i], oneLayer[0], normals[i], normals[0]);
                            var len2 = LengthOfSegment(oneLayer[i], oneLayer[oneLayer.Count - 2], normals[i], normals[oneLayer.Count - 2]);
                            var nearestPoint = NearestIntersection(len1, len2, cross1, cross2, oneLayer[i]);

                            internalPoints.Add(CreateInnerVertices(originalObj, nearestPoint, oneLayer[i], oneLayer[i].ColorNumber, originalObj.Vertices.Count, 1));
                            for (var e = 0; e < vertexBorder.Length; e++)
                            {
                                if (oneLayer[i] == vertexBorder[e])
                                    internalBorders.Add(new StraightLine()
                                    {
                                        Point1 = vertexBorder[e],
                                        Point2 = originalObj.Vertices[originalObj.Vertices.Count - 1],
                                        Number1 = vertexBorder[e].Number,
                                        Number2 = originalObj.Vertices.Count - 1
                                    });
                            }
                        }

                    }
                    else
                        for (var j = i; j < oneLayer.Count - 1; j++)
                        {

                            var normal1 = new Normal(x: oneLayer[j + 1].Coordinates.X - oneLayer[j].Coordinates.X,
                                                     y: oneLayer[j + 1].Coordinates.Y - oneLayer[j].Coordinates.Y,
                                                     z: oneLayer[j].Coordinates.Z,
                                                     number: normals.Count);
                            var pointOfIntersection = IntersectionOfLines(oneLayer[i], oneLayer[j], normals[i], normal1);

                            if (PointBelongToSegm(pointOfIntersection, oneLayer[j], oneLayer[j + 1]))
                            {
                                internalPoints.Add(CreateInnerVertices(originalObj, pointOfIntersection, oneLayer[i], oneLayer[i].ColorNumber, originalObj.Vertices.Count, 450));

                                for (var e = 0; e < vertexBorder.Length; e++)
                                {
                                    if (oneLayer[i] == vertexBorder[e])
                                        internalBorders.Add(new StraightLine()
                                        {
                                            Point1 = vertexBorder[e],
                                            Point2 = originalObj.Vertices[originalObj.Vertices.Count - 1],
                                            Number1 = vertexBorder[e].Number,
                                            Number2 = originalObj.Vertices.Count - 1
                                        });
                                }
                            }
                        }
                }
            }
            //для выпуклой фигуры
            else
            {
                for (var number = 0; number < oneLayer.Count; number++)
                {
                    internalPoints.Add(CreateInnerVertices(originalObj, centerPoint.Coordinates, oneLayer[number], oneLayer[number].ColorNumber, originalObj.Vertices.Count, 300));
                    for (var e = 0; e < vertexBorder.Length; e++)
                        if (oneLayer[number] == vertexBorder[e])
                            internalBorders.Add(new StraightLine()
                            {
                                Point1 = vertexBorder[e],
                                Point2 = originalObj.Vertices[originalObj.Vertices.Count - 1],
                                Number1 = vertexBorder[e].Number,
                                Number2 = originalObj.Vertices.Count - 1
                            });
                }
            }
        }

        private static void IncidenceMatrix(Obj originalObj, int[,] matrix, List<Vertex> oneLayer, List<Vertex> oneLayerNext)
        {
            var FaceBetweenLayers = new List<Face>();
            var z1 = oneLayer[0].Coordinates.Z;
            var z2 = oneLayerNext[0].Coordinates.Z;
            foreach (var face in originalObj.Faces)
            {
                var id1 = (int)face.Triangle[0];
                var id2 = (int)face.Triangle[1];
                var id3 = (int)face.Triangle[2];
                if (id1 > 0 && id2 > 0 && id3 > 0)
                    if ((originalObj.Vertices[id1 - 1].Coordinates.Z == z1 && originalObj.Vertices[id2 - 1].Coordinates.Z == z1) ||
                    (originalObj.Vertices[id2 - 1].Coordinates.Z == z1 && originalObj.Vertices[id3 - 1].Coordinates.Z == z1) ||
                    (originalObj.Vertices[id1 - 1].Coordinates.Z == z1 && originalObj.Vertices[id3 - 1].Coordinates.Z == z1) ||
                    (originalObj.Vertices[id1 - 1].Coordinates.Z == z2 && originalObj.Vertices[id2 - 1].Coordinates.Z == z2) ||
                    (originalObj.Vertices[id2 - 1].Coordinates.Z == z2 && originalObj.Vertices[id3 - 1].Coordinates.Z == z2) ||
                    (originalObj.Vertices[id1 - 1].Coordinates.Z == z2 && originalObj.Vertices[id3 - 1].Coordinates.Z == z2))
                        FaceBetweenLayers.Add(face);
            }

            for (var i = 0; i < oneLayer.Count; i++)
                for (var j = 0; j < oneLayerNext.Count; j++)
                {
                    foreach (var face in FaceBetweenLayers)
                    {
                        if ((face.Triangle[0] == oneLayer[i].Number && face.Triangle[1] == oneLayerNext[j].Number) ||
                            (face.Triangle[0] == oneLayer[i].Number && face.Triangle[2] == oneLayerNext[j].Number) ||
                            (face.Triangle[1] == oneLayer[i].Number && face.Triangle[0] == oneLayerNext[j].Number) ||
                            (face.Triangle[1] == oneLayer[i].Number && face.Triangle[2] == oneLayerNext[j].Number) ||
                            (face.Triangle[2] == oneLayer[i].Number && face.Triangle[0] == oneLayerNext[j].Number) ||
                            (face.Triangle[2] == oneLayer[i].Number && face.Triangle[1] == oneLayerNext[j].Number))
                        {
                            matrix[i, j] = 1;
                            Console.WriteLine($"вершина 1 слоя:{oneLayer[i].Number}   вершина 2 слоя:{oneLayerNext[j].Number}");
                            break;
                        }
                    }
                }
        }

        private static double LengthOfSegment(Vertex v1, Vertex v2, Normal n1, Normal n2)
        {
            var cross = IntersectionOfLines(v1, v2, ReflectNormal(n1), ReflectNormal(n2));
            return Math.Sqrt((v1.Coordinates.X - cross.X) * (v1.Coordinates.X - cross.X) + ((v1.Coordinates.Y - cross.Y) * (v1.Coordinates.Y - cross.Y)));

        }

        private static Coordinates NearestIntersection(double len1, double len2, Coordinates cross1, Coordinates cross2, Vertex vertexOfLayer)
        {
            var cross = new Coordinates();
            if (len1 < len2)
            {
                cross.X = cross1.X;
                cross.Y = cross1.Y;
                cross.Z = vertexOfLayer.Coordinates.Z;
            }
            else
            {
                cross.X = cross2.X;
                cross.Y = cross2.Y;
                cross.Z = vertexOfLayer.Coordinates.Z;
            }
            return cross;
        }

        private static Coordinates IntersectionOfLines(Vertex vertex1, Vertex vertex2, Normal normal1, Normal normal2)
        {
            var result = new Coordinates();
            var a1 = normal1.Coordinates.Y;
            var a2 = normal2.Coordinates.Y;
            var b1 = -normal1.Coordinates.X;
            var b2 = -normal2.Coordinates.X;
            var c1 = (normal1.Coordinates.X * vertex1.Coordinates.Y) - (normal1.Coordinates.Y * vertex1.Coordinates.X);
            var c2 = (normal2.Coordinates.X * vertex2.Coordinates.Y) - (normal2.Coordinates.Y * vertex2.Coordinates.X);

            var den = a1 * b2 - a2 * b1;

            if (den != 0)
            {
                result.X = ((b1 * c2) - (b2 * c1)) / den;
                result.Y = ((a2 * c1) - (a1 * c2)) / den;
            }

            return result;
        }

        private static Normal ReflectNormal(Normal normal)
        {
            normal.Coordinates.X *= -1;
            normal.Coordinates.Y *= -1;

            return normal;
        }

        private static bool PointBelongToSegm(Coordinates pointOfIntersection, Vertex vertexOfSegm1, Vertex vertexOfSegm2)
        {
            return (((pointOfIntersection.X - vertexOfSegm1.Coordinates.X) / (vertexOfSegm2.Coordinates.X - vertexOfSegm1.Coordinates.X)) ==
                                        ((pointOfIntersection.Y - vertexOfSegm1.Coordinates.Y) / vertexOfSegm2.Coordinates.Y - vertexOfSegm1.Coordinates.Y)) ? true : false;
        }

        private static Vertex CreateInnerVertices(Obj originalObj, Coordinates vertex1, Vertex vertex2, int colorsCount, int verticesCount, int den)
        {

            if (den == 1)
            {
                var internalPoint = new Vertex(vertex1.X,
                                                  vertex1.Y,
                                                  vertex2.Coordinates.Z,
                                                  colorsCount,
                                                  verticesCount);
                originalObj.Vertices.Add(internalPoint);
                originalObj.Normals.Add(new Normal(vertex2.Coordinates.X * -1, vertex2.Coordinates.Y * -1, vertex2.Coordinates.Z, internalPoint.Number));
                return internalPoint;
            }
            else
            {
                var internalPoint = new Vertex(vertex1.X + vertex2.Coordinates.X / den,
                                                      vertex1.Y + vertex2.Coordinates.Y / den,
                                                      vertex2.Coordinates.Z,
                                                      colorsCount,
                                                      verticesCount);
                originalObj.Vertices.Add(internalPoint);
                originalObj.Normals.Add(new Normal(vertex2.Coordinates.X * -1, vertex2.Coordinates.Y * -1, vertex2.Coordinates.Z, internalPoint.Number));
                return internalPoint;
            }
        }


        private static List<Vertex> GetOneLayer(List<Vertex> layers, ref int index)
        {
            var oneLayer = new List<Vertex>() { layers[index - 1] };
            while (index < layers.Count && layers[index].Coordinates.Z == layers[index - 1].Coordinates.Z)
            {
                oneLayer.Add(layers[index]);
                index++;
            }
            index++;
            if (oneLayer.Count == 1) index++;
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
            var minXLine = new StraightLine(minAndMaxVertex.MinX, new Vertex(minAndMaxVertex.MinX.Coordinates.X, minAndMaxVertex.MinX.Coordinates.Y - 10, minAndMaxVertex.MinX.Coordinates.Z));
            var maxXLine = new StraightLine(minAndMaxVertex.MaxX, new Vertex(minAndMaxVertex.MaxX.Coordinates.X, minAndMaxVertex.MaxX.Coordinates.Y - 10, minAndMaxVertex.MaxX.Coordinates.Z));
            var minYLine = new StraightLine(minAndMaxVertex.MinY, new Vertex(minAndMaxVertex.MinY.Coordinates.X - 10, minAndMaxVertex.MinY.Coordinates.Y, minAndMaxVertex.MinX.Coordinates.Z));
            var maxYLine = new StraightLine(minAndMaxVertex.MaxY, new Vertex(minAndMaxVertex.MaxX.Coordinates.X - 10, minAndMaxVertex.MaxY.Coordinates.Y, minAndMaxVertex.MaxX.Coordinates.Z));

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
            for (var i = 1; i < oneLayer.Count - 1; i++)
            {
                var angleBetweenLines = GetAngle(oneLayer[i - 1], oneLayer[i], oneLayer[i + 1]);
                if (angleBetweenLines < 90) problemPoints.Add(oneLayer[i]);
            }
            if (GetAngle(oneLayer[oneLayer.Count - 1], oneLayer[0], oneLayer[1]) < 90) problemPoints.Add(oneLayer[0]);
            if (GetAngle(oneLayer[oneLayer.Count - 2], oneLayer[oneLayer.Count - 1], oneLayer[0]) < 90) problemPoints.Add(oneLayer[oneLayer.Count - 1]);
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
            ObjSortedByColor.Add(new Obj());
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
                    var i = (int)id;
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

            return index < 0 ? 0 : index;
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
