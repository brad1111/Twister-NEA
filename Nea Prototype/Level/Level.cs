using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        [JsonIgnore] private GameGridManager _gridManager;

        //These need to be public properties so that JSON.net can fill these values from the json file.
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

        ///// <summary>
        ///// Normal usage of create level with the gridStartLocations
        ///// </summary>
        ///// <param name="gridStartLocations">The locations where all the items on the grid start</param>
        //public Level(EnemyType enemyType)
        //{
        //    this.gridStartLocations = gridStartLocations;
        //    _gridManager.EnemyType = enemyType;
        //}

        public void SetupGrid(ref Canvas gameCanvas, EnemyType enemyType)
        {
            DecodeGridStartLocations(enemyType);
            //Add grid items
            for (int y = 0; y < gridStartLocations.GetLength(0); y++)
            {
                for (int x = 0; x < gridStartLocations.GetLength(1); x++)
                {
                    gameCanvas.Children.Add(_gridManager.GridItemsViews[y, x]);
                    MoveItemToPlace(ref _gridManager.GridItemsViews[y,x], _gridManager.GridItems[y,x].Position);
                }
            }

            //Add character
            for (int i = 0; i < 2; i++)
            {
                gameCanvas.Children.Add(_gridManager.CharactersViews[i]);
                //_gridManager.Characters[i].Position = 
                MoveItemToPlace(ref _gridManager.CharactersViews[i], _gridManager.Characters[i].Position);
            }
        }

        private void DecodeGridStartLocations(EnemyType enemyType)
        {
            GridItem[,] gridItems = new GridItem[yLength(), xLength()];
            GridItemView[,] gridItemsViews = new GridItemView[yLength(), xLength()];
            Character[] characters = new Character[2];
            GridItemView[] charactersView = new GridItemView[2];
            List<Exitable> exitables = new List<Exitable>();
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
                              PlayerOne playerOne = new PlayerOne()
                              {
                                  Position = new Position(x, y)
                              };
                              CharacterItem playerOneItem = new CharacterItem(playerOne);
                              GridItemView playerOneView = new GridItemView(playerOneItem);
                              //gridItems[y, x] = playerOneItem;
                              gridItems[y, x] = new Walkable()
                              {
                                  Position = new Position(x, y)
                              };
                              gridItemsViews[y, x] = new GridItemView(gridItems[y, x]);
                              charactersView[0] = playerOneView;
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
                                      //enemy = new BotPlayer();
                                      //break;
                                  case EnemyType.Remote:
                                  default:
                                      throw new NotImplementedException(
                                          $"{nameof(enemyType)} players are not implemented");
                              }

                              enemy.Position = new Position(x, y);
                              var enemyItem = new CharacterItem(enemy);

                              var enemyView = new GridItemView(enemyItem);
                              gridItems[y, x] = new Walkable
                              {
                                  Position = new Position(x, y)
                              };
                              gridItemsViews[y, x] = new GridItemView(gridItems[y, x]);
                              charactersView[1] = enemyView;
                              characters[1] = enemy;
                              break;
                          //Exitable block
                          case 5:
                              Exitable exitable = new Exitable()
                              {
                                  Position = new Position(x, y)
                              };
                              GridItemView exitableView = new GridItemView(exitable);
                              gridItems[y, x] = exitable;
                              gridItemsViews[y, x] = exitableView;
                              exitables.Add(exitable);
                              break;
                          default:
                              throw new NotImplementedException($"The value of {gridStartLocations[y,x]} is not implemented in Level.Level.GridStartLocation()");
                    }
                }
            }
            _gridManager = GameGridManager.NewGameGrid(characters, charactersView, gridItemsViews, gridItems, exitables.ToArray());
        }

        public void MoveItemToPlace(ref GridItemView itemView, Position location)
        {
            Canvas.SetLeft(itemView, location.x * Constants.GRID_ITEM_WIDTH);
            Canvas.SetTop(itemView, location.y * Constants.GRID_ITEM_WIDTH);
        }

        public void MoveCharacter(int characterNo, Direction dir)
        {
            //Cleanup canvas if using a debugging mode
            if (_gridManager.DebuggingCanvasLeftovers > 0)
            {
                UIElementCollection canvasItems = _gridManager.GameCanvas.Children;
                canvasItems.RemoveRange(canvasItems.Count - _gridManager.DebuggingCanvasLeftovers - 1, _gridManager.DebuggingCanvasLeftovers);
                _gridManager.DebuggingCanvasLeftovers = 0;
            }


            GridItemView characterView = GetCharacterView(characterNo);
            //If character won't collide with the wall
            if (!WallCollisionDetection(ref characterView, dir))
            {
                MoveCharacterInternal(ref characterView, dir);
            }

            if (EnemyCollisionDetection())
            {
                MessageBox.Show("Enemy killed you.");
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

        /// <summary>
        /// Checks whether a character will collide into a wall with their movement
        /// </summary>
        /// <param name="characterView">The actual characters view</param>
        /// <param name="movementDirection">The direction the character would move</param>
        /// <param name="canvas">The canvas to draw rectangles on if visualising the collision detection</param>
        /// <returns>Whether the character will collide</returns>
        private bool WallCollisionDetection(ref GridItemView characterView, Direction movementDirection)
        {
            double x, y = 0;
            x = Canvas.GetLeft(characterView);
            y = Canvas.GetTop(characterView);
            int xApprox, yApprox = 0;
            const double half_GRID_ITEM_WIDTH = Constants.GRID_ITEM_WIDTH / 2;
            Queue<GridItemView> ItemsToCheckForCollision = new Queue<GridItemView>(3);
            //Get approx co-ords
            xApprox = (int) Math.Floor((x + half_GRID_ITEM_WIDTH) / Constants.GRID_ITEM_WIDTH);
            yApprox = (int) Math.Floor((y + half_GRID_ITEM_WIDTH) / Constants.GRID_ITEM_WIDTH);
            switch (movementDirection)
            {
                case Direction.Up:
                    //get three possible collisionable items above
                    if (yApprox == 0)
                    {
                        //then can't look any higher so look for distance from top wall
                        // (if y is smaller than 4 then it will collide, otherwise it wont).
                        return (y <= Constants.KEYPRESS_PX_MOVED);
                    }
                    else
                    {
                        ItemsToCheckForCollision = QueuingLocationsToCheck(ref xApprox, ref yApprox, 0 , -1);
                        //Override y to be the moved value so that they can be checked for intersection
                        y -= Constants.KEYPRESS_PX_MOVED;
                    }
                    break;
                case Direction.Down:
                    //get three possible collisionable items below
                    if (yApprox == yLength())
                    {
                        //then can't look any higher so look for distance from top wall
                        // (if y is greater than 396 then it will collide, otherwise it wont).
                        return (y >= Constants.GRID_WIDTH - Constants.KEYPRESS_PX_MOVED);
                    }
                    else
                    {
                        ItemsToCheckForCollision = QueuingLocationsToCheck(ref xApprox, ref yApprox, 0 , 1);

                        //Override y to be the moved value so that they can be checked for intersection
                        y += Constants.KEYPRESS_PX_MOVED;
                    }
                    break;
                case Direction.Left:
                    //get three possible collisionable items below
                    if (xApprox == xLength())
                    {
                        //then can't look any higher so look for distance from top wall
                        // (if x is greater than 396 then it will collide, otherwise it wont).
                        return (x >= Constants.KEYPRESS_PX_MOVED);
                    }
                    else
                    {
                        ItemsToCheckForCollision = QueuingLocationsToCheck(ref xApprox, ref yApprox, -1 , 0);

                        //Override x to be the moved value so that they can be checked for intersection
                        x -= Constants.KEYPRESS_PX_MOVED;
                    }

                    break;
                case Direction.Right:
                    //get three possible collisionable items below
                    if (xApprox == xLength())
                    {
                        //then can't look any higher so look for distance from top wall
                        // (if x is greater than 396 then it will collide, otherwise it wont).
                        return (x >= Constants.GRID_WIDTH - Constants.KEYPRESS_PX_MOVED);
                    }
                    else
                    {
                        ItemsToCheckForCollision = QueuingLocationsToCheck(ref xApprox, ref yApprox, 1, 0);

                        //Override x to be the moved value so that they can be checked for intersection
                        x += Constants.KEYPRESS_PX_MOVED;
                    }
                    break;
                default:
                    throw new NotImplementedException($"Direction value of {nameof(movementDirection)} is not implemented in Level.WallCollisionDetection()");
            }

            //Rectangle variable is to check to see if it intersects
            Rect characterRect = new Rect(x + 1, y + 1, Constants.GRID_ITEM_WIDTH - 2 , Constants.GRID_ITEM_WIDTH - 2);

            //Only display the rectangles if they are wanted
            if (_gridManager.WallCollisionRectangles)
            {
                Canvas canvas = _gridManager.GameCanvas;
                Rectangle charcterRectangle = new Rectangle()
                {
                    Width = characterRect.Width,
                    Height = characterRect.Height,
                    Fill = new SolidColorBrush(Colors.Blue)
                };

                canvas.Children.Add(charcterRectangle);
                Canvas.SetLeft(charcterRectangle, characterRect.Left);
                Canvas.SetTop(charcterRectangle, characterRect.Top);
            }

            _gridManager.DebuggingCanvasLeftovers += ItemsToCheckForCollision.Count + 1;

            bool collision = false;

            while (ItemsToCheckForCollision.Count > 0)
            {
                GridItemView tempItem = ItemsToCheckForCollision.Dequeue();
                Rect nonWalkableRect = new Rect(Canvas.GetLeft(tempItem), Canvas.GetTop(tempItem), Constants.GRID_ITEM_WIDTH, Constants.GRID_ITEM_WIDTH);
                if (nonWalkableRect.IntersectsWith(characterRect) || collision /*== true*/)
                {
                    collision = true;
                }

                //Only display rectangles if they are wanted to debug
                if (_gridManager.WallCollisionRectangles)
                {
                    Rectangle nonwalkableRectangle = new Rectangle()
                    {
                        Width = characterRect.Width,
                        Height = characterRect.Height,
                        Fill = new SolidColorBrush(Colors.Blue)
                    };
                    _gridManager.GameCanvas.Children.Add(nonwalkableRectangle);
                    Canvas.SetLeft(nonwalkableRectangle, nonWalkableRect.Left);
                    Canvas.SetTop(nonwalkableRectangle, nonWalkableRect.Top);
                }
            }

            return collision;
        }

        private Queue<GridItemView> QueuingLocationsToCheck(ref int xApprox, ref int yApprox, int xcheck, int ycheck)
        {
            Queue<GridItemView> queue = new Queue<GridItemView>(3);
            //If xCheck is 0 then it should be replaced by i, otherwise yCheck should shouldnt
            bool xCheckIsi = xcheck == 0;
            for (int i = -1; i <= 1; i++)
            {
                //If xcheck is the variable value (i.e. the direction you want to check for multiple items, e.g. if you're checking up you want to check for up,
                //up-left, up-right etc so xcheck changes, same with down). Otherwise ycheck is the variable (when moving left or right)
                switch (xCheckIsi)
                {
                       case true:
                           xcheck = i;
                           break;
                       case false:
                           ycheck = i;
                           break;
                }

                //If outside the _gridManager
                if (xApprox + xcheck < 0 || yApprox + ycheck < 0 || xApprox + xcheck > (Constants.GRID_TILES_XY - 1) ||
                    yApprox + ycheck > (Constants.GRID_TILES_XY - 1))
                {
                    break;
                }
                //Check for right-above, right and right-below, and if they are non-walkable
                if (_gridManager.GridItems[yApprox + ycheck, xApprox + xcheck].GetType() == typeof(NonWalkable))
                {
                    //Then add to the queue
                    queue.Enqueue(_gridManager.GridItemsViews[yApprox + ycheck, xApprox + xcheck]);
                }
            }

            return queue;
        }

        public bool EnemyCollisionDetection()
        {

            //Get the characters views
            GridItemView characterOneView = GetCharacterView(1);
            GridItemView characterTwoView = GetCharacterView(2);
            //Create both character rectangles
            Rect char1Rect = new Rect(Canvas.GetLeft(characterOneView) + 1, Canvas.GetTop(characterOneView) + 1, characterOneView.ActualWidth - 2, characterOneView.ActualHeight - 2);
            Rect char2Rect = new Rect(Canvas.GetLeft(characterTwoView) + 1, Canvas.GetTop(characterTwoView) + 1, characterTwoView.ActualWidth - 2, characterTwoView.ActualHeight - 2);

            
            if (_gridManager.EnemyCollisionRectangles)
            {
                Canvas canvas = _gridManager.GameCanvas;
                Rectangle char1Rectangle = new Rectangle()
                {
                    Width = char1Rect.Width,
                    Height = char1Rect.Height,
                    Fill = new SolidColorBrush(Colors.DarkMagenta)
                };
                Rectangle char2Rectangle = new Rectangle()
                {
                    Width = char2Rect.Width,
                    Height = char2Rect.Height,
                    Fill = new SolidColorBrush(Colors.DarkMagenta)
                };

                canvas.Children.Add(char1Rectangle);
                canvas.Children.Add(char2Rectangle);

                Canvas.SetLeft(char1Rectangle, char1Rect.Left);
                Canvas.SetLeft(char2Rectangle, char2Rect.Left);

                Canvas.SetTop(char1Rectangle, char1Rect.Top);
                Canvas.SetTop(char2Rectangle, char2Rect.Top);
                _gridManager.DebuggingCanvasLeftovers += 2;
            }
            //Returns whether they intersect.
            return char1Rect.IntersectsWith(char2Rect);
        }

        public GridItemView GetCharacterView(int characterNo)
        {
            switch (characterNo)
            {
                case 1:
                    return _gridManager.CharactersViews[0];
                case 2:
                    return _gridManager.CharactersViews[1];
                default:
                    throw new NotImplementedException(
                        $"Player {characterNo} is not implemented in Level.Level.GetCharactersView()");
            }
        }
    }
}