using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Nea_Prototype.Controls
{
    public class RegexTextBox : TextBox
    {
        private readonly Brush Gray = new SolidColorBrush(Colors.Gray);
        private readonly Brush Green = new SolidColorBrush(Colors.Green);
        private readonly Brush Red = new SolidColorBrush(Colors.Red);

        #region DependencyProperty Watermark
 
        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(TextBox),
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));
 
        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
 
        #endregion

        #region DependencyProperty Watermark
 
        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty RegexProperty =
            DependencyProperty.Register("RegularExpression", typeof(string), typeof(TextBox),
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));
 
        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public string RegularExpression
        {
            //If the regex isn't empty return it, otherwise everything is allowed
            get { return (string)GetValue(RegexProperty) ?? ".*"; }
            set { SetValue(RegexProperty, value); }
        }
 
        #endregion

        public override void EndInit()
        {
            base.EndInit();
            Text = Watermark;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            int caretLocation = SelectionStart;
            if (e.Key == Key.Back && Text == "")
            {
                Text = Watermark;
            }
            else if (e.Key != Key.Back && Text.EndsWith(Watermark))
            {
                Text = Text.Remove(Text.Length - Watermark.Length, Watermark.Length);
                if (caretLocation >= Text.Length)
                    caretLocation = Text.Length;
                SelectionStart = caretLocation;
            }

            base.OnKeyUp(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (Text == Watermark)
            {
                Foreground = Gray;
            }
            else if (Regex.IsMatch(Text, RegularExpression))
            {
                Foreground = Green;
            }
            else
            {
                Foreground = Red;
            }
        }
    }
}