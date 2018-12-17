using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Twister.Algorithms;
using Twister.Characters;
using Twister.Network;

namespace Twister.Grid
{
    /// <summary>
    /// Singleton that manages all data in relation to the grid
    /// </summary>
    public class GameGridManager
    {
        private Character[] characters;
        private CharacterItem[] charactersViews;
        private GridItem[,] gridItems;
        private Exitable[] exitLocations;
        private Storyboard rotationStoryboard = null;
        public int PreviousAngle { get; set; }

        public static GameGridManager Instance { get; } = new GameGridManager();

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
        public static GameGridManager NewGameGrid(Character[] characters, CharacterItem[] charactersView, GridItem[,] gridItems, Exitable[] exitableLocations)
        {
            Instance.characters = characters;
            Instance.charactersViews = charactersView;
            Instance.gridItems = gridItems;
            Instance.exitLocations = exitableLocations;
            Instance.PreviousAngle = 0;
            Instance.rotationStoryboard = null;
            Instance.GameCanvas = null;
            return Instance;
        }

        public static void RotateStoryBoard(int angleDiff)
        {
            int newAngle = Instance.PreviousAngle + angleDiff;
            if (newAngle >= 90)
            {


                newAngle = 90;
                //Should ideally change the time but dont bother yet
            }
            else if (newAngle <= -90)
            {
                newAngle = -90;
            }

            if (Instance.rotationStoryboard is null || Instance.rotationStoryboard?.GetCurrentProgress() >= 0/*.8*/)
            {
                Instance.rotationStoryboard = new Storyboard();
                Instance.rotationStoryboard.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
                DoubleAnimation animation = new DoubleAnimation()
                {
                    From = Instance.PreviousAngle,
                    To = newAngle,
                    Duration = Instance.rotationStoryboard.Duration
                };
                Instance.rotationStoryboard.Children.Add(animation);
                Storyboard.SetTarget(animation, Instance.GameCanvas);
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

                Instance.rotationStoryboard.Begin();

                Instance.PreviousAngle = newAngle;
            }

            //Check for updates for exits (if not networked)
            if (!CommunicationManager.Instance.IsNetworked)
            {
                ExitingManager.CheckForUpdates(Instance.PreviousAngle, angleDiff);
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

        public CharacterItem[] CharactersViews
        {
            get => charactersViews;
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
        

        #region Debugging Variables

        public int DebuggingCanvasLeftovers { get; set; }
        public bool WallCollisionRectangles { get; set; }
        public bool EnemyCollisionRectangles { get; set; }
        public bool ShowPath { get; set; }

        #endregion

        public static void Clear()
        {
            NewGameGrid(null, null, null, null);
        }
    }
}