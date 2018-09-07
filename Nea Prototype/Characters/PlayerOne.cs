using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Nea_Prototype.Enums;

namespace Nea_Prototype.Characters
{
    public class PlayerOne : Character
    {
        public PlayerOne()
        {
            weight = 1;
            sprite = new CachedBitmap(new BitmapImage(new Uri("PlayerOne.png")), BitmapCreateOptions.None,
                BitmapCacheOption.Default);
        }

        public void Move(Direction direction)
        {

        }
    }
}