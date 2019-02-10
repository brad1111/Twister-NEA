using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Twister.Pages
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page, IKeyboardInputs
    {
        private Level.Level thisLevel = null;
        public MainMenu()
        {
            InitializeComponent();
        }

        //private void BtnSinglePlayer_OnClick(object sender, RoutedEventArgs e)
        //{
        //    TopFrameManager.Instance.MainFrame.Navigate(new GamePage(ProtagonistType.Local, EnemyType.AI,
        //        LevelIO.ReadJSON("testing.json")));
        //}

        //private void BtnMultiPlayer_OnClick(object sender, RoutedEventArgs e)
        //{
        //    TopFrameManager.Instance.MainFrame.Navigate(new GamePage(ProtagonistType.Local, EnemyType.Local, LevelIO.ReadJSON("testing.json")));
        //}

        private void BtnNetworked_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.MainFrame.Navigate(new ConnectPage(null));
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.MainFrame.Navigate(new SetupGame());
        }

        private void BtnSettings_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.MainFrame.Navigate(new SettingsPage());
        }

        private void BtnExit_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.CloseGame();
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //No keydown needed
        }

        private void BtnHelp_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.MainFrame.Navigate(new AboutPage());
        }

    }
}
