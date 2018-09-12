using Nea_Prototype.Characters;
using Nea_Prototype.Grid;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
        private Walkable walkableTile = new Walkable();
        private GridItemView walkableView;

        public GamePage()
        {
            InitializeComponent();
            characterView = new GridItemView(character);
            enemyView = new GridItemView(enemy);
            nonwalkableView = new GridItemView(nonWalkableTile);
            walkableView = new GridItemView(walkableTile);
            timer = new DispatcherTimer()
            {
                //Every ~1/60 of a second update
                Interval = new TimeSpan(0, 0, 0, 0, 17)
            };
            timer.Tick += TimerTick;
            
            Loaded += (s, e) =>
            {
                cvsPlayArea.Children.Add(characterView);
                Canvas.SetLeft(characterView, 40);
                Canvas.SetTop(characterView, 40);

                cvsPlayArea.Children.Add(enemyView);
                Canvas.SetLeft(enemyView, 360);
                Canvas.SetTop(enemyView, 360);
                //Canvas.SetLeft(character, 40);
                //Canvas.SetTop(character, 40);

                cvsPlayArea.Children.Add(nonwalkableView);
                Canvas.SetBottom(nonwalkableView, 40);
                Canvas.SetLeft(nonwalkableView, 40);

                cvsPlayArea.Children.Add(walkableView);
                Canvas.SetTop(walkableView, 40);
                Canvas.SetRight(walkableView,40);
                timer.Start();
            };
        }

        private void TimerTick(object sender, EventArgs e)
        {
            //Timer is used for keyboard inputs so that the user can press two directions
            //and go diagonally, and so 2 players can play at once

            double getLeft;
            double getUp;
            if (Keyboard.IsKeyDown(KeyBindings.Player1_right))
            {
                getLeft = Canvas.GetLeft(characterView);
                Canvas.SetLeft(characterView, getLeft + Constants.KEYPRESS_PX_MOVED);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player1_left))
            {
                getLeft = Canvas.GetLeft(characterView);
                Canvas.SetLeft(characterView, getLeft - Constants.KEYPRESS_PX_MOVED);
            }
            
            if (Keyboard.IsKeyDown(KeyBindings.Player1_up))
            {
                getUp = Canvas.GetTop(characterView);
                Canvas.SetTop(characterView, getUp - Constants.KEYPRESS_PX_MOVED);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player1_down))
            {
                getUp = Canvas.GetTop(characterView);
                Canvas.SetTop(characterView, getUp + Constants.KEYPRESS_PX_MOVED);
            }

            if (Keyboard.IsKeyDown(Key.Right))
            {
                getLeft = Canvas.GetLeft(enemyView);
                Canvas.SetLeft(enemyView, getLeft + Constants.KEYPRESS_PX_MOVED);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player2_left))
            {
                getLeft = Canvas.GetLeft(enemyView);
                Canvas.SetLeft(enemyView, getLeft - Constants.KEYPRESS_PX_MOVED);
            }
            
            if (Keyboard.IsKeyDown(KeyBindings.Player2_up))
            {
                getUp = Canvas.GetTop(enemyView);
                Canvas.SetTop(enemyView, getUp - Constants.KEYPRESS_PX_MOVED);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player2_down))
            {
                getUp = Canvas.GetTop(enemyView);
                Canvas.SetTop(enemyView, getUp + Constants.KEYPRESS_PX_MOVED);
            }
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
