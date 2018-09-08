using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nea_Prototype.Grid
{
    public interface IGridItem
    {
        ImageSource GetSprite();
        Position GetPosition();
    }
}