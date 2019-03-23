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

        /// <summary>
        /// Sum the two operands
        /// </summary>
        /// <param name="a">The operand before the +</param>
        /// <param name="b">The operand after the +</param>
        /// <returns>The sum of a and b</returns>
        public static Position operator + (Position a, Position b)
        {
            return new Position(a.x + b.x, a.y + b.y);
        }

        /// <summary>
        /// Override the equity command (they are equal if they have the same content)
        /// </summary>
        /// <param name="obj">An instance of this class</param>
        /// <returns>Whether the two instances of this class have equal content</returns>
        public override bool Equals(object obj)
        {
            Position pos = obj as Position;
            return pos?.x == this?.x &&
                   pos?.y == this?.y;
        }

        /// <summary>
        /// Override the equity operator (they are equal if they have the same content)
        /// </summary>
        /// <param name="a">The operand before ==</param>
        /// <param name="b">The operand after ==</param>
        /// <returns>Finds if the two instances of the classes have equal conent</returns>
        public static bool operator ==(Position a, Position b)
        {
            return a?.x == b?.x &&
                   a?.y == b?.y;
        }

        /// <summary>
        /// Overrides the nonequity oporator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
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