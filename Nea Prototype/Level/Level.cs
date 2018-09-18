using System;
using System.IO;
using System.Windows.Controls;
using Nea_Prototype.Characters;
using Nea_Prototype.Enums;
using Nea_Prototype.Grid;
using Newtonsoft.Json;

namespace Nea_Prototype.Level
{
    public class Level
    {
        [JsonIgnore]
        private GameGrid grid;

        [JsonIgnore] 
        private EnemyType enemyType;
        [JsonProperty("StartLocations")]
        public int[,] gridStartLocations { get; set; }
        public ExitPlacement ExitLocation { get; set; }

        /// <summary>
        /// Only used for LevelIO.CreateJSON()
        /// </summary>
        public Level()
        {
            
        }

        /// <summary>
        /// Normal usage of create level with the gridStartLocations
        /// </summary>
        /// <param name="gridStartLocations">The locations where all the items on the grid start</param>
        public Level(int[,] gridStartLocations, EnemyType enemyType)
        {
            this.gridStartLocations = gridStartLocations;
            this.enemyType = enemyType;
        }

        public void SetupGrid(ref Canvas gameCanvas)
        {

        }

        private void DecodeGridStartLocations()
        {
            int yLength = gridStartLocations.GetLength(0);
            int xLength = gridStartLocations.GetLength(1);

            GridItem[,] gridItems = new GridItem[yLength, xLength];
            GridItemView[,] gridItemsViews = new GridItemView[yLength, xLength];
            Character[] characters = new Character[2];
            for (int y = 0; y < yLength; y++)
            {
                for (int x = 0; x < xLength; x++)
                {
                    switch (gridStartLocations[y,x])
                    {
                          //walkable object
                          case 0:
                              Walkable walkable = new Walkable()
                              {
                                  Position = new Position(x,y)
                              };
                              GridItemView walkableView = new GridItemView(walkable);
                              gridItems[y, x] = walkable;
                              gridItemsViews[y, x] = walkableView;
                              break;
                          //non-walkable object
                          case 1:
                              NonWalkable nonWalkable = new NonWalkable()
                              {
                                  Position = new Position(x,y)
                              };
                              GridItemView nonWalkableView = new GridItemView(nonWalkable);
                              gridItems[y, x] = nonWalkable;
                              gridItemsViews[y, x] = nonWalkableView;
                              break;
                          //player 1
                          case 2:
                              PlayerOne playerOne = new PlayerOne();
                              CharacterItem playerOneItem = new CharacterItem(playerOne)
                              {
                                  Position = new Position(x, y)
                              };
                              GridItemView playerOneView = new GridItemView(playerOneItem);
                              gridItems[y, x] = playerOneItem;
                              gridItemsViews[y, x] = playerOneView;
                              characters[0] = playerOne;
                              break;
                          //Enemy
                          case 3:
                                 //Choose which type of Enemy:
                                Enemy enemy = null;
                                switch (enemyType)
                                {
                                    case EnemyType.Local:
                                        enemy = new PlayerTwo();
                                        break;
                                    case EnemyType.AI:
                                        enemy = new BotPlayer();
                                        break;
                                    case EnemyType.Remote:
                                        throw new NotImplementedException("Remote players are not implemented");
                                }
                               CharacterItem enemyItem = new CharacterItem(enemy)
                              {
                                  Position = new Position(x, y)
                              };
                              GridItemView enemyView = new GridItemView(enemyItem);
                              gridItems[y, x] = enemyItem;
                              gridItemsViews[y, x] = enemyView;
                              characters[1] = enemy;
                              break;
                          default:
                              throw new NotImplementedException($"The value of {gridStartLocations[y,x]} is not implemented in Level.Level.GridStartLocation()");
                    }
                }
            }
            grid = new GameGrid(characters, gridItemsViews, gridItems);
        }

        private void MoveItemToPlace(ref GridItemView itemView, Position location)
        {
            Canvas.SetLeft(itemView, location.x * Constants.GRID_ITEM_WIDTH);
        }
    }
}