using System;
using System.IO;
using Nea_Prototype.Enums;
using Newtonsoft.Json;

namespace Nea_Prototype.Level
{
    public class LevelIO
    {
        public static void CreateJSON()
        {
            StreamWriter sw = new StreamWriter("testing.json");
            string json = "";

            int[,] tempInt =
            {
                {
                    0, 1, 2, 3
                },
                {
                    4, 5, 6, 7
                }
            };
            Level level = new Level()
            {
                gridStartLocations = tempInt,
                ExitLocation = new ExitPlacement(100,Side.Right, 80)
            };
            
            json = JsonConvert.SerializeObject(level);
            sw.Write(json);
            sw.Close();
        }

        public static Level ReadJSON(string fileToRead)
        {
            StreamReader sr = new StreamReader(fileToRead);
            string json = sr.ReadToEnd();
            Level level = JsonConvert.DeserializeObject<Level>(json);   
            return level;
        }
    }
}