using System;
using Nea_Prototype.Grid;

namespace Nea_Prototype.Algorithms
{
    public class Exiting
    {
        public double FindYInTheVerticalDirection(int yValueRelativeToGrid)
        {
            //TODO update previous angle to current angle
            return yValueRelativeToGrid * Math.Cos(GameGridManager.GetGameGrid().PreviousAngle);
        }

        public double FindXInTheHorizontalDirection(int xValueRelativeToGrid)
        {
            //TODO update previous angle to current angle and check this is right
            return xValueRelativeToGrid * Math.Sin(GameGridManager.GetGameGrid().PreviousAngle);
        }
    }
}