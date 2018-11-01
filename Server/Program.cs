using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Algorithms;

namespace Server
{
    class Program
    {
        private TcpListener listener;
        private Thread listenThread;

        private readonly int PORT_NO = 26332;

        static void Main(string[] args)
        {
            Program p = new Program(ref args);   
        }

        private Program(ref string[] args)
        {
            if (args.Length > 0 && int.TryParse(args[0], out PORT_NO))
            {
                //Port number is set
            }
        }


        private void ServerStart()
        {
            this.listener = new TcpListener(IPAddress.Any, PORT_NO);
            //New thread
            this.listenThread = new Thread(new ThreadStart(() =>
            {
                listener.Start();
                while (true)
                {
                    Console.WriteLine("Waiting for a connection");
                    //Thread blocking call (blocks until client has connected)
                    TcpClient client = listener.AcceptTcpClient();

                    //create a new thread to handle this new client
                    Thread clientThread = new Thread(new ThreadStart(() =>
                    {
                        TcpClient threadClient = client;
                        NetworkStream clientStream = threadClient.GetStream();
                        
                        byte[] message = new byte[4096];
                        int bytesRead = 0;

                        bool mapSent = false;
                        bool mapDownloaded = false;

                        while (true)
                        {
                            bytesRead = 0;
                            try
                            {
                                Console.WriteLine($"Waiting for data from {client.ToString()}");
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
                                Console.WriteLine($"{client.ToString()} disconnected");
                                break;
                            }
                            //msg recieved
                            ASCIIEncoding encoder = new ASCIIEncoding();

                            string bufferMessage = encoder.GetString(message, 0, bytesRead);

                            int characterNo;
                            //If the map has downloaded
                            if (mapDownloaded)
                            {
                                //message logic goes here
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
                                            throw new Exception("Too many people have joined.");
                                    }

                                    ServerChecks();
                                }
                                catch (IndexOutOfRangeException e)
                                {
                                    Console.WriteLine(
                                        $"Incorrect format message received: {e.GetType().FullName} {e.Message}");
                                    continue;
                                }
                                catch (FormatException e)
                                {
                                    Console.WriteLine(
                                        $"Incorrect format message received: {e.GetType().FullName} {e.Message}");
                                    continue;
                                }

                                ServerChecks();

                                //Converts character 1 to character 2 and vice versa
                                int otherCharacterNumber = characterNo == 1 ? 2 : 1;
                                double otherCharacterX = (otherCharacterNumber == 1
                                    ? ServerDataManager.Instance.character1.CharacterPosition.x
                                    : ServerDataManager.Instance.character2.CharacterPosition.x);
                                double otherCharacterY = (otherCharacterNumber == 1
                                    ? ServerDataManager.Instance.character1.CharacterPosition.y
                                    : ServerDataManager.Instance.character2.CharacterPosition.y);

                                byte[] buffer = encoder.GetBytes($"{otherCharacterNumber},{otherCharacterX},{otherCharacterY},{ServerDataManager.Instance.CharactersCollided},{ServerDataManager.Instance.CharactersWon},{ServerDataManager.Instance.ExitsOpen.ToArray().ToString()}");
                                clientStream.Write(buffer, 0, buffer.Length);
                                clientStream.Flush();
                                continue;
                            }
                            else if(!mapSent)
                            {
                                //Send map over (would be in JSON)
                                byte[] buffer = encoder.GetBytes("Send map");
                                clientStream.Write(buffer, 0, buffer.Length);
                                clientStream.Flush();
                                continue;
                            }

                        }
                    }));
                }
            }));
            listenThread.Start();
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

            if (Collisions.EnemyCollisionDetectionCommon(char1Left,char1Top,
                                                         char2Left,char2Top))
            {

                ServerDataManager.Instance.CharactersCollided = true;
            }

            //Rotation stuff worked out here for exist being open

        }

    }
}
