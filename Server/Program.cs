using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private TcpListener listener;
        private Thread listenThread;

        private const int PORT_NO = 26332;

        static void Main(string[] args)
        {
            Program p = new Program();
            
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

                            //message logic goes here
                        }
                    }));
                }
            }));
            listenThread.Start();
        }


    }
}
