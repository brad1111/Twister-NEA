using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Shapes;
using Common;
using Nea_Prototype.Grid;

namespace Nea_Prototype.Algorithms
{
    public class ExitingManager
    {
        private List<double> AnglesToOpenInClockwiseDirection { get; } = new List<double>();
        private List<double> AnglesToCloseInClockwiseDirection { get; } = new List<double>();

        private ExitingManager()
        {
            
        }

        public static ExitingManager Instance { get; } = new ExitingManager();

        /// <summary>
        /// Finds and stores the angles needed to open/close from the lines drawn between the items
        /// </summary>
        /// <param name="e1">The top of the external exit</param>
        /// <param name="e2">The bottom of the external exit</param>n
        /// <param name="i1">The top of the internal exit</param>
        /// <param name="i2">The bottom of the internal exit</param>
        private void FindAnglesNeededToOpenInternal(int e1, int e2, int i1, int i2)
        {
            

            ////Adds degrees version of angle to array
            ////Converts to double otherwise it will round the division to 0.
            //AnglesToOpenInClockwiseDirection.Add(Math.Acos((double)e1 / i1) * (180/Math.PI));
            //AnglesToCloseInClockwiseDirection.Add(Math.Acos((double)e2 / i2) * (180/Math.PI));

            //find the anglefrom i1 to e1 and i2 to e2
            double angle1 = Math.Atan(((double)10 / Math.Abs(i1 - e1))) * (180/Math.PI);
            double angle2 = Math.Atan(((double)10 / Math.Abs(i2 - e2))) * (180 / Math.PI);

            if (angle1 < angle2)
            {
                AnglesToOpenInClockwiseDirection.Add(angle1);
                AnglesToCloseInClockwiseDirection.Add(angle2);
            }
            else
            {
                AnglesToOpenInClockwiseDirection.Add(-angle1);
                AnglesToCloseInClockwiseDirection.Add(angle2);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e1">The external top height</param>
        /// <param name="e2">The length of the exit</param>
        public void FindAnglesNeededToOpen(int e1, int e2)
        {
            AnglesToOpenInClockwiseDirection.Clear();
            AnglesToCloseInClockwiseDirection.Clear();
            foreach (var exitLocation in GameGridManager.GetGameGrid().ExitLocationsViews)
            {
                int getTop = (int)Canvas.GetTop(exitLocation);
                FindAnglesNeededToOpenInternal(e1, e2, getTop, getTop + Constants.GRID_ITEM_WIDTH);
            }

            //AnglesToOpenInClockwiseDirection.AddRange(new List<double>(){-20,-20});
            //AnglesToCloseInClockwiseDirection.AddRange(new List<double>(){20,20});
        }

        public void CheckForUpdates(int currentAngle, int rotationMultiplier)
        {
            if (rotationMultiplier == 0)
            {
                return;
                //Dont bother if it isn't rotating
            }


            if (rotationMultiplier > 0)
            {
                //Positive rotation
                for (int i = 0; i < AnglesToOpenInClockwiseDirection.Count; i++)
                {
                    if (AnglesToOpenInClockwiseDirection[i] < currentAngle)
                    {
                        GameGridManager.GetGameGrid().ExitLocations[i].CanExit = true;
                    }
                    if (AnglesToCloseInClockwiseDirection[i] < currentAngle)
                    {
                        GameGridManager.GetGameGrid().ExitLocations[i].CanExit = false;
                    }
                }
            }
            else /*if (rotationMultiplier > 0)*/
            {
                //Negative rotation
                for (int i = 0; i < AnglesToOpenInClockwiseDirection.Count; i++)
                {
                    if (AnglesToCloseInClockwiseDirection[i] > currentAngle)
                    {
                        GameGridManager.GetGameGrid().ExitLocations[i].CanExit = true;
                    }
                    if (AnglesToOpenInClockwiseDirection[i] > currentAngle)
                    {
                        GameGridManager.GetGameGrid().ExitLocations[i].CanExit = false;
                    }

                }
            }
        }

        public ReadOnlyCollection<double> AnglesToOpen => AnglesToOpenInClockwiseDirection.AsReadOnly();
        public ReadOnlyCollection<double> AnglesToClose => AnglesToCloseInClockwiseDirection.AsReadOnly();

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