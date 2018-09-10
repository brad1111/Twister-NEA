using System.Windows.Media;

namespace Nea_Prototype.Grid
{
    public class GridItemViewModel
    {
        private GridItem gridItem;

        public GridItemViewModel(GridItem gridItem)
        {
            this.gridItem = gridItem;

        }

        public ImageSource Sprite => gridItem.Sprite;

        public Position Position => gridItem.Position;
    }
}