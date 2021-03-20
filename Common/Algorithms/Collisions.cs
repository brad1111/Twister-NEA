using System.Drawing;

namespace Common.Algorithms
{
    public class Collisions
    {
        public static Rectangle rectangle1 { get; private set; }
        public static Rectangle rectangle2 { get; private set; }

        /// <summary>
        /// Detects whether the two characters intersect
        /// </summary>
        /// <returns>Whether the enemy has collided with Player 1</returns>
        public static bool EnemyCollisionDetectionCommon(double character1GetLeft, double character1GetTop
                                                 , double character2GetLeft, double character2GetTop)
        {

            //Create both character rectangles
            rectangle1 = new Rectangle((int) (character1GetLeft + 1), (int) (character1GetTop + 1), Constants.GRID_ITEM_WIDTH - 2, Constants.GRID_ITEM_WIDTH - 2);
            rectangle2 = new Rectangle((int) (character2GetLeft + 1), (int) (character2GetTop + 1), Constants.GRID_ITEM_WIDTH - 2, Constants.GRID_ITEM_WIDTH - 2);

            //Returns whether they intersect
            return rectangle1.IntersectsWith(rectangle2);
        }
    }
}