using System;


namespace Project3D.L.Model
{
    public class Vertex
    {
        public Coordinates Coordinates { get; set; }
        public int ColorNumber { get; set; }
        public int Number
        {
            get
            {
                return Number;
            }
            set
            {
                if (value < 1)
                    Console.WriteLine("File read error");
                else
                    Number = value;
            }
        }

        //методы расстояния
    }
}
