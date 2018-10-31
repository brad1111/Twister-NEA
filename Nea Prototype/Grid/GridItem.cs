using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common.Grid;
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

        public virtual ImageSource Sprite
        {
            get => sprite;
        }

        public Position Position
        {
            get => location;
            set
            {
                location = value;                
            }
        }

        

        /// <summary>
        /// Converts a relative string into a bitmap that is stored in sprite
        /// </summary>
        /// <param name="relativeLocation">The relative location where the bitmap file is stored</param>
        protected ImageSource SetupSprite(string relativeLocation)
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(relativeLocation, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            CachedBitmap cachedSrc = new CachedBitmap(src, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            return cachedSrc;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}