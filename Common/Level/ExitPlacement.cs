using Common.Enums;

namespace Common.Level
{
    public class ExitPlacement
    {
        public ExitPlacement(int heightFromAnchor, Side sideplaced, int length)
        {
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


        public int HeightFromAnchor
        {
            get => heightFromAnchor;
        }

        public Side SidePlaced
        {
            get => sidePlaced;
        }

        public int Length
        {
            get => length;
        }
    }
}