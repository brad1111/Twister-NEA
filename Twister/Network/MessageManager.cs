using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Twister.Network
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

        public static MessageManager Instance { get; } = new MessageManager();

        public event EventHandler MessageHandler;

        //Gets whether the client is connected, if the client is null then it is not connected
        public bool IsConnected => serverClient?.Connected ?? false;

        /// <summary>
        /// Connects to the server of IP, port
        /// </summary>
        /// <param name="IP">The IP address of the server</param>
        /// <param name="port">The port to connect on</param>
        /// <returns>Whether the connection was successful</returns>
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
                //Attempt to connect to the server
                serverClient = new TcpClient();
                
                serverClient.Connect(IP, port);
            }
            catch (SocketException e)
            {
                //SocketExceptions are expected if the server is not available on that port so just inform the user
                MessageBox.Show(e.ToString(), "Error");
            }
            catch (Exception e)
            {
                //All other exceptions are unexpected so disconnect from the server
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
                    TopFrameManager.Instance.MainFrame.Dispatcher.Invoke(new Action(
                        () => TopFrameManager.Instance.GoToMainMenu()));
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

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendMessage(string message)
        {
            //If the server is no longer connected tell the Communication Manager that it should stop sending sendmessage requests and disconnect
            if (!IsConnected)
            {
                CommunicationManager.Instance.Disconnect();
                return;
            }
            ASCIIEncoding encoder = new ASCIIEncoding();
            NetworkStream serverStream = null;
            try
            {
                //Attempt to send the message
                serverStream = serverClient.GetStream();
                byte[] buffer = encoder.GetBytes(message);
                serverStream.Write(buffer, 0, buffer.Length);
            }
            catch(IOException ex)
            {
                //There was an error reading/writing to the stream (usually occurs when the server has disconnected)
                MessageBox.Show(ex.ToString(), "Error");
            }
            catch (InvalidOperationException ex)
            {
                //There was an error performing something, can also occur on a disconnect
                MessageBox.Show(ex.ToString(), "Error");
            }
            catch (Exception ex)
            {
                //Unexpected error occured
                MessageBox.Show(ex.ToString(), "Error");
                throw;
            }
            finally
            {
                //Make sure we always flush the stream if its there
                serverStream?.Flush();
            }

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
            serverClient?.Close();
            serverClient = null;
            messages.Clear();
        }

        public List<string> GetLocalIPs()
        {

            List<IPAddress> ips = Dns.GetHostAddresses(Dns.GetHostName()).ToList();
            List<IPAddress> unneeded = ips.FindAll(x => IPAddress.IsLoopback(x));
            ips.RemoveAll(x => unneeded.Contains(x));

            List<string> ipStrings = new List<string>(ips.Count);
            foreach (IPAddress ip in ips)
            {
                ipStrings.Add(ip.ToString());
            }
            return ipStrings;
        }
    }
}