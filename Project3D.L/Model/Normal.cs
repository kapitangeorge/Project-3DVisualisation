using System;

namespace Project3D.L.Model
{
    public class Normal
    {
        public Coordinates Coordinates { get; set; }
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
    }
}
