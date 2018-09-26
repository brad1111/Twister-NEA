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
                foreach (Exitable exitableItem in GameGridManager.GetGameGrid().ExitLocations)
                {
                    pnlExitableItems.Children.Add(new CheckBox()
                    {
                        Content = count,
                        IsTabStop = false
                    });
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
    }
}
