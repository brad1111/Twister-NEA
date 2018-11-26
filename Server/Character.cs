using Common;
using Common.Grid;

namespace Server
{
    public class Character
    {
        public Character(int charNumber)
        {
            characterNumber = charNumber;
            int tempPos = charNumber == 1 ? Constants.GRID_ITEM_WIDTH : Constants.GRID_WIDTH - Constants.GRID_ITEM_WIDTH;
            CharacterPosition = new Position(tempPos,tempPos);
        }

        private int characterNumber { get; }
        public Position CharacterPosition { get; }
    }
}