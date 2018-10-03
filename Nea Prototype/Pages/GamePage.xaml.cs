using Nea_Prototype.Characters;
using Nea_Prototype.Grid;
using Nea_Prototype.Level;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Nea_Prototype.Enums;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// The main page that is used for the game
    /// </summary>
    public partial class GamePage : Page
    {
        //The timer that checks for keyboard input
        private DispatcherTimer keyboardInputTimer;
        private DispatcherTimer rotationTimer;

        //The storage for level information
        private Level.Level level = LevelIO.ReadJSON("testing.json");

        public GamePage()
        {
            InitializeComponent();
            
            //Sets up the grid by decoding the int array and placing everything on the canvas
            level.SetupGrid(ref cvsPlayArea, EnemyType.Local);
            //Set the canvas of the singleton for easier access to the canvas (so the canvas does
            //not need to be referneced every tick for the collision detection visualisation to work)
            GameGridManager.GetGameGrid().GameCanvas = cvsPlayArea;
            keyboardInputTimer = new DispatcherTimer()
            {
                //Every ~1/1000 of a second update
                Interval = new TimeSpan(0, 0, 0, 0, 1)
            };
            //Have the timer use the timertick event
            keyboardInputTimer.Tick += KeyboardInputTimerTick;
            
            rotationTimer = new DispatcherTimer()
            {
                //Update every half second
                Interval = new TimeSpan(0,0,0,0,500)
            };
            rotationTimer.Tick += (s, e) =>
            {

                double rotationAbs = GameGridManager.GetGameGrid().PreviousAngle;
                double randomRotation = rng.Next(45) *
                                        Algorithms.Rotation.RotationMultiplier(GameGridManager.GetGameGrid().Characters,
                                            ref rotationAbs);

                GameGridManager.RotateStoryBoard((int) randomRotation);
            };
            //When the page has loaded start the timer
            Loaded += (s, e) =>
            {
                keyboardInputTimer.Start();
                rotationTimer.Start();
            };
        }

        /// <summary>
        /// Everytime the timer ticks check for keyboard input
        /// </summary>
        /// <param name="sender">The timer</param>
        /// <param name="e">The event arguments</param>
        private void KeyboardInputTimerTick(object sender, EventArgs e)
        {
            //Timer is used for keyboard inputs so that the user can press two directions
            //and go diagonally, and so 2 players can play at once

            double getLeft;
            double getUp;
            if (Keyboard.IsKeyDown(KeyBindings.Player1_right))
            {
                level.MoveCharacter(1, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player1_left))
            {
                level.MoveCharacter(1, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindings.Player1_up))
            {
                level.MoveCharacter(1, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player1_down))
            {
                level.MoveCharacter(1, Direction.Down);
            }

            if (Keyboard.IsKeyDown(KeyBindings.Player2_right))
            {
                level.MoveCharacter(2, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player2_left))
            {
                level.MoveCharacter(2, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindings.Player2_up))
            {
                level.MoveCharacter(2, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player2_down))
            {
                level.MoveCharacter(2, Direction.Down);
            }

        }

        /// <summary>
        /// When a key is pressed also run the same code for the timer (this is so that
        /// when a user presses a key once very quickly it still moves the player
        /// </summary>
        /// <param name="sender">Keydown event</param>
        /// <param name="e">The arguments to do with the key pressed</param>
        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            KeyboardInputTimerTick(sender, e);
        }
        static Random rng = new Random();
    }
}
