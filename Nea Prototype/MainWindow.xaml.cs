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
using Nea_Prototype.Pages;

namespace Nea_Prototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GamePage page = new GamePage();

        public MainWindow()
        {
            InitializeComponent();
            frmMainFrame.Navigate(page);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == KeyBindings.DebugOverlayModifier &&
                e.Key == KeyBindings.DebugOverlayKey)
            {
                if (frmOverlay.Content?.GetType() == typeof(DebugOverlayPage))
                {
                    //Goto a new null page
                    frmOverlay.Navigate(new Page());
                    frmOverlay.NavigationService.RemoveBackEntry();
                }
                else
                {
                    frmOverlay.Navigate(new DebugOverlayPage());
                }
            }

            page.Page_KeyDown(sender, e);
        }
    }
}
