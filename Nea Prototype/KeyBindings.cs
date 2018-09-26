using System.Windows.Input;

namespace Nea_Prototype
{
    public static class KeyBindings
    {
        #region Player 1 Keybindings
        /// <summary>
        /// 
        /// </summary>
        private static Key player1_up = Key.W;
        private static Key player1_down = Key.S;
        private static Key player1_left = Key.A;
        private static Key player1_right = Key.D;
        #endregion

        #region Player 2 Keybindings
        private static Key player2_up = Key.Up;
        private static Key player2_down = Key.Down;
        private static Key player2_left = Key.Left;
        private static Key player2_right = Key.Right;
        #endregion

        #region Player 1 encapsulation

        public static Key Player1_up { get => player1_up; set => player1_up = value; }
        public static Key Player1_down { get => player1_down; set => player1_down = value; }
        public static Key Player1_left { get => player1_left; set => player1_left = value; }
        public static Key Player1_right { get => player1_right; set => player1_right = value; }

        #endregion
        
        #region Player 2 encapsulation
        public static Key Player2_up { get => player2_up; set => player2_up = value; }
        public static Key Player2_down { get => player2_down; set => player2_down = value; }
        public static Key Player2_left { get => player2_left; set => player2_left = value; }
        public static Key Player2_right { get => player2_right; set => player2_right = value; }

        

        #endregion

        #region Overlay bindings

        private static ModifierKeys debugOverlayModifier = ModifierKeys.Shift;
        private static Key debugOverlayKey = Key.Tab;

        #endregion

        #region Overlay encapsulation

        public static ModifierKeys DebugOverlayModifier{ get => debugOverlayModifier; set => debugOverlayModifier = value; }

        public static Key DebugOverlayKey { get => debugOverlayKey; set => debugOverlayKey = value; }

        #endregion
    }
}