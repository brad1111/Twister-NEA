using System;
using Nea_Prototype.Characters;

namespace Nea_Prototype.Algorithms
{
    public class Rotation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int RotationMultiplier(Character[] characters, ref double rotationAngle)
        {
            int totalMomentFromCentre = 0;
            //Absolute because it does not matter which way it goes
            double rotationAngleRadians = Math.Abs((Math.PI / 180) * rotationAngle);

            foreach (Character character in characters)
            {
                //The distance from the centre line of the board relative to the rotation
                int relativeDistanceFromPivot = character.Position.x - 5;
                //The distance from the centre of the board in the x direction no matter the rotation
                double fixedDistanceFromPivot = 0;

                if (-90 < rotationAngle && rotationAngle < 90)
                {   
                    fixedDistanceFromPivot = character.Position.x * Math.Sin((Math.PI / 2)-rotationAngleRadians);
                }

                int posFromCentre = (character.Position.x + 1) - Constants.CENTRE_TILE_XY;

                int moment = posFromCentre * character.GetWeight;

                totalMomentFromCentre += moment;
            }

            return (totalMomentFromCentre / Math.Abs(totalMomentFromCentre));
        }
    }
}