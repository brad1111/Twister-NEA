using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nea_Prototype.Characters
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
            src.UriSource = new Uri("Enemy.png", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            sprite = src;
        }

    }
}