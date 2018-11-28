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

        /// <summary>
        /// Only call this on the overlay panel
        /// </summary>
        public LosePage(Level.Level level, ProtagonistType pt, EnemyType et)
        {
            InitializeComponent();
            //Stop the game
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                GameGridManager.Clear();
                GamePage gp = (GamePage) TopFrameManager.Instance.MainFrame.Content;
                gp.EndGame();
            }

            this.level = level;
            this.pt = pt;
            this.et = et;
        }

        private void BtnEnd_OnClick(object sender, RoutedEventArgs e)
        {
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
