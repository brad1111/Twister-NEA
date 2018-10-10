using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Nea_Prototype.Keybindings;

namespace Nea_Prototype.Pages
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
                    KeyBindingsManager.KeyBindings.Player1_up = keyStored;
                    break;
                case "Player 1 Down":
                    KeyBindingsManager.KeyBindings.Player1_down = keyStored;
                    break;
                case "Player 1 Left":
                    KeyBindingsManager.KeyBindings.Player1_left = keyStored;
                    break;
                case "Player 1 Right":
                    KeyBindingsManager.KeyBindings.Player1_right = keyStored;
                    break;
                case "Player 2 Up":
                    KeyBindingsManager.KeyBindings.Player2_up = keyStored;
                    break;
                case "Player 2 Down":
                    KeyBindingsManager.KeyBindings.Player2_down = keyStored;
                    break;
                case "Player 2 Left":
                    KeyBindingsManager.KeyBindings.Player2_left = keyStored;
                    break;
                case "Player 2 Right":
                    KeyBindingsManager.KeyBindings.Player2_right = keyStored;
                    break;
                case "Debug overlay key":
                    KeyBindingsManager.KeyBindings.DebugOverlayKey = keyStored;
                    break;
                default:
                    return;
            }
            KeyBindingsManager.SaveKeybindings(KeyBindingsManager.KeyBindings);
            btnSender.Content = keyStored;
        }
    }
}
