using System.Windows.Media;
using Common.Grid;

namespace Nea_Prototype.Grid
{
    /// <summary>
    /// The viewmodel binds the data of a model to a view
    /// </summary>
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