using Nea_Prototype.Characters;

namespace Nea_Prototype.Grid
{
    public class GameGrid
    {
        private Character[] characters;
        private GridItemView[] charactersView;
        private GridItemView[,] gridItemsViews;
        private GridItem[,] gridItems;
        
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

        public GameGrid(Character[] characters, GridItemView[] charactersView, GridItemView[,] gridItemsViews, GridItem[,] gridItems)
        {
            this.characters = characters;
            this.charactersView = charactersView;
            this.gridItemsViews = gridItemsViews;
            this.gridItems = gridItems;
        }
    }
}