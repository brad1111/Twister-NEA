using Common.Enums;

namespace Common.Level
{
    public class ExitPlacement
    {
        public ExitPlacement(int heightFromAnchor, Side sideplaced, int length)
        {
            //Setup exitplacement
            this.heightFromAnchor = heightFromAnchor;
            this.sidePlaced = sideplaced;
            this.length = length;
        }

        //How high to place the external exit
        private int heightFromAnchor;
        //Which side to place the external exit
        private Side sidePlaced;
        //How big the external exit is
        private int length;


        /// <summary>
        /// The height from the top to place this item
        /// </summary>
        public int HeightFromAnchor
        {
            get => heightFromAnchor;
        }

        /// <summary>
        /// The side to place the exit on
        /// </summary>
        public Side SidePlaced
        {
            get => sidePlaced;
        }

        /// <summary>
        /// The length the exit item should be
        /// </summary>
        public int Length
        {
            get => length;
        }
    }
}