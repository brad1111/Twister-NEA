namespace Twister.Grid
{
    /// <summary>
    /// An item that the character can't walk on
    /// </summary>
    public class NonWalkable : GridItem
    {
        public NonWalkable()
        {
            relativeLocation = "NonWalkable.png";
            CurrentWeighting = int.MaxValue; // Current weight is infinity
        }
    }
}