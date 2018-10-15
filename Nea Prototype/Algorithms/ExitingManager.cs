using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Nea_Prototype.Grid;

namespace Nea_Prototype.Algorithms
{
    public class ExitingManager
    {
        private List<double> AnglesToOpenInClockwiseDirection = new List<double>();
        private List<double> AnglesToCloseInClockwiseDirection = new List<double>();

        private ExitingManager()
        {
            
        }

        public static ExitingManager Instance { get; } = new ExitingManager();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e1">The top of the external exit</param>
        /// <param name="e2">The bottom of the external exit</param>
        /// <param name="i1">The top of the internal exit</param>
        /// <param name="i2">The bottom of the internal exit</param>
        private void FindAnglesNeededToOpenInternal(int e1, int e2, int i1, int i2)
        {
            //Adds degrees version of angle to array
            AnglesToOpenInClockwiseDirection.Add(Math.Acos(e1 / i1) * (Math.PI/180));
            AnglesToCloseInClockwiseDirection.Add(Math.Acos(e2/i2) * (Math.PI/180));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e1">The external top height</param>
        /// <param name="e2">The length of the exit</param>
        public void FindAnglesNeededToOpen(int e1, int e2)
        {
            foreach (var exitLocation in GameGridManager.GetGameGrid().ExitLocationsViews)
            {
                int getTop = (int) Canvas.GetTop(exitLocation);
                FindAnglesNeededToOpenInternal(e1, e2, getTop, getTop + Constants.GRID_ITEM_WIDTH);
            }
        }

        //public double FindYInTheVerticalDirection(int yValueRelativeToGrid)
        //{
        //    //TODO update previous angle to current angle
        //    return yValueRelativeToGrid * Math.Cos(GameGridManager.GetGameGrid().PreviousAngle);
        //}

        //public double FindXInTheHorizontalDirection(int xValueRelativeToGrid)
        //{
        //    //TODO update previous angle to current angle and check this is right
        //    return xValueRelativeToGrid * Math.Sin(GameGridManager.GetGameGrid().PreviousAngle);
        //}
    }
}