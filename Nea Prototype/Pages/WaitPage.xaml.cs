using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using Common.Enums;
using Nea_Prototype.Network;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for WaitPage.xaml
    /// </summary>
    public partial class WaitPage : Page, IKeyboardInputs
    {
        private readonly ProtagonistType pt;
        private readonly EnemyType et;
        private readonly Level.Level level; 
        private DispatcherTimer _waitingTimer = new DispatcherTimer()
        {
            Interval = new TimeSpan(0,0,1)
        };

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
            _waitingTimer.Tick += (s, e) =>
            {
                MessageManager.Instance.SendMessage("Waiting");
            };
            _waitingTimer.Start();
        }

        public void StartHandler(object sender, EventArgs e)
        {
            if (e is MessageEventArgs)
            {
                MessageEventArgs eventArgs = (MessageEventArgs) e;
                string message = eventArgs.Message;
                if (message == "start")
                {
                    _waitingTimer.Stop();
                    MessageManager.Instance.MessageHandler -= StartHandler;
                    TopFrameManager.Instance.MainFrame.Dispatcher.Invoke(new Action(() =>
                    {
                        GamePage gp = new GamePage(pt, et, level);
                        TopFrameManager.Instance.MainFrame.Navigate(gp);
                    }));
                }
                
            }
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //No key events needed
        }
    }
}
