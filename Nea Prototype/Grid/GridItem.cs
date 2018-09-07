using System.Windows.Controls;
using System.Windows.Media;

namespace Nea_Prototype.Grid
{
    public class GridItem : Image, IGridItem
    {        
        protected ImageSource sprite;

        public ImageSource GetSprite()
        {
            return sprite;
        }

        public override void EndInit()
        {
            base.EndInit();
            base.Source = sprite;
        }
    }
}