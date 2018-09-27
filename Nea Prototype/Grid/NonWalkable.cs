using System;
using System.Windows.Media.Imaging;

namespace Nea_Prototype.Grid
{
    /// <summary>
    /// An item that the character can't walk on
    /// </summary>
    public class NonWalkable : GridItem
    {
        public NonWalkable()
        {
            sprite = SetupSprite("NonWalkable.png");
        }
    }
}