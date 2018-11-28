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
using Common.Enums;
using Nea_Prototype.Level;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page, IKeyboardInputs
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void BtnSinglePlayer_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnMultiPlayer_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.MainFrame.Navigate(new GamePage(ProtagonistType.Local, EnemyType.Local, LevelIO.ReadJSON("testing.json")));
        }

        private void BtnNetworked_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.MainFrame.Navigate(new ConnectPage());
        }

        private void BtnSettings_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.MainFrame.Navigate(new SettingsPage());
        }

        private void BtnExit_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //No keydown needed
        }
    }
}
