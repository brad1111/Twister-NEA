using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Nea_Prototype.Annotations;

namespace Nea_Prototype.Grid
{
    /// <summary>
    /// The non-view version of a grid item
    /// </summary>
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
        /// <summary>
        /// Tell the UI thread that a property has been changed and that it needs to update any
        /// data bound values
        /// </summary>
        /// <param name="propertyName">The name of the property that has been updated</param>
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Converts a relative string into a bitmap that is stored in sprite
        /// </summary>
        /// <param name="relativeLocation">The relative location where the bitmap file is stored</param>
        protected void SetupSprite(string relativeLocation)
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(relativeLocation, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            CachedBitmap cachedSrc = new CachedBitmap(src, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            sprite = cachedSrc;
        }
    }
}