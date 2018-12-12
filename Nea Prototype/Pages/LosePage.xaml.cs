using System;
using System.Collections.Generic;
using System.Linq;
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
using Nea_Prototype.Grid;
using Nea_Prototype.Network;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for WinPage.xaml
    /// </summary>
    public partial class LosePage : Page, IKeyboardInputs
    {
        private Level.Level level;
        private ProtagonistType pt;
        private EnemyType et;
        private bool isNetworked;

        /// <summary>
        /// Only call this on the overlay panel
        /// </summary>
        public LosePage(Level.Level level, ProtagonistType pt, EnemyType et, bool isNetworked)
        {
            InitializeComponent();
            //Stop the game
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                (TopFrameManager.Instance.MainFrame.Content as GamePage).StopTimers();
                if (isNetworked)
                {
                    CommunicationManager.Instance.Stop();
                }
            }

            this.level = level;
            this.pt = pt;
            this.et = et;
            this.isNetworked = isNetworked;
        }

        private void BtnEnd_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.GoToMainMenu();
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //No keydown event needed
        }

        private void BtnRetry_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.GoToMainMenu();
            //Recreate the gamepage
            TopFrameManager.Instance.MainFrame.Navigate(new GamePage(pt, et, level));
        }
    }
}
