using System.Windows.Controls;

namespace Nea_Prototype.Characters
{
    public class Character
    {
        protected int weight;
        protected Image sprite;
        protected int score;

        /// <summary>
        /// Checks for collisions
        /// </summary>
        /// <param name=""></param>
        public void Collide(int x, int y)
        {

        }

        public int GetWeight => weight;
        public Image GetSprite => sprite;

        public int GetScore
        {
            get => score;
            set => score = value;
        }

    }
}