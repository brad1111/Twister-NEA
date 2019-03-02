using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Twister.Pages
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
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                (TopFrameManager.Instance.MainFrame.Content as GamePage).StopTimers();
            }
        }

        public static bool open { get; private set; } = false;

        private void BtnContinue_OnClick(object sender, RoutedEventArgs e)
        {
            open = false;
            TopFrameManager.Instance.GoToMainMenu();
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //No keydown event needed
        }
    }
}
