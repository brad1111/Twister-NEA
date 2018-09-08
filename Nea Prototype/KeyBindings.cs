using System.Windows.Input;

namespace Nea_Prototype
{
    public static class KeyBindings
    {
        private static Key player1_up = Key.W;
        private static Key player1_down = Key.S;
        private static Key player1_left = Key.A;
        private static Key player1_right = Key.D;

        public static Key Player1_up { get => player1_up; set => player1_up = value; }
        public static Key Player1_down { get => player1_down; set => player1_down = value; }
        public static Key Player1_left { get => player1_left; set => player1_left = value; }
        public static Key Player1_right { get => player1_right; set => player1_right = value; }
    }
}