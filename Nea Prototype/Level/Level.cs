using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Xml.Schema;
using Nea_Prototype.Characters;
using Nea_Prototype.Enums;
using Nea_Prototype.Grid;
using Newtonsoft.Json;

namespace Nea_Prototype.Level
{
    public class Level
    {
        [JsonIgnore] private GameGrid grid;

        [JsonIgnore] private EnemyType enemyType;
        [JsonProperty("StartLocations")] public int[,] gridStartLocations { get; internal set; }
        public ExitPlacement ExitLocation { get; internal set; }

        private int yLength()
        {
            return gridStartLocations.GetLength(0);
        }

        private int xLength()
        {
            return gridStartLocations.GetLength(1); 
        }

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
        public Level(EnemyType enemyType)
        {
            this.gridStartLocations = gridStartLocations;
            this.enemyType = enemyType;
        }

        public void SetupGrid(ref Canvas gameCanvas)
        {
            DecodeGridStartLocations();
            for (int y = 0; y < gridStartLocations.GetLength(0); y++)
            {
                for (int x = 0; x < gridStartLocations.GetLength(1); x++)
                {
                    gameCanvas.Children.Add(grid.GridItemsViews[y, x]);
                    MoveItemToPlace(ref grid.GridItemsViews[y,x], grid.GridItems[y,x].Position);
                }
            }
        }

        private void DecodeGridStartLocations()
        {
            GridItem[,] gridItems = new GridItem[yLength(), xLength()];
            GridItemView[,] gridItemsViews = new GridItemView[yLength(), xLength()];
            Character[] characters = new Character[2];
            GridItemView[] charactersView = new GridItemView[2];
            for (int y = 0; y < yLength(); y++)
            {
                for (int x = 0; x < xLength(); x++)
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
                              charactersView[0] = gridItemsViews[y, x] = playerOneView;
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
                              charactersView[1] = gridItemsViews[y, x] = enemyView;
                              characters[1] = enemy;
                              break;
                          default:
                              throw new NotImplementedException($"The value of {gridStartLocations[y,x]} is not implemented in Level.Level.GridStartLocation()");
                    }
                }
            }
            grid = new GameGrid(characters, charactersView, gridItemsViews, gridItems);
        }

        public void MoveItemToPlace(ref GridItemView itemView, Position location)
        {
            Canvas.SetLeft(itemView, location.x * Constants.GRID_ITEM_WIDTH);
            Canvas.SetTop(itemView, location.y * Constants.GRID_ITEM_WIDTH);
        }

        public void MoveCharacter(int characterNo, Direction dir)
        {
            MoveCharacterInternal(GetCharacterView(characterNo), dir);
        }

        private void MoveCharacterInternal(GridItemView itemView, Direction dir)
        {
            double leftPos = 0;
            double topPos = 0;

            switch (dir)
            {
                case Direction.Up:
                    topPos = Canvas.GetTop(itemView);
                    Canvas.SetTop(itemView, topPos - Constants.KEYPRESS_PX_MOVED);
                    break;
                case Direction.Down:
                    topPos = Canvas.GetTop(itemView);
                    Canvas.SetTop(itemView, topPos + Constants.KEYPRESS_PX_MOVED);
                    break;
                case Direction.Left:
                    leftPos = Canvas.GetLeft(itemView);
                    Canvas.SetLeft(itemView, leftPos - Constants.KEYPRESS_PX_MOVED);
                    break;
                case Direction.Right:
                    leftPos = Canvas.GetLeft(itemView);
                    Canvas.SetLeft(itemView, leftPos + Constants.KEYPRESS_PX_MOVED);
                    break;
                default:
                    throw new NotImplementedException(
                        $"Direction '{nameof(dir)}' is not implemented in Level.Level.MoveCharacter()");
            }
        }

        public GridItemView GetCharacterView(int characterNo)
        {
            switch (characterNo)
            {
                case 1:
                    return grid.CharactersViews[0];
                case 2:
                    return grid.CharactersViews[1];
                default:
                    throw new NotImplementedException(
                        $"Player {characterNo} is not implemented in Level.Level.GetCharactersView()");
            }
        }
    }
}