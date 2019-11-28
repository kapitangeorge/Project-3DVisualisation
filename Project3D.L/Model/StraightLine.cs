using System;
using System.Collections.Generic;
using System.Text;

namespace Project3D.L.Model
{
    public class StraightLine
    {
        public Vertex Point1 { get; set; }
        public Vertex Point2 { get; set; }
        public int Number1 { get; set; }
        public int Number2 { get; set; }

        public StraightLine(Vertex point1, Vertex point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public StraightLine(Vertex point1, Vertex point2, int number1, int number2)
        {
            Point1 = point1;
            Point2 = point2;
            Number1 = number1;
            Number2 = number2;

        }

        public StraightLine()
        {
        }
    }
}
