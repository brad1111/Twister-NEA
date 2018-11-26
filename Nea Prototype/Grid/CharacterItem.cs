using Nea_Prototype.Characters;

namespace Nea_Prototype.Grid
{
    /// <summary>
    /// The chracter's grid item (the one that connects the character to the characterview)
    /// </summary>
    public class CharacterItem : GridItem
    {
        private Character character;

        public CharacterItem(Character character)
        {
            this.character = character;
            sprite = character.GetSprite();
            relativeLocation = sprite.ToString();
        }

        public Character GetCharacter => character;
    }
}