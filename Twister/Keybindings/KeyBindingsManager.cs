using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Twister.Keybindings
{
    /// <summary>
    /// Contains the defaults and variables for keybindings
    /// </summary>
    public class KeyBindingsManager
    {
        private KeyBindingsManager()
        {
            
        }

        public static KeyBindingsManager Instance { get; } = LoadKeybindings();


        private const string KeyBindingsLocation = "keybindings.json";

        public static KeyBindingsManager LoadKeybindings()
        {
            if (!File.Exists(KeyBindingsLocation))
            {
                //Create the keybindings file
                SaveKeybindings(new KeyBindingsManager());
            }
            //Now read the file
            using (StreamReader sr = new StreamReader(KeyBindingsLocation))
            {
                string kbJson = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<KeyBindingsManager>(kbJson);
            }
        }

        public static void SaveKeybindings(KeyBindingsManager kbManager)
        {
            //Saves the keybindings
            string kbJson = JsonConvert.SerializeObject(kbManager);
                
            using (StreamWriter sw = new StreamWriter(KeyBindingsLocation))
            {
                sw.Write(kbJson);
            }
        }

        #region Player 1 keybindings

        public Key Player1_up { get; set; } = Key.W;
        public Key Player1_down { get; set; } = Key.S;
        public Key Player1_left { get; set; } = Key.A;
        public Key Player1_right { get; set; } = Key.D;

        #endregion
        
        #region Player 2 keybindings

        public Key Player2_up { get; set; } = Key.Up;
        public Key Player2_down { get; set; } = Key.Down;
        public Key Player2_left { get; set; } = Key.Left;
        public Key Player2_right { get; set; } = Key.Right;

        #endregion

        #region Overlay keybindings

        public ModifierKeys DebugOverlayModifier { get; set; } = ModifierKeys.Shift;

        public Key DebugOverlayKey { get; set; } = Key.Tab;

        public Key PauseMenuKey { get; set; } = Key.Escape;

        public Key PauseGameKey { get; set; } = Key.Pause;

        #endregion
    }
}