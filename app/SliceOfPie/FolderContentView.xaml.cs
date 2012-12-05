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

        public event RoutedEventHandler CreateDocumentButtonClicked, CreateFolderButtonClicked;
        public event EventHandler<ListableItemEventArgs> ItemDoubleClicked;

        /// <summary>
        /// This UserControl shows a list of subfolders in a specified parent container.
        /// </summary>
        /// <param name="parentContainer">The parent whose subfolders will be shown</param>
        /// <param name="doubleClickAction">The MouseDoubleClick handler for each subfolder</param>
        public FolderContentView(IItemContainer parentContainer) {
            InitializeComponent();
            //On button clicks
            CreateDocumentButton.Click += new RoutedEventHandler(
                (sender, e) => OnCreateDocumentButtonClicked(e) //fire own event
            );
            CreateFolderButton.Click += new RoutedEventHandler(
                (sender, e) => OnCreateFolderButtonClicked(e) //fire own event
            );
            //Add items in the provied parent containeer to the list and attach MouseDoubleClick handler.

            foreach (Folder folder in parentContainer.GetFolders()) { //Add folders first
                StackPanel sp = new StackPanel() { Width = 50, Height = 50, Orientation = Orientation.Vertical, IsHitTestVisible = false };
                sp.Children.Add(new Image() { Source = IconFactory.FolderIcon, Width = 24, Height = 24 });
                sp.Children.Add(new TextBlock() { Text = folder.Title, MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center });
                ListViewItem listViewItem = new ListViewItem() { Margin = new Thickness(2) };
                listViewItem.Content = sp;
                listViewItem.Tag = folder;
                listViewItem.MouseDoubleClick += new MouseButtonEventHandler(
                    (sender, e) => OnItemDoubleClicked(new ListableItemEventArgs((sender as ListViewItem).Tag as ListableItem)) //fire own event
                );
                FolderListView.Items.Add(listViewItem);
            }
            foreach (Document document in parentContainer.GetDocuments()) { //Then documents
                StackPanel sp = new StackPanel() { Width = 50, Height = 50, Orientation = Orientation.Vertical };
                sp.Children.Add(new Image() { Source = IconFactory.DocumentIcon, Width = 24, Height = 24 });
                sp.Children.Add(new TextBlock() { Text = document.Title, MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center });
                ListViewItem listViewItem = new ListViewItem() { Margin = new Thickness(2) };
                listViewItem.Content = sp;
                listViewItem.Tag = document;
                listViewItem.MouseDoubleClick += new MouseButtonEventHandler(
                    (sender, e) => OnItemDoubleClicked(new ListableItemEventArgs((sender as ListViewItem).Tag as ListableItem)) //fire own event
                );
                FolderListView.Items.Add(listViewItem);
            }
        }

        #region event triggers

        private void OnCreateDocumentButtonClicked(RoutedEventArgs e) {
            if (CreateDocumentButtonClicked != null) {
                //Invoke the delegates attached to this event
                CreateDocumentButtonClicked(this, e);
            }
        }

        private void OnCreateFolderButtonClicked(RoutedEventArgs e) {
            if (CreateFolderButtonClicked != null) {
                //Invoke the delegates attached to this event
                CreateFolderButtonClicked(this, e);
            }
        }

        private void OnItemDoubleClicked(ListableItemEventArgs e) {
            if (ItemDoubleClicked != null) {
                //Invoke the delegates attached to this event
                ItemDoubleClicked(this, e);
            }
        }

        #endregion
    }
}
