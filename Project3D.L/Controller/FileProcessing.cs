using Project3D.L.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Project3D.L.Controller
{
    public class FileProcessing
    {
        public static Obj ReadFile(string filePath)
        {
            string textFromFile;
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                byte[] array = new byte[fileStream.Length];
                fileStream.Read(array, 0, array.Length);
                textFromFile = Encoding.Default.GetString(array);


            }
            return ConvertText(textFromFile);
        }

        public static void WriteFile(string filePath, List<Obj> objSortByColors)
        {
            filePath = CreateNewDirectory(filePath);
            var index = 1;

            foreach (var obj in objSortByColors)
            {
                var newFilePath = filePath + $"\\NewOneColorModel{index}.Obj";
                using (var fileStream = new StreamWriter(newFilePath, false))
                {
                    WriteCreateBy(fileStream);
                    for (var i = 0; i < obj.Vertices.Count; i++)
                    {
                        fileStream.WriteLine(obj.Normals[i].ToString());
                        fileStream.WriteLine(obj.Vertices[i].ToString());

                    }
                    foreach (var faces in obj.Faces)
                    {
                        fileStream.WriteLine(faces.ToString());
                    }
                };
                index++;
            }
        }

        private static void WriteCreateBy(StreamWriter fileStream)
        {
            fileStream.WriteLine("#################");
            fileStream.WriteLine();
            fileStream.WriteLine("File created by company Pink Owl\'s");
            fileStream.WriteLine();
            fileStream.WriteLine("#################");
            fileStream.WriteLine();
        }

        private static string CreateNewDirectory(string filePath)
        {
            var newFilePath = filePath + @"\NewModels";
            DirectoryInfo dirInfo = new DirectoryInfo(newFilePath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
           
            return newFilePath;
        }

        //    public static string MakeNewFilePath(string oldFilePath)
        //{
        //    int oldFilePathLength = oldFilePath.Length;
        //    string fileExtension = "";
        //    string oldFileName = "";
        //    string fileDirectory = "";
        //    {
        //        int i = oldFilePathLength - 1;
        //        while (i >= 0 && oldFilePath[i] != '.')
        //        {
        //            fileExtension += oldFilePath[i];
        //            i--;
        //        }
        //        i--;

        //        while (i >= 0 && oldFilePath[i] != '\\')
        //        {
        //            oldFileName += oldFilePath[i];
        //            i--;
        //        }
        //        i--;

        //        while (i >= 0)
        //        {
        //            fileDirectory += oldFilePath[i];
        //            i--;
        //        }
        //    }

        //    string newFilePath = fileExtension + '.' + oldFileName + "WEN\\" + fileDirectory;
        //    return new string(newFilePath.ToArray().Reverse().ToArray());
        //}

        //public static string MakeNewFilePath(string oldFilePath, string newFileName)
        //{
        //    int oldFilePathLength = oldFilePath.Length;
        //    string fileExtension = "";
        //    string fileDirectory = "";
        //    {
        //        int i = oldFilePathLength - 1;
        //        while (i >= 0 && oldFilePath[i] != '.')
        //        {
        //            fileExtension += oldFilePath[i];
        //            i--;
        //        }
        //        i--;

        //        while (i >= 0 && oldFilePath[i] != '\\')
        //            i--;
        //        i--;

        //        while (i >= 0)
        //        {
        //            fileDirectory += oldFilePath[i];
        //            i--;
        //        }
        //    }

        //    newFileName = new string(newFileName.ToArray().Reverse().ToArray());
        //    string newFilePath = fileExtension + '.' + newFileName + fileDirectory;
        //    return new string(newFilePath.ToArray().Reverse().ToArray());
        //}

        private static Obj ConvertText(string textFromFile)
        {
            Obj originalObj = new Obj();
            string[] textStrings = textFromFile.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            foreach (string str in textStrings)
            {
                string[] strParts = str.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                if (strParts.Length == 7 && strParts[0] == "v")
                {
                    VertexProcessing(strParts, originalObj);
                }

                if (strParts.Length == 4 && strParts[0] == "vn")
                {
                    NormalsProcessing(strParts, originalObj);
                }

                if (strParts.Length == 4 && strParts[0] == "f")
                {
                    FacesProcessing(strParts, originalObj);
                }
            }


            return originalObj;

        }

        private static void FacesProcessing(string[] faces, Obj originalObj)
        {
            var triangleProp = new int[3];
            for (var i = 1; i <= 3; i++)
            {
                var stringFace = faces[i].Split('/');
                if (Int32.TryParse(stringFace[0], out int result))
                {
                    triangleProp[i - 1] = result;
                }
            }
            originalObj.Faces.Add(new Face(triangleProp));
        }

        private static void NormalsProcessing(string[] normals, Obj originalObj)
        {
            var normalsProp = new double[3];
            for (var i = 1; i <= 3; i++)
            {
                normalsProp[i - 1] = Convert.ToDouble(normals[i], new CultureInfo("en-US"));
            }
            originalObj.Normals.Add(new Normal(normalsProp[0], normalsProp[1], normalsProp[2], originalObj.Normals.Count + 1));
        }

        private static void VertexProcessing(string[] vertex, Obj originalObj)
        {
            var vertexInfo = new double[6];
            for (var i = 1; i <= 6; i++)
            {
                vertexInfo[i - 1] = Convert.ToDouble(vertex[i], new CultureInfo("en-US"));
            }

            int colorsCount = originalObj.Colors.Count;
            int colorNumber = 0;
            int number = originalObj.Vertices.Count + 1;
            for (int i = 0; i < colorsCount; i++)
                if (originalObj.Colors[i].R == vertexInfo[3]
                && originalObj.Colors[i].G == vertexInfo[4]
                && originalObj.Colors[i].B == vertexInfo[5])
                    colorNumber = i + 1;
            if (colorNumber == 0)
            {
                originalObj.Colors.Add(new Color(vertexInfo[3], vertexInfo[4], vertexInfo[5]));
                colorNumber = colorsCount + 1;
            }
            originalObj.Vertices.Add(new Vertex()
            {
                Coordinates = new Coordinates() { X = vertexInfo[0], Y = vertexInfo[1], Z = vertexInfo[2] },
                Number = number,
                ColorNumber = colorNumber
            });
        }
    }
}
