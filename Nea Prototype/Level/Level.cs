using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Schema;
using Nea_Prototype.Algorithms;
using Nea_Prototype.Characters;
using Nea_Prototype.Enums;
using Nea_Prototype.Grid;
using Newtonsoft.Json;

namespace Nea_Prototype.Level
{
    public class Level
    {
        /// <summary>
        /// The singleton that stores everything to do with the game grid
        /// </summary>
        [JsonIgnore] private GameGridManager _gridManager;

        //These need to be public Properties so that JSON.net can fill these values from the json file.
        /// <summary>
        /// The integer array which represents the layout of the grid
        /// </summary>
        [JsonProperty("StartLocations")] public int[,] gridStartLocations { get; internal set; }
        /// <summary>
        /// The location of the exit
        /// </summary>
        public ExitPlacement ExitLocation { get; set; }
        

        /// <summary>
        ///  y
        /// </summary>
        /// <returns>The yLength of the gridStartLocations array</returns>
        private int yLength()
        {
            return gridStartLocations.GetLength(0);
        }

        /// <summary>
        /// x
        /// </summary>
        /// <returns>The yLength of the gridStartLocations array</returns>
        private int xLength()
        {
            return gridStartLocations.GetLength(1); 
        }

       
        public Level()
        {
            
        }

        /// <summary>
        /// Sets up the grid in terms of decoding the integer array into items
        /// and sets up the canvas.
        /// </summary>
        /// <param name="gameCanvas">The canvas reference is needed</param>
        /// <param name="enemyType"></param>
        public void SetupGrid(ref Canvas gameCanvas, ref Canvas exitCanvas, EnemyType enemyType)
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

            //Add exit rectangle
            exitCanvas.Children.Add(ExitRectanglePlacement());
        }

        /// <summary>
        /// Converts the integer array for the start locations into the grid items, grid views, characters, characters' views, and exitable items
        /// </summary>
        /// <param name="enemyType">The type of enemy to instatiate</param>
        private void DecodeGridStartLocations(EnemyType enemyType)
        {
            GridItem[,] gridItems = new GridItem[yLength(), xLength()];
            GridItemView[,] gridItemsViews = new GridItemView[yLength(), xLength()];
            Character[] characters = new Character[2];
            GridItemView[] charactersView = new GridItemView[2];
            List<Exitable> exitables = new List<Exitable>();
            List<GridItemView> exitableViews = new List<GridItemView>();
            int exitableIndex = 0;
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
                              Enemy enemy = new Enemy();

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
                              //Set the array index to the correct value and immedietly increment
                              Exitable exitable = new Exitable(exitableIndex++)
                              {
                                  Position = new Position(x, y)
                              };
                              GridItemView exitableView = new GridItemView(exitable);
                              gridItems[y, x] = exitable;
                              gridItemsViews[y, x] = exitableView;
                              exitables.Add(exitable);
                              exitableViews.Add(exitableView);
                              break;
                          default:
                              throw new NotImplementedException($"The value of {gridStartLocations[y,x]} is not implemented in Level.Level.GridStartLocation()");
                    }
                }
            }
            _gridManager = GameGridManager.NewGameGrid(characters, charactersView, gridItemsViews, gridItems, exitables.ToArray(), exitableViews.ToArray());
        }

        /// <summary>
        /// Moves an item of certain position to that location on the canvas
        /// </summary>
        /// <param name="itemView">The item to move</param>
        /// <param name="location">Where to move it</param>
        public void MoveItemToPlace(ref GridItemView itemView, Position location)
        {
            Canvas.SetLeft(itemView, location.x * Constants.GRID_ITEM_WIDTH);
            Canvas.SetTop(itemView, location.y * Constants.GRID_ITEM_WIDTH);
        }

        /// <summary>
        /// Moves a character from a position up, down, left, or right depending on whether
        /// it is able to move to that position
        /// </summary>
        /// <param name="characterNo">The character to move</param>
        /// <param name="dir">The direction to move the character in</param>
        public void MoveCharacter(int characterNo, Direction dir)
        {
            //Cleanup canvas if using a debugging mode
            if (_gridManager.DebuggingCanvasLeftovers > 0)
            {
                UIElementCollection canvasItems = _gridManager.GameCanvas.Children;
                canvasItems.RemoveRange(canvasItems.Count - _gridManager.DebuggingCanvasLeftovers, _gridManager.DebuggingCanvasLeftovers);
                _gridManager.DebuggingCanvasLeftovers = 0;
            }


            GridItemView characterView = GetCharacterView(characterNo);
            //If character won't collide with the wall
            if (!Collisions.WallCollisionDetection(ref characterView, dir))
            {
                MoveCharacterInternal(ref characterView, dir);
            }

            if (Collisions.EnemyCollisionDetection())
            {
                MessageBox.Show("Enemy killed you.");
            }
        }

        /// <summary>
        /// Where the character is actually moved (no checks on whether it can be done, just does it)
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="dir"></param>
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

        public Rectangle ExitRectanglePlacement()
        {
            int x1Temp = 0, y1Temp = 0, x2Temp = 0, y2Temp = 0;

            switch (ExitLocation.SidePlaced)
            {
                case Side.Top:
                    x1Temp = ExitLocation.HeightFromAnchor;
                    x2Temp = x1Temp + ExitLocation.Length;
                    y1Temp = -50;
                    y2Temp = -60;
                    break;
                case Side.Bottom:
                    x1Temp = ExitLocation.HeightFromAnchor;
                    x2Temp = x1Temp + ExitLocation.Length;
                    y1Temp = Constants.GRID_WIDTH + 70;
                    y2Temp = Constants.GRID_WIDTH + 60;
                    break;  
                case Side.Left:
                    x1Temp = -70;
                    x2Temp = -60;
                    y1Temp = ExitLocation.HeightFromAnchor;
                    y2Temp = y1Temp + ExitLocation.Length;
                    break;
                case Side.Right:
                    x1Temp = Constants.GRID_WIDTH + 50;
                    x2Temp = Constants.GRID_WIDTH + 60;
                    y1Temp = ExitLocation.HeightFromAnchor;
                    y2Temp = y1Temp + ExitLocation.Length;
                    break;
                default:
                    throw new NotImplementedException($"Side {nameof(ExitLocation.SidePlaced)} is not implemented");
            }
            Rectangle rectangle = new Rectangle()
            {
                Height =  y2Temp - y1Temp,
                Width = x2Temp - x1Temp,
                Fill = new SolidColorBrush(Colors.GreenYellow)
            };
            Canvas.SetLeft(rectangle, x1Temp);
            Canvas.SetTop(rectangle, y1Temp);
            return rectangle;
        }

        /// <summary>
        /// Converts a charcater number into their respective CharacterView
        /// </summary>
        /// <param name="characterNo">The characters number, either player 1 or player 2</param>
        /// <returns>The characters' view</returns>
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