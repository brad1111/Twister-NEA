namespace Nea_Prototype.Grid
{
    /// <summary>
    /// An item that a character can only walk on if open
    /// </summary>
    public class Exitable : Walkable
    {
        public Exitable()
        {
            SetupSprite("Exitable.png");
        }

        //By default can't exit
        public bool CanExit { get; set; }
    }
}