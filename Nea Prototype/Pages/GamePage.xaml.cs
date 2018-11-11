using Nea_Prototype.Characters;
using Nea_Prototype.Grid;
using Nea_Prototype.Level;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Common.Enums;
using Nea_Prototype.Algorithms;
using Nea_Prototype.Keybindings;
using Nea_Prototype.Network;

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

        //The connection to the message manager if networked
        private MessageManager messageInstance = null;

        //The storage for level information
        private Level.Level level = LevelIO.ReadJSON("testing.json");

        /// <summary>
        /// Creates an instance of GamePage
        /// </summary>
        /// <param name="pt">The type of protagonist to generate</param>
        /// <param name="et">The type of enemy to generate</param>
        public GamePage(ProtagonistType pt, EnemyType et)
        {
            InitializeComponent();
            
            //Sets up the grid by decoding the int array and placing everything on the canvas
            level.SetupGrid(ref cvsPlayArea, ref cvsExitArea, ProtagonistType.Local, EnemyType.Local);
            //Set the canvas of the singleton for easier access to the canvas (so the canvas does
            //not need to be referenced every tick for the collision detection visualisation to work)
            GameGridManager.GetGameGrid().GameCanvas = cvsPlayArea;

            //Setup the angles that open the exits
            ExitingManager.FindAnglesNeededToOpen(level.ExitLocation.HeightFromAnchor, level.ExitLocation.Length);

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
                double rotation = Rotation.AbsAngleDelta() *
                                        Algorithms.Rotation.RotationMultiplier(GameGridManager.GetGameGrid().Characters,
                                            ref rotationAbs);

                GameGridManager.RotateStoryBoard((int) rotation);
            };
            
            //If there is some networking involved within characters then start the communication manager and tie it to the message manager
            if (pt == ProtagonistType.Remote || et == EnemyType.Remote)
            {
                CommunicationManager.Instance.SetupEnemyTypes(pt, et);
            }

            //When the page has loaded start the timer
            Loaded += (s, e) =>
            {
                StartTimers();
            };
        }

        /// <summary>
        /// Every time the timer ticks check for keyboard input
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


        public void HandleMessage(object sender, MessageEventArgs e)
        {
            if (e != null)
            {
                try
                {
                    string receivedMessage = e.Message;
                    //break up string
                    string[] messageComponents = receivedMessage.Split(',');
                    int characterNumber = int.Parse(messageComponents[0]);
                    double x = double.Parse(messageComponents[1]);
                    double y = double.Parse(messageComponents[2]);

                    //Move characters into position
                    Canvas.SetLeft(GameGridManager.GetGameGrid().CharactersViews[characterNumber - 1], x);
                    Canvas.SetTop(GameGridManager.GetGameGrid().CharactersViews[characterNumber - 1], y);

                    for (int i = 3; i < messageComponents.Length; i++)
                    {
                        int j = i - 3;
                        //For each is left these are the exit conditions
                        bool isGateOpen = bool.Parse(messageComponents[i]);
                        GameGridManager.GetGameGrid().ExitLocations[j].CanExit = isGateOpen;
                    }
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }


        public void StopTimers()
        {
            keyboardInputTimer.Stop();
            rotationTimer.Stop();
            //If networked you need to stop the network timer
            if (CommunicationManager.Instance.IsNetworked)
            {
                CommunicationManager.Instance.Stop();
            }
            allowKeyDown = false;
        }

        public void EndGame()
        {
            StopTimers();
            if (CommunicationManager.Instance.IsNetworked)
            {
                CommunicationManager.Instance.ClearEnemyTypes();
            }
        }

        public void StartTimers()
        {
            keyboardInputTimer.Start();
            rotationTimer.Start();
            //If networked you need to start the network timer
            if (CommunicationManager.Instance.IsNetworked)
            {
                CommunicationManager.Instance.Start();
            }
            allowKeyDown = true;
        }
    }
}
