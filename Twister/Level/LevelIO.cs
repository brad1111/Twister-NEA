using System.IO;
using Common.Enums;
using Common.Level;
using Newtonsoft.Json;

namespace Twister.Level
{
    public class LevelIO
    {
        /// <summary>
        /// Makes a sample json file for testing purpose from a sample level object
        /// </summary>
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

        /// <summary>
        /// Reads a json file and converts it to a level object
        /// </summary>
        /// <param name="fileToRead">The json file to be read</param>
        /// <returns>A level object</returns>
        public static Level ReadJSON(string fileToRead)
        {
            StreamReader sr = new StreamReader(fileToRead);
            string json = sr.ReadToEnd();
            Level level = JsonConvert.DeserializeObject<Level>(json);   
            return level;
        }
    }
}