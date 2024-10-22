﻿using System.Windows.Media;
using Twister.Characters;

namespace Twister.Grid
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
            absoluteLocation = sprite.ToString();
            if (character is Enemy)
            {
                RenderTransform = new TranslateTransform();
            }
        }

        public Character GetCharacter => character;
    }
}