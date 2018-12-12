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
                if (isNetworked)
                {
                    CommunicationManager.Instance.Stop();
                    (TopFrameManager.Instance.MainFrame.Content as GamePage).StopTimers();
                }
                else
                {
                    ClearLevel();
                }
            }

            this.level = level;
            this.pt = pt;
            this.et = et;
            this.isNetworked = isNetworked;
        }

        /// <summary>
        /// Clears the game info (Make sure that you check TFM.Instance.MF.Content is Gamepage)
        /// </summary>
        private void ClearLevel()
        {
            GameGridManager.Clear();
            GamePage gp = (GamePage) TopFrameManager.Instance.MainFrame.Content;
            gp.EndGame();
        }

        private void BtnEnd_OnClick(object sender, RoutedEventArgs e)
        {
            //If networked then close everything
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                ClearLevel();
            }


            //Clear the overlay frame
            TopFrameManager.Instance.ClearOverlayFrame();
            //Clear the main frame
            while (TopFrameManager.Instance.MainFrame.CanGoBack)
            {
                TopFrameManager.Instance.MainFrame.GoBack();
            }
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //No keydown event needed
        }

        private void BtnRetry_OnClick(object sender, RoutedEventArgs e)
        {
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                TopFrameManager.Instance.MainFrame.Content = null;
            }

            //Go back to beginning
            while (TopFrameManager.Instance.MainFrame.CanGoBack)
            {
                TopFrameManager.Instance.MainFrame.GoBack();
            }
            //Close the overlay
            TopFrameManager.Instance.ClearOverlayFrame();

            //Recreate the gamepage
            TopFrameManager.Instance.MainFrame.Navigate(new GamePage(pt, et, level));
        }
    }
}
