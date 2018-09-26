namespace Nea_Prototype.Grid
{
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