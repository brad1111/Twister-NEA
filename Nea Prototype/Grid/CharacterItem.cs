using Nea_Prototype.Characters;

namespace Nea_Prototype.Grid
{
    public class CharacterItem : GridItem
    {
        private Character character;

        public CharacterItem(Character character)
        {
            this.character = character;
            sprite = character.GetSprite();
        }

        public Character GetCharacter => character;
    }
}