using Nea_Prototype.Characters;
using Nea_Prototype.Grid;
using Nea_Prototype.Level;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Nea_Prototype.Algorithms;
using Nea_Prototype.Enums;
using Nea_Prototype.Keybindings;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// The main page that is used for the game
    /// </summary>
    public partial class GamePage : IKeyboardInputs
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
            level.SetupGrid(ref cvsPlayArea, ref cvsExitArea, EnemyType.Local);
            //Set the canvas of the singleton for easier access to the canvas (so the canvas does
            //not need to be referneced every tick for the collision detection visualisation to work)
            GameGridManager.GetGameGrid().GameCanvas = cvsPlayArea;

            //Setup the angles that open the exits
            ExitingManager.Instance.FindAnglesNeededToOpen(level.ExitLocation.HeightFromAnchor, level.ExitLocation.Length);

            keyboardInputTimer = new DispatcherTimer()
            {
                //Every ~1/1000 of a second update
                Interval = new TimeSpan(0, 0, 0, 0, 1)
            };
            //Have the timer use the timertick event
            keyboardInputTimer.Tick += KeyboardInputTimerTick;
            
            rotationTimer = new DispatcherTimer()
            {
                //Update every 1/4 second
                Interval = new TimeSpan(0,0,0,0, 250)
            };
            rotationTimer.Tick += (s, e) =>
            {

                double rotationAbs = GameGridManager.GetGameGrid().PreviousAngle;
                double rotation = Algorithms.Rotation.AbsAngleDelta() *
                                        Algorithms.Rotation.RotationMultiplier(GameGridManager.GetGameGrid().Characters,
                                            ref rotationAbs);

                GameGridManager.RotateStoryBoard((int) rotation);
            };
            //When the page has loaded start the timer
            Loaded += (s, e) =>
            {
                StartTimers();
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
            if (Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.Player1_right))
            {
                level.MoveCharacter(1, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.Player1_left))
            {
                level.MoveCharacter(1, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.Player1_up))
            {
                level.MoveCharacter(1, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.Player1_down))
            {
                level.MoveCharacter(1, Direction.Down);
            }

            if (Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.Player2_right))
            {
                level.MoveCharacter(2, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.Player2_left))
            {
                level.MoveCharacter(2, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.Player2_up))
            {
                level.MoveCharacter(2, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.Player2_down))
            {
                level.MoveCharacter(2, Direction.Down);
            }

            if (Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.PauseMenuKey))
            {
                StopTimers();
                TopFrameManager.FrameManager.OverlayFrame.Navigate(new PauseMenuPage());
            }

            if (Keyboard.Modifiers == KeyBindingsManager.KeyBindings.DebugOverlayModifier &&
                Keyboard.IsKeyDown(KeyBindingsManager.KeyBindings.DebugOverlayKey))
            {
                if (TopFrameManager.FrameManager.OverlayFrame.Content?.GetType() == typeof(DebugOverlayPage))
                {
                    TopFrameManager.FrameManager.ClearOverlayFrame();
                }
                else
                {
                    TopFrameManager.FrameManager.OverlayFrame.Navigate(new DebugOverlayPage());
                }
            }
        }

        private bool allowKeyDown = false;

        /// <summary>
        /// When a Key is pressed also run the same code for the timer (this is so that
        /// when a user presses a Key once very quickly it still moves the player
        /// </summary>
        /// <param name="sender">Keydown event</param>
        /// <param name="e">The arguments to do with the Key pressed</param>
        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (allowKeyDown)
            {
                KeyboardInputTimerTick(sender, e);
            }
        }

        public void StopTimers()
        {
            keyboardInputTimer.Stop();
            rotationTimer.Stop();
            allowKeyDown = false;
        }

        public void StartTimers()
        {
            keyboardInputTimer.Start();
            rotationTimer.Start();
            allowKeyDown = true;
        }
    }
}
