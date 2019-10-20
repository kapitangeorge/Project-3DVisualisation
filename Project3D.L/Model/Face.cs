using System;
using System.Collections.Generic;
using System.Text;

namespace Project3D.L.Model
{
    public class Face
    {
        public long[] Triangle { get; set; } = new long[3];

        public override string ToString()
        {
            return $"f {Triangle[0]}//{Triangle[0]} {Triangle[1]}//{Triangle[1]} {Triangle[2]}//{Triangle[2]}";
        }
    }
}
