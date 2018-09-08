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

namespace Nea_Prototype.Grid
{
    /// <summary>
    /// Interaction logic for GridItem.xaml
    /// </summary>
    public partial class GridItem : UserControl, IGridItem
    {
        public GridItem()
        {
            InitializeComponent();
        }

        protected ImageSource sprite;
        protected Position position;

        public ImageSource GetSprite()
        {
            //return sprite;
            return new BitmapImage(new Uri("PlayerOne.png", UriKind.Relative));
        }

        public Position GetPosition()
        {
            return position;
        }
    }
}
