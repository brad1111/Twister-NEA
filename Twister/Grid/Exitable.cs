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
            absoluteLocation = $@"{App.AppDir}\Assets\ExitableClosed.png";
            closedSprite = SetupSprite();
            absoluteLocation = $@"{App.AppDir}\Assets\ExitableOpen.png";
            openSprite = SetupSprite();
            Source = closedSprite;
            CurrentWeighting = int.MaxValue; // Current weight is infinity
        }

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