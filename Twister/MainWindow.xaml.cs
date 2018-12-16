using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Twister.Pages;

namespace Twister
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

        private void FrmMainFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is MainMenu)
            {
                //Going back to the main menu should try to close the server.
                TopFrameManager.Instance.TryCloseServer();
            }
        }
    }
}
