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
using System.Windows.Threading;
using Nea_Prototype.Algorithms;
using Nea_Prototype.Grid;
using CheckBox = System.Windows.Controls.CheckBox;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for DebugOverlayPage.xaml
    /// </summary>
    public partial class DebugOverlayPage : Page
    {
        DispatcherTimer angleTimer = new DispatcherTimer();

        public DebugOverlayPage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                int count = 0;

                //Check to see if the opiton is already on and set that to the value
                WallToggleButton.IsChecked = GameGridManager.Instance.WallCollisionRectangles;
                EnemyToggleButton.IsChecked = GameGridManager.Instance.EnemyCollisionRectangles;
                foreach (Exitable exitableItem in GameGridManager.Instance.ExitLocations)
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

                //Add all of the angles to the display
                for(int i = 0; i < ExitingManager.AnglesToOpen.Count; i++)
                {
                    Label open = new Label()
                    {
                        Content = $"{i} opens at: {ExitingManager.AnglesToOpen[i]}"
                    };
                    Label close = new Label()
                    {
                        Content = $"{i} closes at: {ExitingManager.AnglesToClose[i]}"
                    };
                    pnlAnglesPanel.Children.Add(open);
                    pnlAnglesPanel.Children.Add(close);
                }
                

                //Auto update the angle
                angleTimer.Interval = new TimeSpan(0,0,1);
                angleTimer.Tick += (se, ev) => 
                    lblAngle.Content = $"Angle: {GameGridManager.Instance.PreviousAngle}";
                angleTimer.Start();
            };
        }


        private void WallToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            //If item is checked update variable
            GameGridManager.Instance.WallCollisionRectangles = ((sender as CheckBox).IsChecked ?? false);
        }

        private void EnemyToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            //If item is checked update variables
            GameGridManager.Instance.EnemyCollisionRectangles = ((sender as CheckBox).IsChecked ?? false);
        }

        private void ExitableToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            //If item is checked update variables
            CheckBox senderCheckBox = (CheckBox) sender;
            if (int.TryParse(senderCheckBox.Content.ToString(), out var arrayIndex))
            {
                GameGridManager.Instance.ExitLocations[arrayIndex].CanExit = senderCheckBox.IsChecked ?? false;
            }
        }
           
    }
}
