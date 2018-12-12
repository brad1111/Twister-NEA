using System.Collections.Generic;
using System.Threading;
using Common.Grid;
using Common.Level;
using Newtonsoft.Json;

namespace Server.Level
{
    public class Level
    {
        [JsonProperty("StartLocations")] public int[,] gridStartLocations { get; internal set; }
        public ExitPlacement ExitLocation { get; set; }

        public InternalExit[] InternalExits { get; private set; }

        /// <summary>
        /// Sets up the exits etc so that the level can do stuff
        /// </summary>
        public void SetupLevel()
        {
            List<InternalExit> exits = new List<InternalExit>();
            for (int y = 0; y < gridStartLocations.GetLength(0); y++)
            {
                for (int x = 0; x < gridStartLocations.GetLength(1); x++)
                {
                    //If grid is an exitable block add it to InternalExits with the relevant information
                    if (gridStartLocations[y, x] == 5)
                    {
                        InternalExit exit = new InternalExit(new Position(x, y));
                        exits.Add(exit);
                    }
                }
            }

            InternalExits = exits.ToArray();
        }

    }
}