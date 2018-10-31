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
        private static Common.Algorithms.ExitingManager instance = Common.Algorithms.ExitingManager.Instance;   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e1">The external top height</param>
        /// <param name="e2">The length of the exit</param>
        public void FindAnglesNeededToOpen(int e1, int e2)
        {
            instance.ClearLists();
            foreach (var exitLocation in GameGridManager.GetGameGrid().ExitLocationsViews)
            {
                int getTop = (int)Canvas.GetTop(exitLocation);
                instance.FindAnglesNeededToOpenInternal(e1, e2, getTop, getTop + Constants.GRID_ITEM_WIDTH);
            }
        }

        public static void CheckForUpdates(int currentAngle, int rotationMultiplier)
        {
            if (rotationMultiplier == 0)
            {
                return;
                //Dont bother if it isn't rotating
            }


            if (rotationMultiplier > 0)
            {
                //Positive rotation
                for (int i = 0; i < instance.AnglesToOpen.Count; i++)
                {
                    if (instance.AnglesToOpen[i] < currentAngle)
                    {
                        GameGridManager.GetGameGrid().ExitLocations[i].CanExit = true;
                    }
                    if (instance.AnglesToClose[i] < currentAngle)
                    {
                        GameGridManager.GetGameGrid().ExitLocations[i].CanExit = false;
                    }
                }
            }
            else /*if (rotationMultiplier > 0)*/
            {
                //Negative rotation
                for (int i = 0; i < instance.AnglesToOpen.Count; i++)
                {
                    if (instance.AnglesToClose[i] > currentAngle)
                    {
                        GameGridManager.GetGameGrid().ExitLocations[i].CanExit = true;
                    }
                    if (instance.AnglesToOpen[i] > currentAngle)
                    {
                        GameGridManager.GetGameGrid().ExitLocations[i].CanExit = false;
                    }

                }
            }
        }
    }
}