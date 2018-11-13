using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Nea_Prototype.Controls;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for ConnectPage.xaml
    /// </summary>
    public partial class ConnectPage : Page, IKeyboardInputs
    {
        public ConnectPage()
        {
            InitializeComponent();

            string IPRegex =
                /*IPV4*/
                @"((([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]))";
               // /*domain name*/ "(([a-zA-Z0-9].)*([a-zA-Z0-9]))";
            txtIP.RegularExpression = IPRegex;
            
        }

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //Do nothing
        }
    }
}
