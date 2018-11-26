using System;
using System.Windows.Media.Imaging;

namespace Nea_Prototype.Grid
{
    /// <summary>
    /// An item that characters can walk on
    /// </summary>
    public class Walkable : GridItem
    {
        public Walkable()
        {
            relativeLocation = "Walkable.png";
        }        
    }
}