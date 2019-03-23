using Common;
using Common.Grid;

namespace Server.Level
{
    public class InternalExit
    {
        public InternalExit(Position gridPos)
        {
            this.GridPos = gridPos;
        }

        /// <summary>
        /// Stores the x and y co-ordinates of the Exit GridItem on the Grid
        /// </summary>
        public Position GridPos { get; }

        /// <summary>
        /// Converts from Grid location to a canvas position for exit calculations
        /// </summary>
        public Position CanvasPos
        {
            get
            {
                double x = GridPos.x * Constants.GRID_ITEM_WIDTH;
                double y = GridPos.y * Constants.GRID_ITEM_WIDTH;
                return new Position(x,y);
            }
        }
    }
}