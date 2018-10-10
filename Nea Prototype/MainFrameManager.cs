using System.Windows.Controls;

namespace Nea_Prototype
{
    public class MainFrameManager
    {
        private MainFrameManager()
        {
            
        }

        public static MainFrameManager FrameManager { get; } = new MainFrameManager();

        public Frame MainFrame { get; set; }
    }
}