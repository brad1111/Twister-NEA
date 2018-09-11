using System;
using System.Windows.Media.Imaging;

namespace Nea_Prototype.Grid
{
    public class NonWalkable : GridItem
    {
        public NonWalkable()
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri("NonWalkable.png", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            CachedBitmap cachedSrc = new CachedBitmap(src, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            sprite = cachedSrc;
        }
    }
}