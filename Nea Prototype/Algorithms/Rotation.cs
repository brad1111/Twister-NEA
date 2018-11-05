using System;
using System.Windows.Controls;
using Nea_Prototype.Characters;
using Nea_Prototype.Grid;
using Common;

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
                charactersXPos[i] = Canvas.GetLeft(GameGridManager.GetGameGrid().CharactersViews[i]);
                weights[i] = characters[i].GetWeight;
            }

            return Common.Algorithms.Rotation.RotationMultiplier(charactersXPos, weights, ref rotationAngle);
        }

        public static double AbsAngleDelta()
        {
            double velocity = 9.8; //Assume velocity = acceleration * time = 1
            double time = 0.25;
            double totalDeltaRadians = 0;
            //Get characters views
            GridItemView[] charViews = GameGridManager.GetGameGrid().CharactersViews;

            for (int i = 0; i < charViews.Length; i++)
            {
                double radiusFromCentrex = 200 - Canvas.GetLeft(charViews[i]);
                double radiusFromCentrey = 200 - Canvas.GetTop(charViews[i]);
                //c=sqrt(a^2+b^2) (pythagoras)
                double radiusFromCentre = Math.Sqrt(Math.Pow(radiusFromCentrex, 2) + Math.Pow(radiusFromCentrey, 2));
                totalDeltaRadians += (velocity * time) / radiusFromCentre;
            }

            return Math.Abs(totalDeltaRadians / Math.PI)*180;
        }
    }
}