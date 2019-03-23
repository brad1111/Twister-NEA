using System.Windows.Media;
using Common.Grid;

namespace Twister.Characters
{
    /// <summary>
    /// Represents all characters
    /// </summary>
    public class Character
    {
        //Mass of character for rotation calculations
        protected double weight;
        //The characters image
        protected ImageSource sprite;
        //The characters 'score'
        protected int score;
        //The characters location
        private Position location;

        /// <summary>
        /// Gets the weight for location calculations
        /// </summary>
        public double Weight
        {
            get => weight;
            set => weight = value;
        }

        /// <summary>
        /// Gets the sprite of the characters
        /// </summary>
        /// <returns>The sprite</returns>
        public ImageSource GetSprite()
        {
            return sprite;
        }
    

        /// <summary>
        /// Gets/sets the score of the character
        /// </summary>
        public int GetScore
        {
            get => score;
            set => score = value;
        }

        /// <summary>
        /// Gets or sets the position of the character
        /// </summary>
        public Position Position
        {
            get => location; 
            set => location = value;
        }
    }
}