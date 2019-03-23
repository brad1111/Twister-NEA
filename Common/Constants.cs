namespace Common
{
    /// <summary>
    /// The values that stay constant within the game that I don't want to keep repeating
    /// to make it easier to change and easier to understand
    /// </summary>
    public static class Constants
    {
        //The quantity of gridtiles
        public const int GRID_TILES_XY = 20;
        //Which tile is the central gridtile
        public const int CENTRE_TILE_XY = GRID_TILES_XY / 2;
        //How many pixels a charcater will move on a given keypress
        public const int KEYPRESS_PX_MOVED = 1;
        //The width of a griditem
        public const int GRID_ITEM_WIDTH = 20;
        //The width of the entire grid
        public const int GRID_WIDTH = GRID_ITEM_WIDTH * GRID_TILES_XY;
    }
}