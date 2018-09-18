using Nea_Prototype.Characters;

namespace Nea_Prototype.Grid
{
    public class GameGrid
    {
        private Character[] characters;
        private GridItemView[,] gridItemsViews;
        private GridItem[,] gridItems;
        
        public Character[] Characters
        {
            get => characters;
        }

        public GridItemView[,] GridItemsViews
        {
            get => gridItemsViews;
        }

        public GridItem[,] GridItems
        {
            get => gridItems;
        }

        public GameGrid(Character[] characters, GridItemView[,] gridItemsViews, GridItem[,] gridItems)
        {
            this.characters = characters;
            this.gridItemsViews = gridItemsViews;
            this.gridItems = gridItems;
        }
    }
}