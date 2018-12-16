using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Twister.Pages
{
    /// <summary>
    /// Interaction logic for PauseMenuPage.xaml
    /// </summary>
    public partial class PauseMenuPage : Page, IKeyboardInputs
    {
        public PauseMenuPage()
        {
            InitializeComponent();
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //Keyboard input not needed
        }

        private void BtnResume_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.ClearOverlayFrame();
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                GamePage page = (GamePage) TopFrameManager.Instance.MainFrame.Content;
                page.StartTimers();
            }
        }

        private void BtnSettings_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.OverlayFrame.Navigate(new SettingsPage());
        }

        private void BtnExit_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.GoToMainMenu();
        }
    }
}
