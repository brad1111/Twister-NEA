using System.Windows.Controls;
using Nea_Prototype.Characters;
using Nea_Prototype.Enums;
using Newtonsoft.Json;

namespace Nea_Prototype.Grid
{
    public class GameGridManager
    {
        private Character[] characters;
        private GridItemView[] charactersView;
        private GridItemView[,] gridItemsViews;
        private GridItem[,] gridItems;

        private static readonly GameGridManager gameGridStorage = new GameGridManager();

        private GameGridManager()
        {
            
        }

        public static GameGridManager NewGameGrid(Character[] characters, GridItemView[] charactersView, GridItemView[,] gridItemsViews, GridItem[,] gridItems)
        {
            gameGridStorage.characters = characters;
            gameGridStorage.charactersView = charactersView;
            gameGridStorage.gridItemsViews = gridItemsViews;
            gameGridStorage.gridItems = gridItems;
            return gameGridStorage;
        }

        public static GameGridManager GetGameGrid()
        {
            return gameGridStorage;
        }

        public Canvas GameCanvas { get; set; }

        public Character[] Characters
        {
            get => characters;
        }

        public GridItemView[] CharactersViews
        {
            get => charactersView;
        }

        public GridItemView[,] GridItemsViews
        {
            get => gridItemsViews;
        }

        public GridItem[,] GridItems
        {
            get => gridItems;
        }

        public int DebuggingCanvasLeftovers { get; set; }

        public EnemyType EnemyType { get; set; }

        #region Debugging Variables

        public bool WallCollisionRectangles { get; set; }
        public bool EnemyCollisionRectangles { get; set; }

        #endregion
    }
}