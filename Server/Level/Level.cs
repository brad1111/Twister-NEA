using System.Collections.Generic;
using System.Threading;
using Common;
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
        
        public double[] characterWeights { get; set; } = new double[2];

        /// <summary>
        /// Sets up the exits etc so that the level can do stuff
        /// </summary>
        public void SetupLevel(double protagonistWeight, double enemyWeight)
        {
            List<InternalExit> exits = new List<InternalExit>();
            for (int y = 0; y < gridStartLocations.GetLength(0); y++)
            {
                for (int x = 0; x < gridStartLocations.GetLength(1); x++)
                {
                    switch (gridStartLocations[y, x])
                    {
                        //Protagonist
                        case 2:
                            ServerDataManager.Instance.character1 = new Character(1,x * Constants.GRID_ITEM_WIDTH,
                                                                                    y * Constants.GRID_ITEM_WIDTH);
                            break;
                        //Enemy
                        case 3:
                            ServerDataManager.Instance.character2 = new Character(2,x * Constants.GRID_ITEM_WIDTH,
                                y * Constants.GRID_ITEM_WIDTH);
                            break;
                        //If grid is an exitable block add it to InternalExits with the relevant information
                        case 5:
                            InternalExit exit = new InternalExit(new Position(x, y));
                            exits.Add(exit);
                            break;
                        
                    }
                }
            }

            InternalExits = exits.ToArray();

            //Setup enemy weights in level file
            characterWeights[0] = protagonistWeight;
            characterWeights[1] = enemyWeight;
        }

    }
}