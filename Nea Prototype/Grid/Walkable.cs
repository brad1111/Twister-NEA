using System;
using System.Windows.Media.Imaging;

namespace Nea_Prototype.Grid
{
    public class Walkable : GridItem
    {
        public Walkable()
        {
            sprite = new CachedBitmap(new BitmapImage(new Uri("Walkable.png")), BitmapCreateOptions.None,
                BitmapCacheOption.Default);
        }        
    }
}