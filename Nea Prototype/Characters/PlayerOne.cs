using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Nea_Prototype.Enums;

namespace Nea_Prototype.Characters
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
            src.UriSource = new Uri("PlayerOne.png", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            sprite = src;

        }

        public void Move(Direction direction)
        {

        }
    }
}