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
using Nea_Prototype.Characters;
using Nea_Prototype.Grid;

namespace Nea_Prototype
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private DispatcherTimer timer;
        private CharacterItem character = new CharacterItem(new PlayerOne());
        private GridItemView characterView;

        public GamePage()
        {
            InitializeComponent();
            characterView = new GridItemView(character);
            timer = new DispatcherTimer()
            {
                //Every ~1/60 of a second update
                Interval = new TimeSpan(0, 0, 0, 17),
            };
            timer.Tick += (s, e) => TimerTick();
            Loaded += (s, e) =>
            {
                cvsPlayArea.Children.Add(characterView);
                Canvas.SetLeft(characterView, 40);
                Canvas.SetTop(characterView, 40);

                //Canvas.SetLeft(character, 40);
                //Canvas.SetTop(character, 40);


            };
        }

        private void TimerTick()
        {

        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            double getLeft;
            double getUp;
            switch (e.Key)
            {
                case Key.Right:
                    getLeft = Canvas.GetLeft(characterView);
                    Canvas.SetLeft(characterView, getLeft + Constants.KEYPRESS_PX_MOVED);
                    break;
                case Key.Left:
                    getLeft = Canvas.GetLeft(characterView);
                    Canvas.SetLeft(characterView, getLeft - Constants.KEYPRESS_PX_MOVED);
                    break;
                case Key.Up:
                    getUp = Canvas.GetTop(characterView);
                    Canvas.SetTop(characterView, getUp - Constants.KEYPRESS_PX_MOVED);
                    break;
                case Key.Down:
                    getUp = Canvas.GetTop(characterView);
                    Canvas.SetTop(characterView, getUp + Constants.KEYPRESS_PX_MOVED);
                    break;
                default:
                    break;
            }

        }
    }
}
