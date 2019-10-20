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
    }
}
