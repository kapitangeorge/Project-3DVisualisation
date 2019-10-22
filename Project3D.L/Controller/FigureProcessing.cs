using Project3D.L.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project3D.L.Controller
{
    public class FigureProcessing
    {

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
    }
}
