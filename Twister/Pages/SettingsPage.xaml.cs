using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Twister.Keybindings;

namespace Twister.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page, IKeyboardInputs
    {
        public SettingsPage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                lstKeybindingsList.ItemsSource = KeybindingsProperties.Properties;
                DataContext = KeybindingsProperties.Properties;
            };
        }
        private Key keyStored { get; set; }
        private bool awaitingKeyPress = false;

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (awaitingKeyPress)
            {
                keyStored = e.Key;
                awaitingKeyPress = false;
            }
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await ButtonClickAsync(sender, e);
        }

        private async Task ButtonClickAsync(object sender, RoutedEventArgs e)
        {
            awaitingKeyPress = true;
            while (awaitingKeyPress)
            {
                await Task.Delay(1000);
            }
            Button btnSender = sender as Button;
            KeybindingProperty keybindingToChange = btnSender?.DataContext as KeybindingProperty;
            keybindingToChange.Key = keyStored;
            switch (keybindingToChange.BindingName)
            {
                case "Player 1 Up":
                    KeyBindingsManager.Instance.Player1_up = keyStored;
                    break;
                case "Player 1 Down":
                    KeyBindingsManager.Instance.Player1_down = keyStored;
                    break;
                case "Player 1 Left":
                    KeyBindingsManager.Instance.Player1_left = keyStored;
                    break;
                case "Player 1 Right":
                    KeyBindingsManager.Instance.Player1_right = keyStored;
                    break;
                case "Player 2 Up":
                    KeyBindingsManager.Instance.Player2_up = keyStored;
                    break;
                case "Player 2 Down":
                    KeyBindingsManager.Instance.Player2_down = keyStored;
                    break;
                case "Player 2 Left":
                    KeyBindingsManager.Instance.Player2_left = keyStored;
                    break;
                case "Player 2 Right":
                    KeyBindingsManager.Instance.Player2_right = keyStored;
                    break;
                case "Debug overlay key":
                    KeyBindingsManager.Instance.DebugOverlayKey = keyStored;
                    break;
                default:
                    return;
            }
            KeyBindingsManager.SaveKeybindings(KeyBindingsManager.Instance);
            btnSender.Content = keyStored;
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            if (TopFrameManager.Instance.MainFrame.Content is SettingsPage &&
                TopFrameManager.Instance.MainFrame.CanGoBack)
            {
                TopFrameManager.Instance.MainFrame.GoBack();
            }
            else if (TopFrameManager.Instance.OverlayFrame.Content is SettingsPage &&
                     TopFrameManager.Instance.OverlayFrame.CanGoBack)
            {
                TopFrameManager.Instance.OverlayFrame.GoBack();
            }
        }
    }
}
