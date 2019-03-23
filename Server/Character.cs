using Common;
using Common.Grid;

namespace Server
{
    public class Character
    {
        /// <summary>
        /// The object that represents a chracter
        /// </summary>
        /// <param name="charNumber">Whether player 1 or player 2 (protagonist or enemy respectively)</param>
        /// <param name="startX">Their starting x coordinate</param>
        /// <param name="startY">Their starting y coordinate</param>
        public Character(int charNumber, int startX, int startY)
        {
            characterNumber = charNumber;
            CharacterPosition = new Position(startX,startY);
        }

        // Their character number (either 1 or 2)
        private int characterNumber { get; }

        //Their coordinates in terms of x and y
        public Position CharacterPosition { get; }
    }
}