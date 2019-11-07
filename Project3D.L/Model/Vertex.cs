using System;


namespace Project3D.L.Model
{
    public class Vertex
    {
        public Coordinates Coordinates { get; set; }
        public int ColorNumber { get; set; }
        public int Number { get; set; }

        public override string ToString()
        {
            return $"v {Coordinates.X} {Coordinates.Y}  {Coordinates.Z}";
        }
        //методы расстояния

        public Vertex(double x, double y, double z)
        {
            Coordinates = new Coordinates { X = x, Y = y, Z = z };
        }

        public Vertex(double x, double y, double z, int colorNumber, int number)
        {
            Coordinates = new Coordinates { X = x, Y = y, Z = z };
            ColorNumber = colorNumber;
            Number = number;
        }

        public static Vertex IntersectionPointOfTwoLines2D(StraightLine line1, StraightLine line2)
        {
            return IntersectionPointOfTwoLines2D(line1.Point1, line1.Point2, line2.Point1, line2.Point2);
        }

        public static Vertex IntersectionPointOfTwoLines2D(Vertex Point1, Vertex Point2, Vertex Point3, Vertex Point4)
        {
            var x = ((Point1.Coordinates.X * Point2.Coordinates.Y - Point1.Coordinates.Y * Point2.Coordinates.X) *
                    (Point3.Coordinates.X - Point4.Coordinates.X) - (Point1.Coordinates.X - Point2.Coordinates.X) *
                    (Point3.Coordinates.X * Point4.Coordinates.Y - Point3.Coordinates.Y * Point4.Coordinates.X)) /
                    ((Point1.Coordinates.X - Point2.Coordinates.X) * (Point3.Coordinates.Y - Point4.Coordinates.Y) -
                    (Point1.Coordinates.Y - Point2.Coordinates.Y) * (Point3.Coordinates.X - Point4.Coordinates.X));
            var y = ((Point1.Coordinates.X * Point2.Coordinates.Y - Point1.Coordinates.Y * Point2.Coordinates.X) *
                    (Point3.Coordinates.Y - Point4.Coordinates.Y) - (Point1.Coordinates.Y - Point2.Coordinates.Y) *
                    (Point3.Coordinates.X * Point4.Coordinates.Y - Point3.Coordinates.Y * Point4.Coordinates.X)) /
                    ((Point1.Coordinates.X - Point2.Coordinates.X) * (Point3.Coordinates.Y - Point4.Coordinates.Y) -
                    (Point1.Coordinates.Y - Point2.Coordinates.Y) * (Point3.Coordinates.X - Point4.Coordinates.X));
            var z = Point1.Coordinates.Z;
            var intersectionPoint = new Vertex(x, y, z);
            return intersectionPoint;
        }
    }
}
