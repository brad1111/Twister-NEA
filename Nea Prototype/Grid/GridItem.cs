using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Nea_Prototype.Annotations;

namespace Nea_Prototype.Grid
{
    public class GridItem : INotifyPropertyChanged
    {
        protected ImageSource sprite;
        protected Position location;


        public ImageSource Sprite
        {
            get => sprite;
        }

        public Position Position
        {
            get => location;
            set
            {
                location = value;
                OnPropertyChanged("Position");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}