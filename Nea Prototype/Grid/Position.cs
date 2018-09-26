namespace Nea_Prototype.Grid
{
    /// <summary>
    /// The position of a grid item within the maze
    /// </summary>
    public class Position
    {

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x { get; set; }
        public int y { get; set; }
    }
}