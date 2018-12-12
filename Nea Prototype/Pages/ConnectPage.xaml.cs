using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Enums;
using Nea_Prototype.Controls;
using Nea_Prototype.Network;
using Newtonsoft.Json;

namespace Nea_Prototype.Pages
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

        private Level.Level levelFile = null;
        
        public ConnectPage()
        {
            InitializeComponent();

            
            txtIP.RegularExpression = IPRegex;
            
        }

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            //Setup networking
            int portNo = 0;
            
            if(!int.TryParse(txtPort.Text, out portNo) && portNo < 65536)
            {
                //Invalid
                MessageBox.Show("Port is too great (>65535).");
                return;
            }

            if (!MessageManager.Instance.Connect(txtIP.Text, portNo))
            {
                //If it hasn't started tell the user
                MessageBox.Show("Unable to connect to server (either incorrect IP or connection didn't go through).", "Error");
            }
            else
            {
                //Wait for the map to be downloaded
                //Connect to the listener and wait for the map to be downloaded
                MessageManager.Instance.MessageHandler += HandleMessage;
                Thread waitThread = new Thread(new ThreadStart(() =>
                {
                    while (levelFile is null)
                    {
                        //Wait until level file is populated
                        MessageManager.Instance.SendMessage("send");
                        //Make sure the client is not rude
                        Thread.Sleep(1000);
                    }
                    //Unsubscribe this message handler
                    MessageManager.Instance.MessageHandler -= HandleMessage;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        GamePage gp = new GamePage(pt: ProtagonistType.Remote, et: EnemyType.Local, _level: levelFile);
                        TopFrameManager.Instance.MainFrame.Navigate(gp);
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
            if (!File.Exists("server.exe"))
            {
                //If server is not found then stop and tell the user
                MessageBox.Show("Could not find server.exe", "Error");
            }

            //The server is in a seperate process to make the client/server model simpler, but also allows the user
            //still to host their own server easily
            TopFrameManager.Instance.ServerProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "server.exe",
                    Arguments = "26332 testing.json",
                    WindowStyle = ProcessWindowStyle.Minimized
                }
            };
            TopFrameManager.Instance.ServerProcess.Start();

            //Wait a sec and try to connect
            Thread connectThread = new Thread(new ThreadStart(() =>
            {
                while (!MessageManager.Instance.IsConnected)
                {
                    try
                    {
                        MessageManager.Instance.Connect("127.0.0.1", 26332);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                    
                    
                    Thread.Sleep(1000);
                }
                //Setup handlemessage so the map gets downloaded
                MessageManager.Instance.MessageHandler += HandleMessage;
                //Wait for map to be downloaded
                while (levelFile is null)
                {
                    MessageManager.Instance.SendMessage("send");
                    Thread.Sleep(1000);
                }
                //Remove handlemessage so that junk is not sent to the server
                MessageManager.Instance.MessageHandler -= HandleMessage;

                //Start the waiting for the game
                Dispatcher.Invoke(new Action(() =>
                {
                    TopFrameManager.Instance.Focus();
                    WaitPage wp = new WaitPage(pt:ProtagonistType.Local, et:EnemyType.Remote, level: levelFile);
                    TopFrameManager.Instance.MainFrame.Navigate(wp);
                }));
            }));
            connectThread.Start();
        }
    }
}
