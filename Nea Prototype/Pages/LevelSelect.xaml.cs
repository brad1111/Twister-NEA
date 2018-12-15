using System;
using System.Collections.Generic;
using System.IO;
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
using Nea_Prototype.Level;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for LevelSelect.xaml
    /// </summary>
    public partial class LevelSelect : Page
    {
        private LevelSelect()
        {
            InitializeComponent();
        }

        private static LevelSelect Instance {get;} = new LevelSelect();
        private bool goBack = false;
        private Level.Level selectedLevel = null;
        

        /// <summary>
        /// Gets the user to select a level and returns is
        /// </summary>
        /// <returns>Returns a level</returns>
        public static async Task<Level.Level> GetLevelSelection()
        {
            TopFrameManager.Instance.OverlayFrame.Dispatcher.Invoke(new Action(() =>
            {
                TopFrameManager.Instance.OverlayFrame.Navigate(Instance);
            }));
            //Clear selections etc
            Instance.goBack = false;
            Instance.selectedLevel = null;
            Instance.lstLevels.SelectedIndex = -1;
            Instance.lstLevels.ItemsSource = Instance.FindLevels();

            return await Instance.WaitForLevelTask();
        }

        private async Task<Level.Level> WaitForLevelTask()
        {
            while (Instance.goBack == false)
            {
                //wait a sec
                await Task.Delay(1000);
            }

            TopFrameManager.Instance.OverlayFrame.Dispatcher.Invoke(new Action(() =>
            {
                TopFrameManager.Instance.ClearOverlayFrame();
            }));

            return Instance.selectedLevel;
        }


        private IEnumerable<string> FindLevels()
        {

            //return Directory.GetFiles(App.AppDir, "*.level", SearchOption.TopDirectoryOnly);
            return new DirectoryInfo(App.AppDir).GetFiles("*.level",SearchOption.TopDirectoryOnly).Select(x => x.Name.Remove(x.Name.Length - 6));
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            goBack = true;
        }

        private void BtnPlay_OnClick(object sender, RoutedEventArgs e)
        {
            //Check to see if the user has selected an item
            if (lstLevels.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a level");
                return;
            }

            //Convert selected string to Level file
            string fullFileName = string.Format("{0}\\{1}.level", App.AppDir, lstLevels.SelectedItem);
            selectedLevel = LevelIO.ReadJSON(fullFileName);
            //Send back to previous page
            goBack = true;
        }
    }
}
