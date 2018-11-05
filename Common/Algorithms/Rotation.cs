using System;
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
        public static double RotationMultiplier(int[] charactersXPos, int[] charactersWeight, ref double rotationAngle)
        {
            double totalMomentFromCentre = 0;
            //Absolute because it does not matter which way it goes
            double rotationAngleRadians = Math.Abs((Math.PI / 180) * rotationAngle);

            for (int i = 0; i < charactersXPos.Length; i++)
            {
                //The distance from the centre line of the board relative to the rotation
                double relativeDistanceFromPivot = 200 - charactersXPos[i];

                double moment = relativeDistanceFromPivot * charactersWeight[i];

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

        public static double AbsAngleDelta(Position[] charactersPos)
        {
            double velocity = 9.8; //Assume velocity = acceleration * time = 1
            double time = 0.25; //TODO make this based on constants
            double totalDeltaRadians = 0;

            for (int i = 0; i < charactersPos.Length; i++)
            {
                double radiusFromCentrex = 200 - charactersPos[i].x;
                double radiusFromCentrey = 200 - charactersPos[i].y;
                //c=sqrt(a^2+b^2) (pythagoras)
                double radiusFromCentre = Math.Sqrt(Math.Pow(radiusFromCentrex, 2) + Math.Pow(radiusFromCentrey, 2));
                totalDeltaRadians += (velocity * time) / radiusFromCentre;
            }

            return Math.Abs(totalDeltaRadians / Math.PI)*180;
        }
    }
}