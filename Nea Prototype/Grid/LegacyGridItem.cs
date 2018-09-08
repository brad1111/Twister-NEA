using System.Windows.Controls;
using System.Windows.Media;

namespace Nea_Prototype.Grid
{
    public class LegacyGridItem : UserControl, IGridItem
    {
        public LegacyGridItem()
        {
            Content = new Image()
            {
                Source = sprite,
                Height = 40,
                Width = 40
            };
        }
        
        protected ImageSource sprite;

        public ImageSource GetSprite()
        {
            return sprite;
        }

        /*public override void EndInit()
        {
            base.EndInit();
            base.Source = sprite;
        }*/
    }
}