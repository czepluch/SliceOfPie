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

namespace SliceOfPie {
    /// <summary>
    /// Interaction logic for FolderContentView.xaml.
    /// This is a template/encapsulating class. Event handling need to be set by the initatiator.
    /// </summary>
    public partial class FolderContentView : UserControl {

        /// <summary>
        /// This UserControl shows a list of subfolders in a specified parent container.
        /// </summary>
        /// <param name="parentContainer">The parent whose subfolders will be shown</param>
        /// <param name="doubleClickAction">The MouseDoubleClick handler for each subfolder</param>
        public FolderContentView(IItemContainer parentContainer, MouseButtonEventHandler doubleClickAction) {
            InitializeComponent();
            //Add items in the provied parent containeer to the list and attach MouseDoubleClick handler.

            foreach (Folder folder in parentContainer.GetFolders()) { //Add folders first
                StackPanel sp = new StackPanel() { Width = 50, Height = 50, Orientation = Orientation.Vertical, IsHitTestVisible = false };
                sp.Children.Add(new Image() { Source = IconFactory.FolderIcon, Width = 24, Height = 24 });
                sp.Children.Add(new TextBlock() { Text = folder.Title, MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center });
                ListViewItem listViewItem = new ListViewItem() { Margin = new Thickness(2) };
                listViewItem.Content = sp;
                listViewItem.Tag = folder;
                listViewItem.MouseDoubleClick += doubleClickAction;
                FolderListView.Items.Add(listViewItem);
            }
            foreach (Document document in parentContainer.GetDocuments()) { //Then documents
                StackPanel sp = new StackPanel() { Width = 50, Height = 50, Orientation = Orientation.Vertical };
                sp.Children.Add(new Image() { Source = IconFactory.DocumentIcon, Width = 24, Height = 24 });
                sp.Children.Add(new TextBlock() { Text = document.Title, MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center });
                ListViewItem listViewItem = new ListViewItem() { Margin = new Thickness(2) };
                listViewItem.Content = sp;
                listViewItem.Tag = document;
                listViewItem.MouseDoubleClick += doubleClickAction;
                FolderListView.Items.Add(listViewItem);
            } 
        }

        /// <summary>
        /// This methods adds an event handler to the Create Document Button
        /// </summary>
        /// <param name="handler">The event handler to add</param>
        public void setCreateDocumentHandler(RoutedEventHandler handler) {
            CreateDocumentButton.Click+=new RoutedEventHandler(handler);
        }

        /// <summary>
        /// This methods adds an event handler to the Create Folder Button
        /// </summary>
        /// <param name="handler">The event handler to add</param>
        public void setCreateFolderHandler(RoutedEventHandler handler) {
            CreateFolderButton.Click += new RoutedEventHandler(handler);
        }
    }
}
