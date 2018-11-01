using System.IO;
using Newtonsoft.Json;

namespace Server.Level
{
    public class LevelIO
    {
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