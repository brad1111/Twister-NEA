using System;
using System.IO;
using System.Windows.Controls;
using Nea_Prototype.Grid;
using Newtonsoft.Json;

namespace Nea_Prototype.Level
{
    public class Level
    {
        [JsonIgnore]
        private GameGrid grid;
        [JsonProperty("StartLocations")]
        public int[,] gridStartLocations { get; set; }
        public ExitPlacement ExitLocation { get; set; }

        public Level()
        {
            
        }

        public Level(int[,] gridStartLocations)
        {
            
        }

        //public static void SetupGridFromFile(string filename)
        //{
        //    StreamReader sr = new StreamReader(filename);
        //    string tempString = String.Empty;
        //    int[,] internalArray = new int[10, 10];
        //    while (sr.EndOfStream == false)
        //    {
        //        tempString += sr.ReadLine();
        //    }

        //    int currentIndex = 0;
        //    int currentLine = 0;
        //    foreach (var character in tempString)
        //    {
        //        try
        //        {
        //            internalArray[currentIndex, currentLine] = character;
        //            if (currentIndex < 9)
        //            {
        //                currentIndex++;
        //            }
        //            else if (currentIndex == 9)
        //            {
        //                currentLine++;
        //                currentIndex = 0;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //            throw;
        //        }

        //    }
        //}

        public void SetupGrid(ref Canvas gameCanvas)
        {

        }
    }
}