namespace Common.Grid
{
    /// <summary>
    /// The position of a grid item within the maze
    /// </summary>
    public class Position
    {

        public Position(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double x { get; set; }
        public double y { get; set; }
    }
}