using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Common.Enums;

namespace Twister.Pages
{
    /// <summary>
    /// Interaction logic for SetupGame.xaml
    /// </summary>
    public partial class SetupGame : Page
    {
        public SetupGame()
        {
            InitializeComponent();
        }

        private Level.Level levelItem = null;

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            if (TopFrameManager.Instance.MainFrame.CanGoBack)
            {
                TopFrameManager.Instance.MainFrame.GoBack();
            }
        }

        private async void BtnLevelSelect_OnClick(object sender, RoutedEventArgs e)
        {
            //Awaited so that code runs in the right order
            await BtnLevelSelect_OnClickAsync();
        }

        private async Task BtnLevelSelect_OnClickAsync()
        {
            //Get the new level selection (if it is null don't update the value)
            levelItem = await LevelSelect.GetLevelSelection() ?? levelItem;
            if (levelItem == null)
            {
                btnSinglePlayer.IsEnabled = false;
                btnLocalMultiPlayer.IsEnabled = false;
                btnLevelSelect.Content = $"Select Level";
            }
            else
            {
                //Level is ready to setup
                btnSinglePlayer.IsEnabled = true;
                btnLocalMultiPlayer.IsEnabled = true;
                btnLevelSelect.Content = $"Select Level: '{levelItem.Name}' currently selected";
            }
        }

        private void BtnSinglePlayer_OnClick(object sender, RoutedEventArgs e)
        {
            PlayLocallyCommon(EnemyType.AI);
        }

        private void BtnLocalMultiPlayer_OnClick(object sender, RoutedEventArgs e)
        {
            PlayLocallyCommon(EnemyType.Local);
        }

        private void BtnNetworked_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.MainFrame.Navigate(new ConnectPage(levelItem));
        }

        /// <summary>
        /// Combines the code for Single Player and Local Multi Player
        /// </summary>
        /// <param name="et">Enemy Type</param>
        private void PlayLocallyCommon(EnemyType et)
        {
            TopFrameManager.Instance.MainFrame.Navigate(new GamePage(ProtagonistType.Local, et, levelItem));
        }
    }
}
