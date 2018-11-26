using System;
using System.Windows.Controls;
using Nea_Prototype.Characters;
using Nea_Prototype.Grid;
using Common;
using Common.Grid;

namespace Nea_Prototype.Algorithms
{
    public static class Rotation
    {
        /// <summary>
        /// Finds out the direction that the rotation will be
        /// </summary>
        /// <returns>The multiplier for direction of rotation (1 or -1)</returns>
        public static double RotationMultiplier(Character[] characters, ref double rotationAngle)
        {
            double[] charactersXPos = new double[characters.Length];
            int[] weights = new int[characters.Length];
            for (int i = 0; i < characters.Length; i++)
            {
                charactersXPos[i] = Canvas.GetLeft(GameGridManager.Instance.CharactersViews[i]);
                weights[i] = characters[i].GetWeight;
            }

            return Common.Algorithms.Rotation.RotationMultiplier(charactersXPos, weights, ref rotationAngle);
        }

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

            return Common.Algorithms.Rotation.AbsAngleDelta(charPositions, 0.25);
        }
    }
}