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
    /// Interaction logic for ContentWrapper.xaml
    /// </summary>
    public partial class ContentWrapper : UserControl {
        
        public new object Content {
            set { _contentControl.Content = value; }
        }

        public ContentWrapper() {
            InitializeComponent();
        }

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
