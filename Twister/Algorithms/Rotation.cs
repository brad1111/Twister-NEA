using System.Windows.Controls;
using Common.Grid;
using Twister.Characters;
using Twister.Grid;

namespace Twister.Algorithms
{
    public static class Rotation
    {
        /// <summary>
        /// Finds out the direction that the rotation will be
        /// </summary>
        /// <returns>The multiplier for direction of rotation (1 or -1)</returns>
        public static double RotationMultiplier(Character[] characters)
        {
            //Create characters and weights 
            double[] charactersXPos = new double[characters.Length];
            double[] weights = new double[characters.Length];
            for (int i = 0; i < characters.Length; i++)
            {
                charactersXPos[i] = Canvas.GetLeft(GameGridManager.Instance.CharactersViews[i]);
                weights[i] = characters[i].Weight;
            }

            return Common.Algorithms.Rotation.RotationMultiplier(charactersXPos, weights);
        }

        /// <summary>
        /// The absolute (non-negative) angle difference
        /// </summary>
        /// <returns>The difference</returns>
        public static double AbsAngleDelta()
        {
            //Create position array
            Position[] charPositions = new Position[GameGridManager.Instance.Characters.Length];
            for (int i = 0; i < GameGridManager.Instance.Characters.Length; i++)
            {
                charPositions[i] = new Position(
                    x:Canvas.GetLeft(GameGridManager.Instance.CharactersViews[i]),
                    y:Canvas.GetTop(GameGridManager.Instance.CharactersViews[i]));
            }

            //Create weights array
            double[] weights =
                {GameGridManager.Instance.Characters[0].Weight, GameGridManager.Instance.Characters[1].Weight};
            return Common.Algorithms.Rotation.AbsAngleDelta(charPositions, 0.25, weights);
        }
    }
}