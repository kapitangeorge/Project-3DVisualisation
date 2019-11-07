using System;
using System.Collections.Generic;
using System.Text;

namespace Project3D.L.Model
{
    public class StraightLine
    {
        public Vertex Point1 { get; }
        public Vertex Point2 { get; }

        public StraightLine(Vertex point1, Vertex point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
    }
}
