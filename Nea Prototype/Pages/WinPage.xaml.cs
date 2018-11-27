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

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for WinPage.xaml
    /// </summary>
    public partial class WinPage : Page, IKeyboardInputs
    {
        public WinPage()
        {
            InitializeComponent();
        }

        private void BtnContinue_OnClick(object sender, RoutedEventArgs e)
        {
            while (TopFrameManager.Instance.MainFrame.CanGoBack)
            {
                TopFrameManager.Instance.MainFrame.GoBack();
            }
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //No keydown event needed
        }
    }
}
