using Nea_Prototype.Enums;

namespace Nea_Prototype.Level
{
    public class ExitPlacement
    {
        public ExitPlacement(int heightFromAnchor, Side sideplaced, int length)
        {
            this.heightFromAnchor = heightFromAnchor;
            this.sidePlaced = sideplaced;
            this.length = length;
        }


        private int heightFromAnchor;
        private Side sidePlaced;
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