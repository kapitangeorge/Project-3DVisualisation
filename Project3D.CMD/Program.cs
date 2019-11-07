using System;
using System.Collections.Generic;
using System.Linq;
using Project3D.L.Controller;
using Project3D.L.Model;

namespace Project3D.CMD
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"G:\University\Множества достижимости\Obj models last\Model3D+0.obj";
            var objFile = FileProcessing.ReadFile(path);
            FigureProcessing.InternalFiling(objFile);

            var objList = FigureProcessing.ColorSeperation(objFile);


            FileProcessing.WriteFile(@"G:\University\Множества достижимости\Color obj\", objList);
            Console.WriteLine("Файл прочитан.");
        }
    }
}
