using Nea_Prototype.Characters;
using Nea_Prototype.Grid;
using Nea_Prototype.Level;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Nea_Prototype.Enums;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private DispatcherTimer keyboardInputTimer;
        //private CharacterItem character = new CharacterItem(new PlayerOne());
        //private GridItemView characterView;
        //private CharacterItem enemy = new CharacterItem(new PlayerTwo());
        //private GridItemView enemyView;
        //private NonWalkable nonWalkableTile = new NonWalkable();
        //private GridItemView nonwalkableView;
        //private Walkable walkableTile = new Walkable();
        //private GridItemView walkableView;

        private Level.Level level = LevelIO.ReadJSON("testing.json");

        public GamePage()
        {
            InitializeComponent();
            //characterView = new GridItemView(character);
            //enemyView = new GridItemView(enemy);
            //nonwalkableView = new GridItemView(nonWalkableTile);
            //walkableView = new GridItemView(walkableTile);
            level.SetupGrid(ref cvsPlayArea, EnemyType.Local);
            GameGridManager.GetGameGrid().GameCanvas = cvsPlayArea;
            keyboardInputTimer = new DispatcherTimer()
            {
                //Every ~1/1000 of a second update
                Interval = new TimeSpan(0, 0, 0, 0, 1)
            };
            keyboardInputTimer.Tick += KeyboardInputTimerTick;
            
            Loaded += (s, e) =>
            {
                //cvsPlayArea.Children.Add(characterView);
                //Canvas.SetLeft(characterView, 40);
                //Canvas.SetTop(characterView, 40);

                //cvsPlayArea.Children.Add(enemyView);
                //Canvas.SetLeft(enemyView, 360);
                //Canvas.SetTop(enemyView, 360);
                ////Canvas.SetLeft(character, 40);
                ////Canvas.SetTop(character, 40);

                //cvsPlayArea.Children.Add(nonwalkableView);
                //Canvas.SetBottom(nonwalkableView, 40);
                //Canvas.SetLeft(nonwalkableView, 40);

                //cvsPlayArea.Children.Add(walkableView);
                //Canvas.SetTop(walkableView, 40);
                //Canvas.SetRight(walkableView,40);
                

                keyboardInputTimer.Start();
            };
        }

        private void KeyboardInputTimerTick(object sender, EventArgs e)
        {
            //Timer is used for keyboard inputs so that the user can press two directions
            //and go diagonally, and so 2 players can play at once

            double getLeft;
            double getUp;
            if (Keyboard.IsKeyDown(KeyBindings.Player1_right))
            {
                level.MoveCharacter(1, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player1_left))
            {
                level.MoveCharacter(1, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindings.Player1_up))
            {
                level.MoveCharacter(1, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player1_down))
            {
                level.MoveCharacter(1, Direction.Down);
            }

            if (Keyboard.IsKeyDown(KeyBindings.Player2_right))
            {
                level.MoveCharacter(2, Direction.Right);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player2_left))
            {
                level.MoveCharacter(2, Direction.Left);
            }
            
            if (Keyboard.IsKeyDown(KeyBindings.Player2_up))
            {
                level.MoveCharacter(2, Direction.Up);
            }
            else if (Keyboard.IsKeyDown(KeyBindings.Player2_down))
            {
                level.MoveCharacter(2, Direction.Down);
            }
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            KeyboardInputTimerTick(sender, e);
        }
    }
}
