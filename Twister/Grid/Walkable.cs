namespace Twister.Grid
{
    /// <summary>
    /// An item that characters can walk on
    /// </summary>
    public class Walkable : GridItem
    {
        public Walkable()
        {
            absoluteLocation = $@"{App.AppDir}\Assets\Walkable.png";
        }        
    }
}