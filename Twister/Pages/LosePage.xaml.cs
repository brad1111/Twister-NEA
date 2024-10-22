﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Enums;
using Twister.Grid;
using Twister.Network;

namespace Twister.Pages
{
    /// <summary>
    /// Interaction logic for WinPage.xaml
    /// </summary>
    public partial class LosePage : Page, IKeyboardInputs
    {
        private Level.Level level;
        private ProtagonistType pt;
        private EnemyType et;
        private bool isNetworked;

        /// <summary>
        /// Only call this on the overlay panel
        /// </summary>
        public LosePage(Level.Level level, ProtagonistType pt, EnemyType et, bool isNetworked)
        {
            InitializeComponent();
            //Stop the game
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                (TopFrameManager.Instance.MainFrame.Content as GamePage).StopTimers();
                if (isNetworked)
                {
                    CommunicationManager.Instance.Stop();
                }
            }

            this.level = level;
            this.pt = pt;
            this.et = et;
            this.isNetworked = isNetworked;
        }

        private void BtnEnd_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.GoToMainMenu();
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //No keydown event needed
        }

        private void BtnRetry_OnClick(object sender, RoutedEventArgs e)
        {
            GameGridManager.Clear();
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                (TopFrameManager.Instance.MainFrame.Content as GamePage).EndGame(disconnect:false);
            }
            //Recreate the gamepage
            TopFrameManager.Instance.MainFrame.Navigate(new GamePage(pt, et, level));
            //Close the overlay
            TopFrameManager.Instance.ClearOverlayFrame();
        }
    }
}
