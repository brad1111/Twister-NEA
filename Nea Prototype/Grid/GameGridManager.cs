using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Nea_Prototype.Characters;
using Nea_Prototype.Enums;
using Newtonsoft.Json;

namespace Nea_Prototype.Grid
{
    /// <summary>
    /// Singleton that manages all data in relation to the grid
    /// </summary>
    public class GameGridManager
    {
        private Character[] characters;
        private GridItemView[] charactersView;
        private GridItemView[,] gridItemsViews;
        private GridItem[,] gridItems;
        private Exitable[] exitLocations;
        private GridItemView[] exitLocationsViews;
        private Storyboard rotationStoryboard = null;
        public int PreviousAngle { get; set; }
        
        private static readonly GameGridManager gameGridStorage = new GameGridManager();

        private GameGridManager()
        {
        }
        
        /// <summary>
        /// Creates a new grid (replacing an old one)
        /// </summary>
        /// <param name="characters">The characters to store<param>
        /// <param name="charactersView">The views of the characters to store</param>
        /// <param name="gridItemsViews">The views of all of the grid items to store</param>
        /// <param name="gridItems">The non-view part of the grid items to store</param>
        /// <param name="exitableLocations">The non-view part of the internal exitable locations</param>
        /// <returns>The instance of the new singleton</returns>
        public static GameGridManager NewGameGrid(Character[] characters, GridItemView[] charactersView, GridItemView[,] gridItemsViews, GridItem[,] gridItems, Exitable[] exitableLocations, GridItemView[] exitLocationsViews)
        {
            gameGridStorage.characters = characters;
            gameGridStorage.charactersView = charactersView;
            gameGridStorage.gridItemsViews = gridItemsViews;
            gameGridStorage.gridItems = gridItems;
            gameGridStorage.exitLocations = exitableLocations;
            gameGridStorage.exitLocationsViews = exitLocationsViews;
            return gameGridStorage;
        }

        /// <summary>
        /// Gets the current instance of the singleton
        /// </summary>
        /// <returns>The current instance of the signleton</returns>
        public static GameGridManager GetGameGrid()
        {
            return gameGridStorage;
        }

        public static void RotateStoryBoard(int angleDiff)
        {
            int newAngle = GetGameGrid().PreviousAngle + angleDiff;
            if (newAngle >= 90)
            {


                newAngle = 90;
                //Should ideally change the time but dont bother yet
            }
            else if (newAngle <= -90)
            {
                newAngle = -90;
            }

            if (GetGameGrid().rotationStoryboard is null || GetGameGrid().rotationStoryboard?.GetCurrentProgress() >= 0/*.8*/)
            {
                GetGameGrid().rotationStoryboard = new Storyboard();
                GetGameGrid().rotationStoryboard.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
                DoubleAnimation animation = new DoubleAnimation()
                {
                    From = GetGameGrid().PreviousAngle,
                    To = newAngle,
                    Duration = GetGameGrid().rotationStoryboard.Duration
                };
                GetGameGrid().rotationStoryboard.Children.Add(animation);
                Storyboard.SetTarget(animation, GetGameGrid().GameCanvas);
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

                GetGameGrid().rotationStoryboard.Begin();

                GetGameGrid().PreviousAngle = newAngle;
            }
        }


        /// <summary>
        /// The canvas used by the game
        /// </summary>
        public Canvas GameCanvas { get; set; }

        /// <summary>
        /// All of the characters
        /// </summary>
        public Character[] Characters
        {
            get => characters;
        }

        /// <summary>
        /// All of the characters views
        /// </summary>
        public GridItemView[] CharactersViews
        {
            get => charactersView;
        }

        /// <summary>
        /// All of the views of the grid items
        /// </summary>
        public GridItemView[,] GridItemsViews
        {
            get => gridItemsViews;
        }

        /// <summary>
        /// All of the exit locations
        /// </summary>
        public Exitable[] ExitLocations
        {
            get => exitLocations;
        }

        

        public GridItem[,] GridItems
        {
            get => gridItems;
        }

        public GridItemView[] ExitLocationsViews
        {
            get => exitLocationsViews;
        }

        public int DebuggingCanvasLeftovers { get; set; }

        public EnemyType EnemyType { get; set; }
        

        #region Debugging Variables

        public bool WallCollisionRectangles { get; set; }
        public bool EnemyCollisionRectangles { get; set; }
        

        #endregion
    }
}