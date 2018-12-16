using System.Diagnostics;
using System.Windows.Controls;
using Twister.Grid;
using Twister.Pages;

namespace Twister
{
    public class TopFrameManager
    {
        private TopFrameManager()
        {
            
        }

        public static TopFrameManager Instance { get; } = new TopFrameManager();

        public Frame MainFrame { get; set; }

        public Frame OverlayFrame { get; set; }

        private Process serverProcess = null;

        /// <summary>
        /// Handles the server process so that there can only be one server running at once
        /// </summary>
        public Process ServerProcess
        {
            get
            {
                return serverProcess;
            }
            set
            {
                //If there is already a process running then stop it
                if (serverProcess != null && !serverProcess.HasExited)
                {
                    serverProcess.CloseMainWindow();
                }
                serverProcess = value;
            }
        }

        /// <summary>
        /// Attempts to close the server gracefully
        /// </summary>
        public void TryCloseServer()
        {
            if (ServerProcess != null && !serverProcess.HasExited)
            {
                serverProcess.CloseMainWindow();
            }
        }

        /// <summary>
        /// Kills the server
        /// </summary>
        public void TryKillServer()
        {
            if (ServerProcess != null && !ServerProcess.HasExited)
            {
                serverProcess.Kill();
            }
        }

        public void CloseGame()
        {
            MainWindow.Close();
        }

        public MainWindow MainWindow { private get; set; }

        public void Focus()
        {
            MainWindow.Focus();
        }

        public void GoToMainMenu()
        {
            //End the game completely if running
            if (MainFrame.Content is GamePage)
            {
                GameGridManager.Clear();
                GamePage gp = (GamePage) MainFrame.Content;
                gp.EndGame();
            }
            //Clear the overlay frame
            ClearOverlayFrame();
            //Clear the main frame
            while (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

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