using System;

namespace Project3D.L.Model
{
    public class Normal
    {
        public Coordinates Coordinates { get; set; }
        public int Number { get; set; }
        

        public Normal(double x, double y, double z, int number)
        {
            Coordinates = new Coordinates() { X = x, Y = y, Z = z };
            Number = number;
        }

        public override string ToString()
        {
            return $"vn {Coordinates.X} {Coordinates.Y} {Coordinates.Z}";
        }
    }
}
