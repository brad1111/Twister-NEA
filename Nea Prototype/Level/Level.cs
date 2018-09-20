using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
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
            //MoveCharacterInternal(GetCharacterView(characterNo), dir);
            GridItemView characterView = GetCharacterView(characterNo);
            //If character won't collide with the wall
            if (!WallCollisionDetection(ref characterView, dir))
            {
                MoveCharacterInternal(ref characterView, dir);
            }
        }

        private void MoveCharacterInternal(ref GridItemView itemView, Direction dir)
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
        
        private bool WallCollisionDetection(ref GridItemView characterView, Direction movementDirection)
        {
            double x, y = 0;
            x = Canvas.GetLeft(characterView);
            y = Canvas.GetTop(characterView);
            int xApprox, yApprox = 0;
            const double half_GRID_ITEM_WIDTH = Constants.GRID_ITEM_WIDTH / 2;
            Queue<GridItemView> ItemsToCheckForCollision = new Queue<GridItemView>(3);
            switch (movementDirection)
            {
                case Direction.Up:
                    

                    //Get approx co-ords
                    xApprox = (int) Math.Floor((x + half_GRID_ITEM_WIDTH) / Constants.GRID_ITEM_WIDTH);
                    yApprox = (int) Math.Floor((y + half_GRID_ITEM_WIDTH) / Constants.GRID_ITEM_WIDTH);
                    
                    //get three possible collisionable items above
                    if (yApprox == 0)
                    {
                        //then can't look any higher so look for distance from top wall
                        // (if y is smaller than 4 then it will collide, otherwise it wont).
                        return (y <= 4);
                    }
                    else
                    {
                        //Get above to the left if valid
                        GridItemView AboveLeft = (xApprox > 0) ? (grid.GridItemsViews[yApprox - 1,xApprox - 1]) : null;
                        //Get above
                        GridItemView Above = grid.GridItemsViews[yApprox - 1, xApprox];
                        //Get above to the right if valid
                        GridItemView AboveRight = (xApprox < Constants.GRID_TILES_XY) ? (grid.GridItemsViews[yApprox - 1, xApprox + 1]) : null;

                        for (int i = -1; i <= 1; i++)
                        {
                            //Check for above left, above and above right, and if they are non-walkable
                            if (grid.GridItems[yApprox - 1, xApprox + i].GetType() == typeof(NonWalkable))
                            {
                                //Then add to the queue
                                ItemsToCheckForCollision.Enqueue(grid.GridItemsViews[yApprox - 1, xApprox + i]);
                            }
                        }

                        ////Create rectangles for collision detection
                        //Rect AboveLeftRect = new Rect(Canvas.GetLeft(AboveLeft), Canvas.GetTop(AboveLeft) - 4, Constants.GRID_ITEM_WIDTH, Constants.GRID_ITEM_WIDTH);
                        //Rect AboveRect = new Rect(Canvas.GetLeft(Above), Canvas.GetTop(Above) - 4, Constants.GRID_ITEM_WIDTH, Constants.GRID_ITEM_WIDTH);
                        //Rect AboveRightRect = new Rect(Canvas.GetLeft(AboveRight) - 4, Canvas.GetTop(AboveRight), Constants.GRID_ITEM_WIDTH, Constants.GRID_ITEM_WIDTH);
                        //Rect characterRect = new Rect(x,y,Constants.GRID_ITEM_WIDTH, Constants.GRID_ITEM_WIDTH);
                        ////If character intersects with a rectangle then return
                        //bool intersects = (characterRect.IntersectsWith(AboveLeftRect) ||
                        //        characterRect.IntersectsWith(AboveRect) ||
                        //        characterRect.IntersectsWith(AboveRightRect));
                        
                        //Override y to be the moved value so that they can be checked for intersection
                        y -= 4;
                    }
                    break;
                case Direction.Down:
                    //break;
                case Direction.Left:
                    //break;
                case Direction.Right:
                    //break;
                default:
                    throw new NotImplementedException($"Direction value of {nameof(movementDirection)} is not implemented in Level.WallCollisionDetection()");
            }

            //Rectangle variable is to check to see if it intersects
            Rect characterRect = new Rect(x + 1, y + 1, Constants.GRID_ITEM_WIDTH - 2 , Constants.GRID_ITEM_WIDTH - 2);
            bool collision = false;

            while (ItemsToCheckForCollision.Count > 0)
            {
                GridItemView tempItem = ItemsToCheckForCollision.Dequeue();
                Rect nonWalkableRect = new Rect(Canvas.GetLeft(tempItem), Canvas.GetTop(tempItem), Constants.GRID_ITEM_WIDTH, Constants.GRID_ITEM_WIDTH);
                if (nonWalkableRect.IntersectsWith(characterRect) || collision /*== true*/)
                {
                    collision = true;
                }
            }

            return collision;
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