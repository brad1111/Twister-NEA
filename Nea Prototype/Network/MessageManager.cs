using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nea_Prototype.Network
{
    public class MessageManager
    {
        private TcpClient serverClient = null;

        public IPAddress IP
        {
            get
            {
                IPAddress temp = null;
                IPAddress.TryParse(IPstring, out temp);
                return temp;
            }
        }

        private string IPstring { get; set; } = "127.0.0.1";
        public int Port { get; private set; } = 26332;

        private List<string> messages = new List<string>();

        /// <summary>
        /// Don't allow external instances
        /// </summary>
        private MessageManager() { }

        public readonly static MessageManager Instance = new MessageManager();

        public event EventHandler MessageHandler;

        //Gets whether the client is connected, if the client is null then it is not connected
        public bool IsConnected => serverClient?.Connected ?? false;

        public bool Connect(string IP, int port)
        {
            IPstring = IP;

            if (IP == null)
            {
                return false;
                //Not connected because IP was invalid
            }

            try
            {
                serverClient = new TcpClient();
                
                serverClient.Connect(IP, port);
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.ToString(), "Error");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error");
                serverClient.Close();
                throw;
            }


            if (IsConnected)
            {
                //Start a new AwaitMessage task
                new Task(AwaitMessages).Start();
            }

            return IsConnected;
        }

        /// <summary>
        /// Waits for messages async
        /// </summary>
        public void AwaitMessages()
        {
            NetworkStream serverStream = serverClient.GetStream();
            byte[] message = new byte[4096];
            int bytesRead;

            try
            {
                while (IsConnected)
                {
                    byte[] buffer = new byte[4096];
                    //blocking call until server sends message
                    bytesRead = serverStream.Read(message, 0, 4096);

                    ASCIIEncoding encoder = new ASCIIEncoding();
                    string receivedMessage = encoder.GetString(message, 0, bytesRead);

                    Console.WriteLine($"Received: {receivedMessage}");
                    AddMessage(receivedMessage);
                }
            }
            catch (IOException e)
            {
                if (IsConnected)
                {
                    //The server has crashed or something has happened
                    TopFrameManager.FrameManager.MainFrame.Dispatcher.Invoke(new Action(() =>
                    {

                        while (TopFrameManager.FrameManager.MainFrame.CanGoBack)
                        {
                            TopFrameManager.FrameManager.MainFrame.GoBack();
                            //Go back to the beginning.
                        }
                    }));
                    MessageBox.Show($"Server has disconnected: {e}", "Error");
                    //Close the game
                }
                //else This client has quit
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void SendMessage(string message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            NetworkStream serverStream = serverClient.GetStream();

            byte[] buffer = encoder.GetBytes(message);
            serverStream.Write(buffer, 0, buffer.Length);
            serverStream.Flush();
        }

        /// <summary>
        /// Notifies the game that it has recieved a message (about the location)
        /// </summary>
        public void NotifyMessage(string message)
        {
            EventHandler handler = MessageHandler;
            if (!(handler is null))
            {
                handler(this, new MessageEventArgs(message));
            }
        }

        /// <summary>
        /// Adds message to storage and notifies
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            messages.Add(message);
            NotifyMessage(message);
        }

        /// <summary>
        /// Clears the backend of the server, assumes event is empty
        /// </summary>
        public void ClearServer()
        {
            serverClient.Close();
            serverClient = null;
            messages.Clear();
        }
    }
}