using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Algorithms;
using Server.Level;

namespace Server
{
    class Program
    {
        private TcpListener listener;
        private Thread listenThread;
        private Level.Level level;

        private readonly int PORT_NO = 26332;
        private readonly string levelLocation = String.Empty;

        private Stack<TcpClient> ClientsStack = new Stack<TcpClient>();

        static void Main(string[] args)
        {
            Program p = new Program(ref args);
            p.ServerStart();
        }

        private Program(ref string[] args)
        {
            if (args.Length > 0 && int.TryParse(args[0], out PORT_NO))
            {
                //Port number is set
            }
            else
            {
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
                Console.WriteLine("Can't load level file.");
                Thread.Sleep(1000);
                Environment.Exit(1);
            }
        }

        private void SetupExits()
        {
            for (int i = 0; i < level.InternalExits.Length; i++)
            {
                ExitingManager.Instance.FindAnglesNeededToOpenInternal(
                    level.ExitLocation.HeightFromAnchor,
                    level.ExitLocation.HeightFromAnchor + level.ExitLocation.Length,
                    int.Parse(level.InternalExits[i].CanvasPos.y.ToString()),
                    int.Parse(level.InternalExits[i].CanvasPos.y.ToString()) + Constants.GRID_ITEM_WIDTH);
            }
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        private void ServerStart()
        {
            level = LevelIO.ReadJSON(levelLocation);
            level.SetupLevel();
            SetupExits();
            this.listener = new TcpListener(IPAddress.Any, PORT_NO);
            //New thread
            this.listenThread = new Thread(new ThreadStart(ClientConnection));
            listenThread.Start();
        }
        /// <summary>
        /// Goes through and checks for new clients to connect
        /// </summary>
        private void ClientConnection()
        {
            listener.Start();
            while (true)
            {
                Console.WriteLine("Waiting for a connection");
                //Thread blocking call (blocks until client has connected)
                ClientsStack.Push(listener.AcceptTcpClient());

                //create a new thread to handle this new client
                Thread clientThread = new Thread(new ThreadStart(ClientCommunication));
                clientThread.Start();
            }
        }

        /// <summary>
        /// Sends/receives messages to/from the client.
        /// </summary>
        private void ClientCommunication()
        {
            TcpClient threadClient = ClientsStack.Peek();
            NetworkStream clientStream = threadClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead = 0;

            bool mapSent = false;
            bool mapDownloaded = false;
            bool gameStartedOnThread = false;

            while (true)
            {
                bytesRead = 0;
                try
                {
                    Console.WriteLine($"Waiting for data from {threadClient.ToString()}");
                    //blocks thread until client sends message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch (SocketException e)
                {
                    //Socket Error has occured
                    Console.WriteLine(e);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                if (bytesRead == 0)
                {
                    //client disconnected
                    Console.WriteLine($"{threadClient.ToString()} disconnected");
                    break;
                }

                //msg recieved
                ASCIIEncoding encoder = new ASCIIEncoding();

                string bufferMessage = encoder.GetString(message, 0, bytesRead);
                Console.WriteLine(bufferMessage);

                int characterNo;
                //If the map has downloaded
                if (gameStartedOnThread)
                {
                    string[] messageSections = bufferMessage.Split(',');
                    try
                    {
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

                        ServerChecks();
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

                    ServerChecks();

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

                    byte[] buffer =
                        encoder.GetBytes(
                            $"{otherCharacterNumber},{otherCharacterX},{otherCharacterY},{ServerDataManager.Instance.CharactersCollided},{ServerDataManager.Instance.CharactersWon},{exits}");
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                    continue;
                }
                else if (!mapSent)
                {
                    //Send map over (would be in JSON)
                    byte[] buffer = encoder.GetBytes(ServerDataManager.Instance.levelJson);
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                    mapSent = true;
                    continue;
                }
                else if (mapSent && !mapDownloaded)
                {
                    //Check to see if the client says they have recieved it
                    if (bufferMessage == "received")
                    {
                        //If they have received the map then map is downloaded
                        mapDownloaded = true;
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
                    byte[] buffer = encoder.GetBytes("start");
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                }
            }
        }


        /// <summary>
        /// The checks that the server does to send back to the client (enemy collision detection, doors)
        /// </summary>
        private void ServerChecks()
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

            //Rotation stuff worked out here for exist being open

            //for (int i = 0; i < level.InternalExits.Length; i++)
            //{

            //}
        }
    }
}