using System.Windows.Input;

namespace Twister.Keybindings
{
    public class KeybindingProperty
    {
        public KeybindingProperty(Key key, string bindingName)
        {
            this.key = key;
            this.BindingName = bindingName;
        }

        private Key key;
        public Key Key
        {
            get => key;
            set
            {
                key = value;
            }
        }
        public string KeyName => Key.ToString();
        public string BindingName { get; private set; }
    }
}