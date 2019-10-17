using System;
using System.Collections.Generic;
using System.Text;

namespace Project3D.L.Model
{
    public class Obj
    {
         public List<Vertex> Vertices { get; set; } = new List<Vertex>();
        public List<Normal> Normals { get; set; } = new List<Normal>();
        public List<Face> Faces { get; set; } = new List<Face>();
        public List<Color> Colors { get; set; } = new List<Color>();

        public Obj()
        {
            Vertices = new List<Vertex>();

            Normals = new List<Normal>();

            Faces = new List<Face>();

            Colors = new List<Color>();
        }
    }
}
