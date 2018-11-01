using Common.Level;
using Newtonsoft.Json;

namespace Server.Level
{
    public class Level
    {
        [JsonProperty("StartLocations")] public int[,] gridStartLocations { get; internal set; }
        public ExitPlacement ExitLocation { get; set; }

    }
}