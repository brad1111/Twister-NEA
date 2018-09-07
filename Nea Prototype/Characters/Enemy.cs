using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nea_Prototype.Characters
{
    public class Enemy : Character
    {
        public Enemy()
        {
            weight = 1;
            sprite = new CachedBitmap(new BitmapImage(new Uri("Enemy.png")), BitmapCreateOptions.None, BitmapCacheOption.Default);
        }

        public override void Collide(int x, int y)
        {
            base.Collide(x, y);
        }
    }
}