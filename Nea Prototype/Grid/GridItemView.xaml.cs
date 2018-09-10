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
    /// Interaction logic for GridItemView.xaml
    /// </summary>
    public partial class GridItemView : UserControl
    {
        private GridItemViewModel viewModel;

        public GridItemView(GridItem gridItem)
        {
            InitializeComponent();
            DataContext = viewModel = new GridItemViewModel(gridItem);
            //BitmapImage src = new BitmapImage();
            //src.BeginInit();
            //src.UriSource = new Uri("PlayerOne.png", UriKind.Relative);
            //src.CacheOption = BitmapCacheOption.OnLoad;
            //src.EndInit();
            //imgGridImg.Source = viewModel.Sprite;
        }
        

        protected ImageSource sprite;
        protected Position position;

        //public ImageSource GetSprite()
        //{
        //    //return sprite;
        //    return new BitmapImage(new Uri("PlayerOne.png", UriKind.Relative));
        //}

        //public Position GetPosition()
        //{
        //    return position;
        //}
    }
}
