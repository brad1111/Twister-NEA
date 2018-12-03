using System.Security.Cryptography;

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

        public static Position operator + (Position a, Position b)
        {
            return new Position(a.x + b.x, a.y + b.y);
        }

        public override bool Equals(object obj)
        {
            Position pos = obj as Position;
            return pos?.x == this?.x &&
                   pos?.y == this?.y;
        }

        public static bool operator ==(Position a, Position b)
        {
            return a?.x == b?.x &&
                   a?.y == b?.y;
        }

        public static bool operator !=(Position a, Position b)
        {
            return a?.x != b?.x || 
                   a?.y != b?.y;
        }

        public override int GetHashCode()
        {
            //Overriden this to stop compiler from complaining
            return base.GetHashCode();
        }
    }
}