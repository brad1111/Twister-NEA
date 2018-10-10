using System.Windows.Input;

namespace Nea_Prototype.Keybindings
{
    public class KeybindingProperty
    {
        public KeybindingProperty(Key key, string bindingName)
        {
            this.Key = key;
            this.BindingName = bindingName;
        }
        public Key Key { get; private set; }
        public string KeyName => Key.ToString();
        public string BindingName { get; private set; }
    }
}