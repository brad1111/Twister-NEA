using System.Windows.Controls;
using System.Windows.Media;
using Nea_Prototype.Grid;

namespace Nea_Prototype.Characters
{
    public class Character : IGridItem
    {
        protected int weight;
        protected ImageSource sprite;
        protected int score;
        private Position location;

        /// <summary>
        /// Checks for collisions
        /// </summary>
        /// <param name=""></param>
        public virtual void Collide(int x, int y)
        {

        }

        public int GetWeight => weight;

        public ImageSource GetSprite()
        {
            return sprite;
        }
    

        public int GetScore
        {
            get => score;
            set => score = value;
        }

        public Position GetPosition => location;
    }
}