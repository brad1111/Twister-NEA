﻿using Common;
using Common.Grid;

namespace Server.Level
{
    public class InternalExit
    {
        public InternalExit(Position gridPos)
        {
            this.GridPos = gridPos;
        }

        public Position GridPos { get; }

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