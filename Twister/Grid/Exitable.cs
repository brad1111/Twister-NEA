using System.Windows.Media;

namespace Twister.Grid
{
    /// <summary>
    /// An item that a character can only walk on if open
    /// </summary>
    public class Exitable : Walkable
    {
        private ImageSource closedSprite;
        private ImageSource openSprite;
        private int arrayIndex;

        public Exitable(int arrayIndex)
        {
            this.arrayIndex = arrayIndex;
            relativeLocation = "ExitableClosed.png";
            closedSprite = SetupSprite();
            relativeLocation = "ExitableOpen.png";
            openSprite = SetupSprite();
            Source = closedSprite;
            CurrentWeighting = int.MaxValue; // Current weight is infinity
        }

        //public new ImageSource Source
        //{
        //    get
        //    {
        //        //if you can exit show open if you cant show closed
        //        return (CanExit ? openSprite : closedSprite);
        //    }
        //}

        private bool canExit = false;

        //By default can't exit
        public bool CanExit
        {
            get => canExit;
            set
            {
                canExit = value;
                Source = (canExit ? openSprite : closedSprite);
            }
        }
    }
}