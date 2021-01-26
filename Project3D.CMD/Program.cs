using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Project3D.L.Controller;
using Project3D.L.Model;

namespace Project3D.CMD
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var path = @"C:\obj\Model3D+0.0-08pi_4.obj";
            var objFile = FileProcessing.ReadFile(path);
            FigureProcessing.ConnectionOfLayers(objFile);

            var objList = FigureProcessing.ColorSeperation(objFile);


            FileProcessing.WriteFile(@"C:\obj\Color obj\", objList);
            Console.WriteLine("Файл прочитан.");

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }
}
