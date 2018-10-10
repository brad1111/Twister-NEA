using System.Collections.Generic;

namespace Nea_Prototype.Keybindings
{
    public static class KeybindingsProperties
    {
        public static List<KeybindingProperty> Properties { get; } = new List<KeybindingProperty>()
        {
            new KeybindingProperty(KeyBindingsManager.KeyBindings.Player1_up,    "Player 1 Up"),
            new KeybindingProperty(KeyBindingsManager.KeyBindings.Player1_down,  "Player 1 Down"),
            new KeybindingProperty(KeyBindingsManager.KeyBindings.Player1_left,  "Player 1 Left"),
            new KeybindingProperty(KeyBindingsManager.KeyBindings.Player1_right, "Player 1 Right"),
            
            new KeybindingProperty(KeyBindingsManager.KeyBindings.Player2_up,    "Player 2 Up"),
            new KeybindingProperty(KeyBindingsManager.KeyBindings.Player2_down,  "Player 2 Down"),
            new KeybindingProperty(KeyBindingsManager.KeyBindings.Player2_left,  "Player 2 Left"),
            new KeybindingProperty(KeyBindingsManager.KeyBindings.Player2_right, "Player 2 Right"),

            new KeybindingProperty(KeyBindingsManager.KeyBindings.DebugOverlayKey,    "Debug overlay key"),
        };
    }
}