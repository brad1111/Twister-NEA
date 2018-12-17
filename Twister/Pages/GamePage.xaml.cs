using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Common;
using Common.Enums;
using Common.Grid;
using Twister.Algorithms;
using Twister.Grid;
using Twister.Keybindings;
using Twister.Network;

namespace Twister.Pages
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
        private DispatcherTimer aiTimer;

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
        public GamePage(ProtagonistType pt, EnemyType et, Level.Level _level, double protagonistWeight = 1, double enemyWeight = 1)
        {
            InitializeComponent();
            level = _level;

            Protagonist = pt;
            Enemy = et;

            //Sets up the grid by decoding the int array and placing everything on the canvas
            level.SetupGrid(ref cvsPlayArea, ref cvsExitArea, pt, et);

            //Set the characters weights (for turning moments)
            GameGridManager.Instance.Characters[0].Weight = protagonistWeight;
            GameGridManager.Instance.Characters[1].Weight = enemyWeight;

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


                //Also start the timers
                StartTimers();
            }

            //Setups up AI timer if this is a singleplayer game
            if (gameType == GameType.Singleplayer)
            {
                aiTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0,0,0,0,400)
                };
                aiTimer.Tick += AiTimerOnTick;
            }       

            //Allow keydown so that starts the game etc
            allowKeyDown = true;
        }

        private Storyboard AITransformStoryboard = null;
        private Position moveTo = new Position(0,0);

        private void AiTimerOnTick(object sender, EventArgs e)
        {
            //Make sure you check for collisions
            if (Collisions.EnemyCollisionDetection())
            {
                TopFrameManager.Instance.OverlayFrame.Navigate(new LosePage(level, Protagonist, Enemy,
                    CommunicationManager.Instance.IsNetworked));
            }
            //If the old storyboard is still there set the speed to be fast so that the next animation can be played
            if (AITransformStoryboard != null)
            {
                AITransformStoryboard.SpeedRatio = 10;
            }
            //Move the item to the position
            if (AITransformStoryboard is null || AITransformStoryboard.GetCurrentProgress() > 0)
            {
                

                //Setup movement
                Stack<GridItem> path = Pathfinding.FindPath();
                CharacterItem enemyView = GameGridManager.Instance.CharactersViews[1];
                switch (path.Count)
                {
                        case 0:
                            return; //Can't do anything else because there is nowhere to go
                        case 1:
                            Collisions.EnemyCollisionDetection();//Check to see if they are colliding
                            break;
                        //else continue on
                }
                GridItem nextLocation = path.Pop();
                Position currentPos = new Position(Canvas.GetLeft(enemyView),
                    Canvas.GetTop(enemyView));
                moveTo = nextLocation?.Position;
                if (moveTo is null)
                {
                    return;
                }

                //Move player
                Canvas.SetLeft(enemyView, moveTo.x * Constants.GRID_ITEM_WIDTH);
                Canvas.SetTop(enemyView, moveTo.y * Constants.GRID_ITEM_WIDTH);

                //Setup storyboard
                AITransformStoryboard = new Storyboard();
                AITransformStoryboard.Duration = new Duration(aiTimer.Interval);
                DoubleAnimation xAnimation = new DoubleAnimation()
                {
                    From = -(moveTo.x * Constants.GRID_ITEM_WIDTH - currentPos.x),
                    To = 0,
                    Duration = AITransformStoryboard.Duration
                };
                DoubleAnimation yAnimation = new DoubleAnimation()
                {
                    From = -(moveTo.y * Constants.GRID_ITEM_WIDTH - currentPos.y),
                    To = 0, //Because this is a delta move calculate move
                    Duration = AITransformStoryboard.Duration
                };
                AITransformStoryboard.Children.Add(xAnimation);
                AITransformStoryboard.Children.Add(yAnimation);
                Storyboard.SetTarget(xAnimation, GameGridManager.Instance.CharactersViews[1]);
                Storyboard.SetTargetProperty(xAnimation,
                    new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                Storyboard.SetTarget(yAnimation, GameGridManager.Instance.CharactersViews[1]);
                Storyboard.SetTargetProperty(yAnimation,
                    new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

                //Go
                AITransformStoryboard.Begin();
            }
        }

        /// <summary>
        /// Every time the timer ticks check for keyboard input
        /// </summary>
        /// <param name="sender">The timer</param>
        /// <param name="e">The event arguments</param>
        private void KeyboardInputTimerTick(object sender, EventArgs e)
        {
            //If the level file has been nullified don't continue
            if (level is null)
            {
                return;
            }


            //Timer is used for keyboard inputs so that the user can press two directions
            //and go diagonally, and so 2 players can play at once

            double getLeft;
            double getUp;
            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player1_right))
            {
                level?.MoveCharacter(MainCharacterKeyBind, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player1_left))
            {
                level?.MoveCharacter(MainCharacterKeyBind, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player1_up))
            {
                level?.MoveCharacter(MainCharacterKeyBind, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player1_down))
            {
                level?.MoveCharacter(MainCharacterKeyBind, Direction.Down);
            }
            
            //Section for overlay menus
            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.PauseMenuKey))
            {
                StopTimers();
                TopFrameManager.Instance.OverlayFrame.Navigate(new PauseMenuPage());
            }

            if (Keyboard.Modifiers == KeyBindingsManager.Instance.DebugOverlayModifier &&
                Keyboard.IsKeyDown(KeyBindingsManager.Instance.DebugOverlayKey))
            {
                if (TopFrameManager.Instance.OverlayFrame.Content?.GetType() == typeof(DebugOverlayPage))
                {
                    TopFrameManager.Instance.ClearOverlayFrame();
                }
                else
                {
                    TopFrameManager.Instance.OverlayFrame.Navigate(new DebugOverlayPage());
                }
            }



            //If the game is networked or AI there is only one character
            if (gameType == GameType.Networked || gameType == GameType.Singleplayer)
            {
                return;
            }

            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player2_right))
            {
                level?.MoveCharacter(2, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player2_left))
            {
                level?.MoveCharacter(2, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player2_up))
            {
                level?.MoveCharacter(2, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindingsManager.Instance.Player2_down))
            {
                level?.MoveCharacter(2, Direction.Down);
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
                //And also if a key is pressed and the AI timer hasn't started and it needs to start it
                //Only for local games
                if (!timersEnabled)
                {
                    StartTimers();
                }
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
                    switch (receivedMessage)
                    {
                        //Check if the clients are still alive
                        case "crash":
                        case "close":
                            CommunicationManager.Instance.Disconnect();
                            MessageBox.Show(receivedMessage == "crash" ? "Other player has exited unexpectedly" : "Other player has willingly quit.");
                            break;
                        case "collided":
                            TopFrameManager.Instance.OverlayFrame.Dispatcher.Invoke(new Action(
                                () => TopFrameManager.Instance.OverlayFrame.Navigate(new LosePage(level, Protagonist, Enemy, isNetworked:true))
                            ));
                            
                            break;
                        case "won":
                            TopFrameManager.Instance.OverlayFrame.Dispatcher.Invoke(new Action(
                                () => TopFrameManager.Instance.OverlayFrame.Navigate(new WinPage())
                            ));
                            break;
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
            Console.WriteLine("Timers stopped");
            keyboardInputTimer.Stop();
            rotationTimer.Stop();
            if (gameType == GameType.Singleplayer)
            {
                aiTimer.Stop();
            }
            //If networked you need to stop the network timer
            if (CommunicationManager.Instance.IsNetworked)
            {
                CommunicationManager.Instance.Stop();
            }
            allowKeyDown = false;
        }

        /// <summary>
        /// Ends the game
        /// </summary>
        /// <param name="disconnect">If connected to server autodisconnect</param>
        public void EndGame(bool disconnect = true)
        {
            timersEnabled = false;
            StopTimers();
            keyboardInputTimer.Tick -= KeyboardInputTimerTick;
            if (CommunicationManager.Instance.IsNetworked && disconnect)
            {
                MessageManager.Instance.MessageHandler -= HandleMessage;
                CommunicationManager.Instance.Disconnect();
            }

            //Clear all items to prevent memory leak
            level = null;
            messageInstance = null;
            keyboardInputTimer = null;
            rotationTimer = null;
            cvsPlayArea.Children.Clear();
            cvsExitArea.Children.Clear();
            GC.Collect();
        }

        private bool timersEnabled = false;

        public void StartTimers()
        {
            timersEnabled = true;
            keyboardInputTimer.Start();
            rotationTimer.Start();
            //If networked you need to start the network timer
            if (CommunicationManager.Instance.IsNetworked)
            {
                CommunicationManager.Instance.Start();
            }

            if (gameType == GameType.Singleplayer)
            {
                aiTimer.Start();
            }
            allowKeyDown = true;
        }
    }
}
