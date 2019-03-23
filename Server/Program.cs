using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common;
using Common.Algorithms;
using Common.Grid;
using Newtonsoft.Json;
using Server.Level;

namespace Server
{
    class Program
    {
        private TcpListener listener; //The threads and listeners
        private Thread listenThread;
        private Level.Level level;

        private readonly int PORT_NO = 26332; //Default port
        private readonly string levelLocation = String.Empty;

        private ASCIIEncoding encoder = new ASCIIEncoding();

        private Stack<TcpClient> ClientsStack = new Stack<TcpClient>(2); //Stores each TCP client

        private readonly System.Timers.Timer rotationTimer = new System.Timers.Timer()
        {
            Interval = 250 //Fire every 1/4 second
        };

        static void Main(string[] args)
        {
            //Start the program
            Program p = new Program(ref args);
            p.ServerStart();
        }

        /// <summary>
        /// The setup section of the program
        /// </summary>
        /// <param name="args">The arguments given to the app</param>
        private Program(ref string[] args)
        {

            if (args.Length > 0 && int.TryParse(args[0], out PORT_NO))
            {
                //Port number is set
            }
            else
            {
                //Error msg
                Console.WriteLine("Can't load level file");
                Thread.Sleep(1000);
                Environment.Exit(1);
            }

            if (args.Length >= 2 && File.Exists(args[1]))
            {
                //Level parameter can be set
                levelLocation = args[1];
            }
            else
            {
                //Error msg
                Console.WriteLine("Can't load level file.");
                Thread.Sleep(1000);
                Environment.Exit(1);
            }

            if (!(args.Length >= 4 && double.TryParse(args[2], out protagonistWeight)
                                 && double.TryParse(args[3], out enemyWeight)))
            {
                //We need to know the chracter weight
                Console.WriteLine("Needs the weights for the characters");
                Thread.Sleep(1000);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Sets up the exits
        /// </summary>
        private void SetupExits()
        {
            for (int i = 0; i < level.InternalExits.Length; i++)
            {
                //Find the angle needed to open/close the exit
                ExitingManager.Instance.FindAnglesNeededToOpenInternal(
                    level.ExitLocation.HeightFromAnchor,
                    level.ExitLocation.HeightFromAnchor + level.ExitLocation.Length,
                    int.Parse(level.InternalExits[i].CanvasPos.y.ToString()),
                    int.Parse(level.InternalExits[i].CanvasPos.y.ToString()) + Constants.GRID_ITEM_WIDTH);
                //Store it
                ServerDataManager.Instance.ExitsOpen.Add(false);
            }
        }

        /// <summary>
        /// Sets up the rotation timer and includes all of the code to check each
        /// time the rotation updates whether the doors are open
        /// </summary>
        private void SetupRotationTimer()
        {
            rotationTimer.Elapsed += RotationTimer_Tick;
        }

        private double protagonistWeight = 1, enemyWeight = 1; //Default weights

        /// <summary>
        /// Every 1/4 second we check for the new angle of the maze to determine wheter to open/close exits
        /// </summary>
        /// <param name="s">The sender object (unused)</param>
        /// <param name="e">The arguments (used)</param>
        private void RotationTimer_Tick(object s, EventArgs e)
        {
            Console.WriteLine("Angle = {0} degrees", ServerDataManager.Instance.currentAngle);
            //Check for updates in rotation and hence exits openings
            double[] charactersXPositions = {ServerDataManager.Instance.character1.CharacterPosition.x, ServerDataManager.Instance.character2.CharacterPosition.x};
            Position[] charactersPositions = {ServerDataManager.Instance.character1.CharacterPosition, ServerDataManager.Instance.character2.CharacterPosition};
            double[] charactersWeights = {protagonistWeight, enemyWeight};
            int multiplier = Rotation.RotationMultiplier(charactersXPositions, charactersWeights);

            double angleDelta = Rotation.AbsAngleDelta(charactersPositions, 0.25, charactersWeights);

            double newAngle = ServerDataManager.Instance.currentAngle + angleDelta * multiplier;

            //If the angle is too large or small set it to the max/min value respectively
            if(newAngle < -90)
            {
                newAngle = -90;
            }
            else if (newAngle > 90)
            {
                newAngle = 90;
            }


            //Check for exit opening/closing
            if (multiplier == 0)
            {
                return;
                //Dont bother if it isn't rotating
            }

            if (multiplier > 0)
            {
                //Positive rotation
                for (int i = 0; i < ExitingManager.Instance.AnglesToOpen.Count; i++)
                {
                    if (ExitingManager.Instance.AnglesToOpen[i] < ServerDataManager.Instance.currentAngle)
                    {
                        ServerDataManager.Instance.ExitsOpen[i] = true;
                    }

                    if (ExitingManager.Instance.AnglesToClose[i] < ServerDataManager.Instance.currentAngle)
                    {
                        ServerDataManager.Instance.ExitsOpen[i] = false;
                    }
                }
            }
            else /*if (rotationMultiplier > 0)*/
            {
                //Negative rotation
                for (int i = 0; i < ExitingManager.Instance.AnglesToOpen.Count; i++)
                {
                    if (ExitingManager.Instance.AnglesToClose[i] > newAngle)
                    {
                        ServerDataManager.Instance.ExitsOpen[i] = true;
                    }

                    if (ExitingManager.Instance.AnglesToOpen[i] > ServerDataManager.Instance.currentAngle)
                    {
                        ServerDataManager.Instance.ExitsOpen[i] = false;
                    }
                }
            }

            ServerDataManager.Instance.currentAngle = newAngle;
            //Update currentangle
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        private void ServerStart()
        {
            level = LevelIO.ReadJSON(levelLocation);
            level.SetupLevel(protagonistWeight, enemyWeight);
            //Update levelLocation to include weights
            ServerDataManager.Instance.levelJson = JsonConvert.SerializeObject(level);
            SetupExits();
            SetupRotationTimer();
            ServerDataManager.Instance.Level = level;
            this.listener = new TcpListener(IPAddress.Any, PORT_NO);
            //Run Client listining on a different thread so that it does not block the main thread
            this.listenThread = new Thread(new ThreadStart(ClientConnection));
            listenThread.Start();
        }
        /// <summary>
        /// Goes through and checks for new clients to connect
        /// </summary>
        private void ClientConnection()
        {
            listener.Start();
            //Wait until the stack is full 
            while (ClientsStack.Count < 2)
            {
                Console.WriteLine("Waiting for a connection");
                //Thread blocking call (blocks until client has connected)
                ClientsStack.Push(listener.AcceptTcpClient());

                //create a new thread to handle this new client
                Thread clientThread = new Thread(new ThreadStart(ClientCommunication));
                clientThread.Start();
            }

            //Then wait until all clients have left the stack
            while (ClientsStack.Count > 0)
            {
                Thread.Sleep(1000);
            }
            //Then close
        }

        /// <summary>
        /// Sends/receives messages to/from the client.
        /// </summary>
        private void ClientCommunication()
        {
            //Find out variables and setup default ones
            TcpClient threadClient = ClientsStack.Peek();
            NetworkStream clientStream = threadClient.GetStream();
            int debuggingCharacterNo = ClientsStack.Count;

            byte[] message = new byte[4096];
            int bytesRead = 0;

            bool mapSent = false;
            bool mapDownloaded = false;
            bool gameStartedOnThread = false;
            bool clientConnected = true;
            bool clientReady = false;
            bool restartingGame = false;

            while (clientConnected)
            {
                //As long as the client is still connected still wait for data
                bytesRead = 0;
                try
                {
                    Console.WriteLine($"Waiting for data from client {debuggingCharacterNo}");
                    //If the client isn't waiting block the thread until the client sends a message
                    if (gameStartedOnThread || !mapDownloaded || restartingGame)
                    {
                        bytesRead = clientStream.Read(message, 0, 4096);
                    }
                }
                catch (SocketException e)
                {
                    //Socket Error has occured
                    Console.WriteLine(e);
                    break;
                }
                catch (IOException e)
                {
                    //Client has possibly disconnected so finish the thread.
                    Console.WriteLine(e);
                    clientConnected = false;
                    ServerDataManager.Instance.CharacterCrashed();
                    threadClient.Dispose();
                    break;
                }
                catch (Exception e)
                {
                    //General error, tell the user
                    Console.WriteLine(e);
                    throw;
                }

                //If the client hasn't sent anything and it's not because its waiting then close the socket
                if (bytesRead == 0 && gameStartedOnThread && !clientReady)
                {
                    //Client has disconnected so finish the thread.
                    Console.WriteLine("Character has left");
                    clientConnected = false;
                    ServerDataManager.Instance.CharacterLeft();
                    threadClient.Dispose();
                    break;
                }

                //msg recieved

                string bufferMessage = encoder.GetString(message, 0, bytesRead);
                Console.WriteLine(bufferMessage);

                int characterNo;
                //If the map has downloaded
                if (ServerDataManager.Instance.GameOver)
                {
                    if (ServerDataManager.Instance.ClientCrashed)
                    {
                        SendMessage("crash", ref clientStream);
                        threadClient.Dispose();
                        ClientsStack.Clear();
                        break;
                    }
                    else if(ServerDataManager.Instance.ClientLeft)
                    {
                        SendMessage("close", ref clientStream);
                        threadClient.Dispose();
                        ClientsStack.Clear();
                        break;
                    }
                }
                
                if (gameStartedOnThread)
                {

                    string[] messageSections = bufferMessage.Split(',');
                    try
                    {
                        //Converts decodes the data from the message string and stores the data 
                        characterNo = int.Parse(messageSections[0]);
                        double xCoord = double.Parse(messageSections[1]);
                        double yCoord = double.Parse(messageSections[2]);

                        switch (characterNo)
                        {
                            case 1:
                                ServerDataManager.Instance.character1.CharacterPosition.x = xCoord;
                                ServerDataManager.Instance.character1.CharacterPosition.y = yCoord;
                                break;
                            case 2:
                                ServerDataManager.Instance.character2.CharacterPosition.x = xCoord;
                                ServerDataManager.Instance.character2.CharacterPosition.y = yCoord;
                                break;
                            default:
                                throw new IndexOutOfRangeException(
                                    "Too many people have joined, or error in transmission");
                        }

                        CollisionDetectionChecks();
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine($"Incorrect format message received: {e.GetType().FullName} {e.Message}");
                        continue;
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine($"Incorrect format message received: {e.GetType().FullName} {e.Message}");
                        continue;
                    }

                    CollisionDetectionChecks();


                    //-----------Section to send message back --------
                    if (ServerDataManager.Instance.CharactersCollided)
                    {
                        SendMessage("collided", ref clientStream);
                        clientReady = false;
                        gameStartedOnThread = false;
                        restartingGame = true;
                        mapDownloaded = false; //We need this for the character to ready up
                        ServerDataManager.Instance.ResetGame();
                    }
                    else if (ServerDataManager.Instance.CharactersWon)
                    {
                        SendMessage("won", ref clientStream);
                        clientReady = false;
                        gameStartedOnThread = false;
                        mapDownloaded = false;
                        ServerDataManager.Instance.ResetGame();
                    }

                    //Converts character 1 to character 2 and vice versa
                    int otherCharacterNumber = characterNo == 1 ? 2 : 1;
                    double otherCharacterX = otherCharacterNumber == 1
                        ? ServerDataManager.Instance.character1.CharacterPosition.x
                        : ServerDataManager.Instance.character2.CharacterPosition.x;
                    double otherCharacterY = otherCharacterNumber == 1
                        ? ServerDataManager.Instance.character1.CharacterPosition.y
                        : ServerDataManager.Instance.character2.CharacterPosition.y;
                    //Convert exit array into string
                    string exits = String.Empty;
                    foreach (var openExit in ServerDataManager.Instance.ExitsOpen)
                    {
                        exits += openExit.ToString() + ",";
                    }

                    SendMessage($"{otherCharacterNumber},{otherCharacterX},{otherCharacterY},{exits}",
                                ref clientStream);
                    continue;
                }
                else if (!mapSent)
                {
                    //Send map over (in JSON)
                    SendMessage(ServerDataManager.Instance.levelJson, ref clientStream);
                    mapSent = true;
                    continue;
                }
                else if (mapSent && !mapDownloaded)
                {
                    //Check to see if the client says they have received it
                    if (bufferMessage == "received")
                    {
                        //If they have received the map then map is downloaded
                        mapDownloaded = true;
                        clientReady = true;
                        ServerDataManager.Instance.CharacterReady();
                    }
                    else if (bufferMessage == "resend")
                    {
                        //The have failed to receive the message send again.
                        mapSent = false;
                    }
                }
                else if (mapSent && mapDownloaded && ServerDataManager.Instance.GameStarted)
                {
                    //If the game has started overall, tell the client and start the game on this thread
                    gameStartedOnThread = true;
                    if (!rotationTimer.Enabled)
                    {
                        rotationTimer.Start();
                    }
                    SendMessage("start", ref clientStream);
                    Console.WriteLine($"---------Game started on {debuggingCharacterNo}---------");
                }
            }
        }


        /// <summary>
        /// The checks that the server does to send back to the client (enemy collision detection)
        /// </summary>
        private void CollisionDetectionChecks()
        {
            //Enemy collision detection
            double char1Left = ServerDataManager.Instance.character1.CharacterPosition.x;
            double char1Top = ServerDataManager.Instance.character1.CharacterPosition.y;

            double char2Left = ServerDataManager.Instance.character2.CharacterPosition.x;
            double char2Top = ServerDataManager.Instance.character2.CharacterPosition.y;

            if (Collisions.EnemyCollisionDetectionCommon(char1Left, char1Top,
                char2Left, char2Top))
            {
                ServerDataManager.Instance.CharactersCollided = true;
            }   

            //Exit collision detection
            if(char1Left < 0 || char1Left > Constants.GRID_WIDTH ||
               char1Top < 0 || char1Top > Constants.GRID_WIDTH)
            {
                ServerDataManager.Instance.CharactersWon = true;
            }
        }

        /// <summary>
        /// Sends a message over the network to the client
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="clientStream">The stream for the message to be sent on</param>
        private void SendMessage(string message, ref NetworkStream clientStream)
        {
            byte[] buffer = encoder.GetBytes(message);
            clientStream.Write(buffer, 0, buffer.Length);
        }
    }
}