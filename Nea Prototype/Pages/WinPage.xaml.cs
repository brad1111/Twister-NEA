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
using Nea_Prototype.Grid;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for WinPage.xaml
    /// </summary>
    public partial class WinPage : Page, IKeyboardInputs
    {
        /// <summary>
        /// Only call this on the overlay panel
        /// </summary>
        public WinPage()
        {
            InitializeComponent();
            open = true;
            //Stop the game
            //if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            //{
            //    GameGridManager.Clear();
            //    GamePage gp = (GamePage) TopFrameManager.Instance.MainFrame.Content;
            //    gp.EndGame();
            //}
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                (TopFrameManager.Instance.MainFrame.Content as GamePage).StopTimers();
            }
        }

        public static bool open { get; private set; } = false;

        private void BtnContinue_OnClick(object sender, RoutedEventArgs e)
        {
            open = false;
            //End the game completely
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                GameGridManager.Clear();
                GamePage gp = (GamePage) TopFrameManager.Instance.MainFrame.Content;
                gp.EndGame();
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
    }
}
