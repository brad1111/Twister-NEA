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
using System.Windows.Media.Animation;
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
        private CharacterItem enemy = new CharacterItem(new PlayerTwo());
        private GridItemView enemyView;
        private NonWalkable nonWalkableTile = new NonWalkable();
        private GridItemView nonwalkableView;
        private Storyboard rotationStoryboard = null;

        public GamePage()
        {
            InitializeComponent();
            characterView = new GridItemView(character);
            enemyView = new GridItemView(enemy);
            nonwalkableView = new GridItemView(nonWalkableTile);
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

                cvsPlayArea.Children.Add(enemyView);
                Canvas.SetRight(enemyView, 40);
                Canvas.SetBottom(enemyView, 40);
                //Canvas.SetLeft(character, 40);
                //Canvas.SetTop(character, 40);

                cvsPlayArea.Children.Add(nonwalkableView);
                Canvas.SetBottom(nonwalkableView, 40);
                Canvas.SetLeft(nonwalkableView, 40);

            };
        }


        private void StoryBoardRotation()
        {
            if (rotationStoryboard is null || rotationStoryboard?.GetCurrentProgress() == 1.0)
            {
                rotationStoryboard = new Storyboard();
                rotationStoryboard.Duration = new Duration(new TimeSpan(0, 0, 1));
                DoubleAnimation animation = new DoubleAnimation()
                {
                    From = 0,
                    To = 30,
                    Duration = rotationStoryboard.Duration
                };
                rotationStoryboard.Children.Add(animation);
                Storyboard.SetTarget(animation, cvsPlayArea);
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

                rotationStoryboard.Begin();
            }
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
            
            StoryBoardRotation();
        }
    }
}
