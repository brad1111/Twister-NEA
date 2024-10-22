﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Enums;
using Twister.Network;

namespace Twister.Pages
{
    /// <summary>
    /// Interaction logic for WaitPage.xaml
    /// </summary>
    public partial class WaitPage : Page, IKeyboardInputs
    {
        private readonly ProtagonistType pt;
        private readonly EnemyType et;
        private readonly Level.Level level; 

        private bool gameStarted = false;
        private Thread waitingThread = null;

        public WaitPage(ProtagonistType pt, EnemyType et, Level.Level level)
        {
            InitializeComponent();
            this.pt = pt;
            this.et = et;
            this.level = level;
            foreach (var ip in MessageManager.Instance.GetLocalIPs())
            {
                txtLocalIps.Text += ip + "\n";
            }

            MessageManager.Instance.MessageHandler += StartHandler;
            Loaded += (s, e) =>
            {
                //Create a thread to waiting until a start message has been received without blocking
                //the ui thread
                waitingThread = new Thread(new ThreadStart(() =>
                {
                    //Wait until the start message has been received
                    while (!gameStarted)
                    {
                        Thread.Sleep(1000);
                    }
                    //Start the game
                    MessageManager.Instance.MessageHandler -= StartHandler;
                    TopFrameManager.Instance.MainFrame.Dispatcher.Invoke(new Action(() =>
                    {
                        GamePage gp = new GamePage(pt, et, level);
                        TopFrameManager.Instance.MainFrame.Navigate(gp);
                    }));
                }));
                waitingThread.Start();
            };
        }

        /// <summary>
        /// Handles the game start event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StartHandler(object sender, EventArgs e)
        {
            //Checks to see if the event is of the correct type and has the correct message.
            //If it does then the gameStarted variable becomes true
            if (e is MessageEventArgs)
            {
                MessageEventArgs eventArgs = (MessageEventArgs) e;
                string message = eventArgs.Message;
                if (message == "start")
                {
                    gameStarted = true;
                }
            }
        }

        /// <summary>
        /// Stops the client from waiting for the game to start
        /// </summary>
        public void CancelWaiting()
        {
            //Kill the waiting thread
            waitingThread?.Abort();
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //No key events needed
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            //So that the handler can be disabled
            gameStarted = true;
            CancelWaiting();
            gameStarted = false;
            TopFrameManager.Instance.TryCloseServer();
            if (TopFrameManager.Instance.MainFrame.CanGoBack)
            {
                TopFrameManager.Instance.MainFrame.GoBack();
            }
        }
    }
}
