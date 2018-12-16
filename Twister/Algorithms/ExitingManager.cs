using System.Collections.ObjectModel;
using System.Windows.Controls;
using Common;
using Twister.Grid;

namespace Twister.Algorithms
{
    public class ExitingManager
    {
        private static Common.Algorithms.ExitingManager instance = Common.Algorithms.ExitingManager.Instance;   

        public static ReadOnlyCollection<double> AnglesToOpen => instance.AnglesToOpen;
        public static ReadOnlyCollection<double> AnglesToClose => instance.AnglesToClose;

        /// <summary>
        /// Finds angles needed to open/close the gates
        /// </summary>
        /// <param name="e1">The external top height</param>
        /// <param name="e2">The length of the exit</param>
        public static void FindAnglesNeededToOpen(int e1, int e2)
        {
            instance.ClearLists();
            foreach (var exitLocation in GameGridManager.Instance.ExitLocations)
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
                        GameGridManager.Instance.ExitLocations[i].CanExit = true;
                    }
                    if (instance.AnglesToClose[i] < currentAngle)
                    {
                        GameGridManager.Instance.ExitLocations[i].CanExit = false;
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
                        GameGridManager.Instance.ExitLocations[i].CanExit = true;
                    }
                    if (instance.AnglesToOpen[i] > currentAngle)
                    {
                        GameGridManager.Instance.ExitLocations[i].CanExit = false;
                    }

                }
            }
        }
    }
}