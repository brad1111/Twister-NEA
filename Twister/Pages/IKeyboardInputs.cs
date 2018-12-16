using System.Windows.Input;

namespace Twister.Pages
{
    public interface IKeyboardInputs
    {
        void Page_KeyDown(object sender, KeyEventArgs e);
    }
}