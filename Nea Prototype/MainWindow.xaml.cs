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
using Nea_Prototype.Keybindings;
using Nea_Prototype.Pages;

namespace Nea_Prototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainMenu menu = new MainMenu();

        public MainWindow()
        {
            InitializeComponent();
            TopFrameManager.Instance.MainWindow = this;
            TopFrameManager.Instance.MainFrame = frmMainFrame;
            TopFrameManager.Instance.OverlayFrame = frmOverlay;
            frmMainFrame.Navigate(menu);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            (frmMainFrame.Content as IKeyboardInputs)?.Page_KeyDown(sender, e);
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            //Kill the server
            TopFrameManager.Instance.TryKillServer();
            //Try to kill connecting etc.

            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                (TopFrameManager.Instance.MainFrame.Content as GamePage).EndGame();
            }
            else if (TopFrameManager.Instance.MainFrame.Content is WaitPage)
            {
                (TopFrameManager.Instance.MainFrame.Content as WaitPage).CancelWaiting();
            }
        }
    }
}
