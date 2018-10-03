using System;
using System.Windows.Controls;
using Nea_Prototype.Characters;
using Nea_Prototype.Grid;

namespace Nea_Prototype.Algorithms
{
    public class Rotation
    {
        /// <summary>
        /// Finds out the direction that the rotation will be
        /// </summary>
        /// <returns>The multiplier for direction of rotation (1 or -1)</returns>
        public static double RotationMultiplier(Character[] characters, ref double rotationAngle)
        {
            double totalMomentFromCentre = 0;
            //Absolute because it does not matter which way it goes
            double rotationAngleRadians = Math.Abs((Math.PI / 180) * rotationAngle);

            for (int i = 0; i < characters.Length; i++)
            {
                //The distance from the centre line of the board relative to the rotation
                double relativeDistanceFromPivot = 200 - Canvas.GetLeft(GameGridManager.GetGameGrid().CharactersViews[i]);
                
                //The distance from the centre of the board in the x direction no matter the rotation
                double fixedDistanceFromPivot = 0;

                if (-90 < rotationAngle && rotationAngle < 90)
                {   
                    fixedDistanceFromPivot = relativeDistanceFromPivot * Math.Sin((Math.PI / 2)-rotationAngleRadians);
                }

                int posFromCentre = (characters[i].Position.x + 1) - Constants.CENTRE_TILE_XY;

                double moment = relativeDistanceFromPivot * characters[i].GetWeight;

                totalMomentFromCentre += moment;
            }

            if (totalMomentFromCentre == 0)
            {
                return 0;
            }
            else
            {
                //Prevents divide by 0 error
                return -(totalMomentFromCentre / Math.Abs(totalMomentFromCentre));
            }
        }

        public static double AbsAngleDelta()
        {
            double velocity = 9.8; //Assume velocity = acceleration * time = 1
            int time = 1;
            double totalDeltaRadians = 0;
            //Get characters views
            GridItemView[] charViews = GameGridManager.GetGameGrid().CharactersViews;

            for (int i = 0; i < charViews.Length; i++)
            {
                double radiusFromCentrex = 200 - Canvas.GetLeft(charViews[i]);
                //double radiusFromCentrey = 200 - Canvas.GetTop(charViews[i]);
                //c=sqrt(a^2+b^2) (pythagoras)
                //double radiusFromCentre = Math.Sqrt(Math.Pow(radiusFromCentrex, 2) + Math.Pow(radiusFromCentrey, 2));
                totalDeltaRadians += (velocity * time) / radiusFromCentrex;
            }

            return Math.Abs(totalDeltaRadians / Math.PI)*180;
        }
    }
}