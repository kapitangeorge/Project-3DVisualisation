using System;
using System.Collections.Generic;
using Project3D.L.Controller;
using Project3D.L.Model;

namespace Project3D.CMD
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"G:\University\Множества достижимости\Color obj\Model3D-6.0pi.obj";
            var objFile = FileProcessing.ReadFile(path);
            var objList = FigureProcessing.ColorSeperation(objFile);

            FileProcessing.WriteFile(@"G:\University\Множества достижимости\Color obj\", objList);
            Console.WriteLine("Файл прочитан.");
        }
    }
}
