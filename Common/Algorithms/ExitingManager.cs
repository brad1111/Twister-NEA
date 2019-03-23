using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Common.Algorithms
{
    public class ExitingManager
    {
        private List<double> AnglesToOpenInClockwiseDirection { get; } = new List<double>();
        private List<double> AnglesToCloseInClockwiseDirection { get; } = new List<double>();

        public ReadOnlyCollection<double> AnglesToOpen => AnglesToOpenInClockwiseDirection.AsReadOnly();
        public ReadOnlyCollection<double> AnglesToClose => AnglesToCloseInClockwiseDirection.AsReadOnly();

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
        public void FindAnglesNeededToOpenInternal(int e1, int e2, int i1, int i2)
        {

            //find the anglefrom i1 to e1 and i2 to e2
            double angle1 = Math.Atan(((double)10 / Math.Abs(i1 - e1))) * (180/Math.PI);
            double angle2 = Math.Atan(((double)10 / Math.Abs(i2 - e2))) * (180 / Math.PI);

            if (angle1 < angle2)
            {
                //Angle 1 is positive
                AnglesToOpenInClockwiseDirection.Add(angle1);
                AnglesToCloseInClockwiseDirection.Add(angle2);
            }
            else
            {
                //Angle 1 is negative
                AnglesToOpenInClockwiseDirection.Add(-angle1);
                AnglesToCloseInClockwiseDirection.Add(angle2);
            }


        }

        /// <summary>
        /// Clears the lists
        /// </summary>
        public void ClearLists()
        {
            AnglesToOpenInClockwiseDirection.Clear();
            AnglesToCloseInClockwiseDirection.Clear();
        }
    }
}