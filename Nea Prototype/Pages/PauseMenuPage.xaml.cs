﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nea_Prototype.Grid;

namespace Nea_Prototype.Pages
{
    /// <summary>
    /// Interaction logic for PauseMenuPage.xaml
    /// </summary>
    public partial class PauseMenuPage : Page, IKeyboardInputs
    {
        public PauseMenuPage()
        {
            InitializeComponent();
        }

        public void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //Keyboard input not needed
        }

        private void BtnResume_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.ClearOverlayFrame();
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                GamePage page = (GamePage) TopFrameManager.Instance.MainFrame.Content;
                page.StartTimers();
            }
        }

        private void BtnSettings_OnClick(object sender, RoutedEventArgs e)
        {
            TopFrameManager.Instance.OverlayFrame.Navigate(new SettingsPage());
        }

        private void BtnExit_OnClick(object sender, RoutedEventArgs e)
        {
            GameGridManager.Clear();
            //Stop anything going in game if its there
            if (TopFrameManager.Instance.MainFrame.Content is GamePage)
            {
                GamePage page = (GamePage) TopFrameManager.Instance.MainFrame.Content;
                page.EndGame();
            }
            while (TopFrameManager.Instance.MainFrame.CanGoBack)
            {
                TopFrameManager.Instance.MainFrame.GoBack();
            }
            TopFrameManager.Instance.ClearOverlayFrame();
        }
    }
}
