using System;
using System.Windows.Media.Imaging;

namespace Twister.Characters
{
    /// <summary>
    /// The enemy attempts to prevent charcater 1 from reaching the exit
    /// </summary>
    public class Enemy : Character
    {
        public Enemy()
        {
            weight = 1;
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri($@"{App.AppDir}\Assets\Enemy.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            sprite = src;
        }

    }
}