using Project3D.L.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Project3D.L.Controller
{
    class FileProcessing
    {
        public static string ReadFile(string filePath)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                byte[] array = new byte[fileStream.Length];
                fileStream.Read(array, 0, array.Length);
                string textFromFile = Encoding.Default.GetString(array);

                return textFromFile;
            }
        }

        public static void WriteFile(string newFilePath, string textFromFile)
        {
            using (FileStream fileStream = File.Create(newFilePath))
            {
                byte[] array = Encoding.Default.GetBytes(textFromFile);
                fileStream.Write(array, 0, array.Length);
                Console.WriteLine("Текст записан в файл");
            }
        }

        public static string MakeNewFilePath(string oldFilePath)
        {
            int oldFilePathLength = oldFilePath.Length;
            string fileExtension = "";
            string oldFileName = "";
            string fileDirectory = "";
            {
                int i = oldFilePathLength - 1;
                while (i >= 0 && oldFilePath[i] != '.')
                {
                    fileExtension += oldFilePath[i];
                    i--;
                }
                i--;

                while (i >= 0 && oldFilePath[i] != '\\')
                {
                    oldFileName += oldFilePath[i];
                    i--;
                }
                i--;

                while (i >= 0)
                {
                    fileDirectory += oldFilePath[i];
                    i--;
                }
            }

            string newFilePath = fileExtension + '.' + oldFileName + "WEN\\" + fileDirectory;
            return new string(newFilePath.ToArray().Reverse().ToArray());
        }

        public static string MakeNewFilePath(string oldFilePath, string newFileName)
        {
            int oldFilePathLength = oldFilePath.Length;
            string fileExtension = "";
            string fileDirectory = "";
            {
                int i = oldFilePathLength - 1;
                while (i >= 0 && oldFilePath[i] != '.')
                {
                    fileExtension += oldFilePath[i];
                    i--;
                }
                i--;

                while (i >= 0 && oldFilePath[i] != '\\')
                    i--;
                i--;

                while (i >= 0)
                {
                    fileDirectory += oldFilePath[i];
                    i--;
                }
            }

            newFileName = new string(newFileName.ToArray().Reverse().ToArray());
            string newFilePath = fileExtension + '.' + newFileName + fileDirectory;
            return new string(newFilePath.ToArray().Reverse().ToArray());
        }

        private static Obj ConvertText(string textFromFile)
        {
            Obj originalObj = new Obj();
            string[] textStrings = textFromFile.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            foreach (string str in textStrings)
            {
                string[] strParts = str.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                if (strParts.Length == 7 && strParts[0] == "v" && double.TryParse(strParts[1], out double x)
                    && double.TryParse(strParts[2], out double y) && double.TryParse(strParts[3], out double z)
                    && double.TryParse(strParts[4], out double r) && double.TryParse(strParts[4], out double g)
                    && double.TryParse(strParts[6], out double b))
                {
                    int colorsCount = originalObj.Colors.Count;
                    int colorNumber = 0;
                    int number = originalObj.Vertices.Count + 1;
                    for (int i = 0; i < colorsCount; i++)
                        if (originalObj.Colors[i].R == r
                        && originalObj.Colors[i].G == g
                        && originalObj.Colors[i].B == b)
                            colorNumber = i + 1;
                    if (colorNumber == 0)
                    {
                        originalObj.Colors.Add(new Color(r,g,b));
                        colorNumber = colorsCount + 1;
                    }
                    originalObj.Vertices.Add(new Vertex()
                    {
                        Coordinates = new Coordinates() { X = x, Y = y, Z = z },
                        Number = number,
                        ColorNumber = colorNumber
                    });
                }
            }
            return originalObj;
        }

    }
}
