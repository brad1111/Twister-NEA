﻿using System;
using System.Collections.Generic;
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
                IPAddress.TryParse(iPstring, out temp);
                return temp;
            }
        }

        private string iPstring { get; set; } = "127.0.0.1";
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

        public bool Connect(string IP, int port)
        {
            iPstring = IP;

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
                while (true)
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

    }
}