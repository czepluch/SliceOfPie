using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SliceOfPie.Client {
    /// <summary>
    /// A ContentWrapper can wrap ui content and provide it with a menu bar.
    /// </summary>
    public partial class ContentWrapper : UserControl {
        
        /// <summary>
        /// This is the content of the ContentWrapper
        /// </summary>
        public new object Content {
            set { _contentControl.Content = value; }
        }

        /// <summary>
        /// Creates a new ContentWrapper
        /// </summary>
        public ContentWrapper() {
            InitializeComponent();
        }

        /// <summary>
        /// Adds a menu button to the Content Wrappers menu bar.
        /// Note that this version of the Content Wrapper does not have room for infinite buttons. The precise amount of buttons available depends on the length of their text.
        /// </summary>
        /// <param name="text">The text on the button</param>
        /// <param name="relativeImagePath">The path to the image on the button. From the root of this assembly.</param>
        /// <param name="clickHandler">The click handler to assign to this button</param>
        public void addMenuButton(string text, string relativeImagePath, RoutedEventHandler clickHandler) {
            //Create the button and set the click handler
            Button button = new Button() { Margin = new Thickness(10, 5, 0, 5) };
            button.Click += new RoutedEventHandler(clickHandler);
            //Create a stackpanel to hold image and text and add it to button
            StackPanel sp = new StackPanel() { Orientation = Orientation.Vertical };
            button.Content = sp;
            //create and add image to stackpanel
            Image image = new Image() { Width = 30, Height = 30, Source = ImageUtil.CreateBitmapImage(relativeImagePath) };
            sp.Children.Add(image);
            //create and add label to stackpanel
            Label label = new Label() { Padding = new Thickness(0), Content = text };
            sp.Children.Add(label);
            //Setup done - add the button to the menubar
            menu.Children.Add(button);
        }
    }
}
