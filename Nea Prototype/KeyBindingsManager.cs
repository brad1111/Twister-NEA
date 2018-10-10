using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Nea_Prototype
{
    /// <summary>
    /// Contains the defaults and variables for keybindings
    /// </summary>
    public class KeyBindingsManager
    {
        private KeyBindingsManager()
        {
            
        }

        public static KeyBindingsManager KeyBindings { get; } = LoadKeybindings();


        private const string KeyBindingsLocation = "keybindings.json";

        public static KeyBindingsManager LoadKeybindings()
        {
            if (!File.Exists(KeyBindingsLocation))
            {
                //Create the keybindings file
                KeyBindingsManager kbManager = new KeyBindingsManager();
                string kbJson = JsonConvert.SerializeObject(kbManager);
                
                using (StreamWriter sw = new StreamWriter(KeyBindingsLocation))
                {
                    sw.Write(kbJson);
                }
            }
            //Now read the file
            using (StreamReader sr = new StreamReader(KeyBindingsLocation))
            {
                string kbJson = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<KeyBindingsManager>(kbJson);
            }
        }



        #region Player 1 Keybindings
        /// <summary>
        /// 
        /// </summary>
        private Key player1_up = Key.W;
        private Key player1_down = Key.S;
        private Key player1_left = Key.A;
        private Key player1_right = Key.D;
        #endregion

        #region Player 2 Keybindings
        private Key player2_up = Key.Up;
        private Key player2_down = Key.Down;
        private Key player2_left = Key.Left;
        private Key player2_right = Key.Right;
        #endregion

        #region Player 1 encapsulation

        public Key Player1_up { get => player1_up; set => player1_up = value; }
        public Key Player1_down { get => player1_down; set => player1_down = value; }
        public Key Player1_left { get => player1_left; set => player1_left = value; }
        public Key Player1_right { get => player1_right; set => player1_right = value; }

        #endregion
        
        #region Player 2 encapsulation
        public Key Player2_up { get => player2_up; set => player2_up = value; }
        public Key Player2_down { get => player2_down; set => player2_down = value; }
        public Key Player2_left { get => player2_left; set => player2_left = value; }
        public Key Player2_right { get => player2_right; set => player2_right = value; }

        

        #endregion

        #region Overlay bindings

        private ModifierKeys debugOverlayModifier = ModifierKeys.Shift;
        private Key debugOverlayKey = Key.Tab;

        #endregion

        #region Overlay encapsulation

        public ModifierKeys DebugOverlayModifier{ get => debugOverlayModifier; set => debugOverlayModifier = value; }

        public Key DebugOverlayKey { get => debugOverlayKey; set => debugOverlayKey = value; }

        #endregion
    }
}