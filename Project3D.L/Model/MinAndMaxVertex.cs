using System;
using System.Collections.Generic;
using System.Text;

namespace Project3D.L.Model
{
    public class MinAndMaxVertex
    {
        public Vertex MinX { get; }
        public Vertex MaxX { get; }
        public Vertex MinY { get; }
        public Vertex MaxY { get; }

        public MinAndMaxVertex(Vertex minX, Vertex maxX , Vertex minY, Vertex maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }
    }
}
