using System;
using System.Collections.Generic;
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
using Nea_Prototype.Grid;
using CheckBox = System.Windows.Controls.CheckBox;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for DebugOverlayPage.xaml
    /// </summary>
    public partial class DebugOverlayPage : Page
    {
        public DebugOverlayPage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                int count = 0;

                //Check to see if the opiton is already on and set that to the value
                WallToggleButton.IsChecked = GameGridManager.GetGameGrid().WallCollisionRectangles;
                EnemyToggleButton.IsChecked = GameGridManager.GetGameGrid().EnemyCollisionRectangles;
                foreach (Exitable exitableItem in GameGridManager.GetGameGrid().ExitLocations)
                {
                    //Place all of the exitable items in a list
                    CheckBox exitableCheckBox = new CheckBox()
                    {
                        Content = count,
                        IsTabStop = false,
                        IsChecked = exitableItem.CanExit,
                    };
                    exitableCheckBox.Click += ExitableToggleButton_OnClick;
                    pnlExitableItems.Children.Add(exitableCheckBox);
                    count++;
                }
            };
        }


        private void WallToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            //If item is checked update variable
            GameGridManager.GetGameGrid().WallCollisionRectangles = ((sender as CheckBox).IsChecked ?? false);
        }

        private void EnemyToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            //If item is checked update variables
            GameGridManager.GetGameGrid().EnemyCollisionRectangles = ((sender as CheckBox).IsChecked ?? false);
        }

        private void ExitableToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            //If item is checked update variables
            CheckBox senderCheckBox = (CheckBox) sender;
            if (int.TryParse(senderCheckBox.Content.ToString(), out var arrayIndex))
            {
                GameGridManager.GetGameGrid().ExitLocations[arrayIndex].CanExit = senderCheckBox.IsChecked ?? false;
            }
        }
           
    }
}
