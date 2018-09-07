using Nea_Prototype.Enums;

namespace Nea_Prototype.Level
{
    public class ExitPlacement
    {
        private int heightFromAnchor;
        private Side sidePlaced;
        private int length;


        public int GetHeightFromAnchor()
        {
            return heightFromAnchor;
        }

        public Side GetSidePlaced()
        {
            return sidePlaced;
        }

        public int GetLength()
        {
            return length;
        }
    }
}