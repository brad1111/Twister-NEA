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
        protected int weight;
        //The characters image
        protected ImageSource sprite;
        //The charcaters 'score'
        protected int score;
        //The characters location
        private Position location;

        /// <summary>
        /// Gets the weight for location calculations
        /// </summary>
        public int GetWeight => weight;

        /// <summary>
        /// Gets the sprite of the charcaters
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