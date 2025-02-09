﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Enums;
using Newtonsoft.Json;
using Twister.Network;

namespace Twister.Pages
{
    /// <summary>
    /// Interaction logic for ConnectPage.xaml
    /// </summary>
    public partial class ConnectPage : Page, IKeyboardInputs
    {
        string IPRegex =
                /*IPV4*/
                @"((([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]))";
               // /*domain name*/ "(([a-zA-Z0-9].)*([a-zA-Z0-9]))";
        private string PortRegex = @"^([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$";

        private Level.Level inputtedLevelFile = null;
        private Level.Level levelFile = null;
        
        public ConnectPage(Level.Level levelFileGiven)
        {
            InitializeComponent();

            
            txtIP.RegularExpression = IPRegex;
            txtPort.RegularExpression = PortRegex;
            inputtedLevelFile = levelFileGiven;
            if (inputtedLevelFile is null)
            {
                btnCreateServer.IsEnabled = false;
                btnCreateServer.Content = "Host (No level selected)";
            }
        }

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            //Show loading bar
            prgLoading.IsEnabled = true;
            prgLoading.Visibility = Visibility.Visible;
            btnBack.IsEnabled = false;
            //Setup networking
            int portNo = 0;
            
            if(!int.TryParse(txtPort.Text, out portNo) && portNo < 65536)
            {
                //Invalid
                MessageBox.Show("Port is too great (>65535).");
                HideProgressBar();
                return;
            }

            if (!MessageManager.Instance.Connect(txtIP.Text, portNo))
            {
                //If it hasn't started tell the user
                MessageBox.Show("Unable to connect to server (either incorrect IP or connection didn't go through).", "Error");
                HideProgressBar();
            }
            else
            {
                //Wait for the map to be downloaded
                //Connect to the listener and wait for the map to be downloaded
                MessageManager.Instance.MessageHandler += HandleMessage;
                //Threading used to stop the UI thread from being blocked whilst waiting
                //for the map to be sent
                Thread waitThread = new Thread(new ThreadStart(() =>
                {
                    while (levelFile is null)
                    {
                        //Wait until level file is populated
                        MessageManager.Instance.SendMessage("send");
                        //Make sure the client is not rude by sending out too many messages
                        Thread.Sleep(1000);
                    }
                    //Unsubscribe this message handler
                    MessageManager.Instance.MessageHandler -= HandleMessage;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        GamePage gp = new GamePage(pt: ProtagonistType.Remote, et: EnemyType.Local, _level: levelFile);
                        TopFrameManager.Instance.MainFrame.Navigate(gp);
                        //Hide loading bar
                        HideProgressBar();
                    }));
                }));
                waitThread.Start();
            }
        }

        public void HandleMessage(object sender, EventArgs e)
        {
            if (e != null && e is MessageEventArgs)
            {
                string receivedMessage = ((MessageEventArgs) e).Message;
                try
                {
                    Level.Level receivedMessageObj = JsonConvert.DeserializeObject<Level.Level>(receivedMessage);
                    levelFile = receivedMessageObj;
                    MessageManager.Instance.SendMessage("received");
                }
                catch (JsonException)
                {
                    MessageManager.Instance.SendMessage("resend");
                }
            }
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //Do nothing
        }

        private void BtnCreateServer_OnClick(object sender, RoutedEventArgs e)
        {
            //Show loading bar
            prgLoading.IsEnabled = true;
            prgLoading.Visibility = Visibility.Visible;
            btnBack.IsEnabled = false;

            if (!File.Exists("server.exe"))
            {
                //If server is not found then stop and tell the user
                MessageBox.Show("Could not find server.exe", "Error");
            }

            bool portValid = false;
            int portNo;

            //Check if the port is valid
            if(int.TryParse(txtPort.Text, out portNo) && portNo < 65536)
            {
                portValid = true;
            }

            //The server is in a separate process to make the client/server model simpler, but also allows the user
            //still to host their own server easily
            TopFrameManager.Instance.ServerProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "server.exe",
                    Arguments = String.Format("{0} {1}.level {2} {3}", portValid ? portNo : 26332, inputtedLevelFile.Name,
                                            inputtedLevelFile.characterWeights[0], inputtedLevelFile.characterWeights[1]),
                    WindowStyle = ProcessWindowStyle.Minimized
                }
            };
            TopFrameManager.Instance.ServerProcess.Start();


            int attempts = 0;
            //Attempt to connect, but wait a few seconds to see if it does connect
            //Do this without blocking the UI thread
            Thread connectThread = new Thread(new ThreadStart(() =>
            {
                while (!MessageManager.Instance.IsConnected)
                {
                    attempts++;
                    //Attempt to connect
                    try
                    {
                        MessageManager.Instance.Connect("127.0.0.1", 26332);
                    }
                    catch (Exception ex)
                    {
                        HideProgressBar();
                        Console.WriteLine(ex);
                        throw;
                    }
                    
                    Thread.Sleep(1000);
                    //Don't attempt more than 5 times
                    if (attempts >= 5)
                    {
                        return;
                    }
                }

                //Setup handlemessage so the map gets downloaded
                MessageManager.Instance.MessageHandler += HandleMessage;
                //Wait for map to be downloaded
                while (levelFile is null)
                {
                    MessageManager.Instance.SendMessage("send");
                    Thread.Sleep(1000);
                }
                //Remove handlemessage so that map requests are no longer sent to the server
                MessageManager.Instance.MessageHandler -= HandleMessage;

                //Start the waiting page for the game
                //Dispatcher is needed since the controls on the page need to be initialised on the UI thread.
                Dispatcher.Invoke(new Action(() =>
                {
                    WaitPage wp = new WaitPage(pt:ProtagonistType.Local, et:EnemyType.Remote, level: levelFile);
                    TopFrameManager.Instance.MainFrame.Navigate(wp);
                    //Hide loading bar
                    HideProgressBar();
                }));
            }));
            connectThread.Start();
        }

        /// <summary>
        /// Hides the loading bar
        /// </summary>
        private void HideProgressBar()
        {
            //Hide loading bar
            prgLoading.IsEnabled = false;
            prgLoading.Visibility = Visibility.Hidden;
            btnBack.IsEnabled = true;
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.GoToMainMenu();
        }
    }
}
