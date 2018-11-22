using System.Windows.Controls;

namespace Nea_Prototype
{
    public class TopFrameManager
    {
        private TopFrameManager()
        {
            
        }

        public static TopFrameManager Instance { get; } = new TopFrameManager();

        public Frame MainFrame { get; set; }

        public Frame OverlayFrame { get; set; }

        /// <summary>
        /// Clears the overlayframe
        /// </summary>
        public void ClearOverlayFrame()
        {
            //Goto a new null menu
            OverlayFrame.Navigate(new Page());
            OverlayFrame.NavigationService.RemoveBackEntry();
        }
    }
}