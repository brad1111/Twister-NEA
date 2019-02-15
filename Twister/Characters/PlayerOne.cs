using System;
using System.Windows.Media.Imaging;
using Common.Enums;

namespace Twister.Characters
{
    /// <summary>
    /// Player 1 attempts to avoid the enemy and get to the exit
    /// </summary>
    public class PlayerOne : Character
    {
        public PlayerOne()
        {
            weight = 1;
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri($@"{App.AppDir}\Assets\PlayerOne.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            sprite = src;

        }
    }
}