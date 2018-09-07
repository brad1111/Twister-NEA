using System;
using System.Windows.Media.Imaging;

namespace Nea_Prototype.Grid
{
    public class NonWalkable : GridItem
    {
        public NonWalkable()
        {
            sprite = new CachedBitmap(new BitmapImage(new Uri("NonWalkable.png")), BitmapCreateOptions.None,
                BitmapCacheOption.Default);
        }
    }
}