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

        private EnemyType Enemy;
        private ProtagonistType Protagonist;

        private GameType gameType
        {
            get
            {
                if (Protagonist == ProtagonistType.Remote || Enemy == EnemyType.Remote)
                {
                    //Remote game
                    return GameType.Networked;
                }
                else if (Enemy == EnemyType.AI)
                {
                    //AI game
                    return GameType.Singleplayer;
                }
                else
                {
                    //Local multiplayer game
                    return GameType.LocalMultiplayer;
                }
            }
        }

        //Which charcater is binded to WASD (or whatever player 1 is bound to) (e.g. player 1 for AI or Local, either for networked)
        private int MainCharacterKeyBind
        {
            get
            {
                if (Protagonist == ProtagonistType.Remote)
                {
                    return 2;
                }
                else
                {
                    //Player 1 is default 
                    return 1;
                }
            }
        }


        //The connection to the message manager if networked
        private MessageManager messageInstance = null;

        //The storage for level information
        private Level.Level level = null;

        /// <summary>
        /// Creates an instance of GamePage
        /// </summary>
        /// <param name="pt">The type of protagonist to generate</param>
        /// <param name="et">The type of enemy to generate</param>
        public GamePage(ProtagonistType pt, EnemyType et, Level.Level _level)
        {
            InitializeComponent();
            level = _level;

            Protagonist = pt;
            Enemy = et;

            //Sets up the grid by decoding the int array and placing everything on the canvas
            level.SetupGrid(ref cvsPlayArea, ref cvsExitArea, ProtagonistType.Local, EnemyType.Local);
            //Set the canvas of the singleton for easier access to the canvas (so the canvas does
            //not need to be referenced every tick for the collision detection visualisation to work)
            GameGridManager.Instance.GameCanvas = cvsPlayArea;

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

                double rotationAbs = GameGridManager.Instance.PreviousAngle;
                double rotation = Rotation.AbsAngleDelta() *
                                        Algorithms.Rotation.RotationMultiplier(GameGridManager.Instance.Characters,
                                            ref rotationAbs);

                GameGridManager.RotateStoryBoard((int) rotation);
            };
            
            //If there is some networking involved within characters then start the communication manager and tie it to the message manager
            if (gameType == GameType.Networked)
            {
                CommunicationManager.Instance.SetupEnemyTypes(pt, et);
                //Also tell the server that it has received and loaded the map
                
                messageInstance = MessageManager.Instance;
                messageInstance.MessageHandler += HandleMessage;
                messageInstance.SendMessage("received");

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
            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player1_right))
            {
                level.MoveCharacter(MainCharacterKeyBind, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player1_left))
            {
                level.MoveCharacter(MainCharacterKeyBind, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player1_up))
            {
                level.MoveCharacter(MainCharacterKeyBind, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player1_down))
            {
                level.MoveCharacter(MainCharacterKeyBind, Direction.Down);
            }
            
            //Section for overlay menus
            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.PauseMenuKey))
            {
                StopTimers();
                TopFrameManager.FrameManager.OverlayFrame.Navigate(new PauseMenuPage());
            }

            if (Keyboard.Modifiers == KeyBindingsManager.Instance.DebugOverlayModifier &&
                Keyboard.IsKeyDown(KeyBindingsManager.Instance.DebugOverlayKey))
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



            //If the game is networked or AI there is only one character
            if (gameType == GameType.Networked || gameType == GameType.Singleplayer)
            {
                return;
            }

            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player2_right))
            {
                level.MoveCharacter(2, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player2_left))
            {
                level.MoveCharacter(2, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player2_up))
            {
                level.MoveCharacter(2, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player2_down))
            {
                level.MoveCharacter(2, Direction.Down);
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


        public void HandleMessage(object sender, EventArgs e)
        {
            if (e != null && e is MessageEventArgs)
            {
                try
                {
                    

                    string receivedMessage = (e as MessageEventArgs).Message;
                    //Check if the clients are still alive
                    if (receivedMessage == "crash" || receivedMessage == "close")
                    {
                        
                        CommunicationManager.Instance.Disconnect();
                        MessageBox.Show(receivedMessage == "crash" ? "Other player has exited unexpectedly" : "Other player has willingly quit.");
                    }
                    
                    //break up string

                    string[] messageComponents = receivedMessage.Split(',');
                    int characterNumber = int.Parse(messageComponents[0]);
                    double x = double.Parse(messageComponents[1]);
                    double y = double.Parse(messageComponents[2]);

                    //Move characters into position
                    //Get main thread dispatcher
                    Dispatcher dispatcher =
                        GameGridManager.Instance.CharactersViews[characterNumber - 1].Dispatcher;
                    dispatcher.Invoke(new Action(() =>
                    {
                        Canvas.SetLeft(GameGridManager.Instance.CharactersViews[characterNumber - 1], x);
                        Canvas.SetTop(GameGridManager.Instance.CharactersViews[characterNumber - 1], y);
                        for (int i = 0; i < GameGridManager.Instance.ExitLocations.Length; i++)
                        {
                            int j = i + 3;
                            //For each is left these are the exit conditions
                            bool isGateOpen = GameGridManager.Instance.ExitLocations[i].CanExit;
                            if (!bool.TryParse(messageComponents[j], out isGateOpen))
                            {
                                Console.WriteLine($"Couldn't resolve messageComponents[{j}] as a boolean. Value: {messageComponents[j]}");
                            }
                            GameGridManager.Instance.ExitLocations[i].CanExit = isGateOpen;
                        }
                    }));
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
                MessageManager.Instance.MessageHandler -= HandleMessage;
                CommunicationManager.Instance.Disconnect();
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
