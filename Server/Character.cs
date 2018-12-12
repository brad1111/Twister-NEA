using Common;
using Common.Grid;

namespace Server
{
    public class Character
    {
        public Character(int charNumber, int startX, int startY)
        {
            characterNumber = charNumber;
            CharacterPosition = new Position(startX,startY);
        }

        private int characterNumber { get; }
        public Position CharacterPosition { get; }
    }
}