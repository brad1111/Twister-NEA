using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Nea_Prototype.Annotations;

namespace Nea_Prototype.Keybindings
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