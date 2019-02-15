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

        /// <summary>
        /// Rotation storyboard setup for the canvas
        /// </summary>
        /// <param name="angleDiff">The how much to rotate in what direction</param>
        public static void RotateStoryBoard(int angleDiff)
        {
            //Get the angle that the grid will change to
            int newAngle = Instance.PreviousAngle + angleDiff;
            //Make sure that it doesn't rotate past +/- 90 degrees
            if (newAngle >= 90)
            {
                newAngle = 90;
            }
            else if (newAngle <= -90)
            {
                newAngle = -90;
            }

            //Make sure the rotation storyboard is available to be overwritten (if above 90% complete that is assumed to be
            //a good time to do this)
            if (Instance.rotationStoryboard is null || Instance.rotationStoryboard?.GetCurrentProgress() >= 0.9)
            {
                //Create a storyboard and set its time
                Instance.rotationStoryboard = new Storyboard();
                Instance.rotationStoryboard.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
                //Make sure the animation goes from the current to the next angle smoothly and set its length
                DoubleAnimation animation = new DoubleAnimation()
                {
                    From = Instance.PreviousAngle,
                    To = newAngle,
                    Duration = Instance.rotationStoryboard.Duration
                };
                Instance.rotationStoryboard.Children.Add(animation);
                //Actually set the storyboard to work on the canvas
                Storyboard.SetTarget(animation, Instance.GameCanvas);
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

                //Start the storyboard
                Instance.rotationStoryboard.Begin();

                //Set the current angle to be the next angle for future calculations
                Instance.PreviousAngle = newAngle;
            }

            //Check to see if the exits need to be updated (unnecessary in networked since this is sent with every transmission)
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