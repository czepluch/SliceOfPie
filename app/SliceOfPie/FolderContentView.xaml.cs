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

        private IItemContainer _container; //backing field

        public event RoutedEventHandler CreateDocumentButtonClicked, CreateFolderButtonClicked;
        public event EventHandler<ListableItemEventArgs> ItemDoubleClicked;

        public IItemContainer ItemContainer {
            get { return _container; }
            set {
                _container = value; 
                ReloadItemContainerContents();
            }
        }

        /// <summary>
        /// This UserControl shows a list of subfolders in a specified parent container.
        /// </summary>
        /// <param name="parentContainer">The parent whose subfolders will be shown</param>
        /// <param name="doubleClickAction">The MouseDoubleClick handler for each subfolder</param>
        public FolderContentView() {
            InitializeComponent();
            //On button clicks
            CreateDocumentButton.Click += new RoutedEventHandler(
                (sender, e) => OnCreateDocumentButtonClicked(e) //fire own event
            );
            CreateFolderButton.Click += new RoutedEventHandler(
                (sender, e) => OnCreateFolderButtonClicked(e) //fire own event
            );
            
        }

        private void ReloadItemContainerContents() {
            FolderListView.Items.Clear();
            foreach (Folder folder in ItemContainer.GetFolders()) { //Add folders first
                FolderListView.Items.Add(CreateListViewItem(folder));
            }
            foreach (Document document in ItemContainer.GetDocuments()) { //Then documents
                FolderListView.Items.Add(CreateListViewItem(document));
            }
        }

        private ListViewItem CreateListViewItem(ListableItem item) {
            StackPanel sp = new StackPanel() { Width = 50, Height = 50, Orientation = Orientation.Vertical, IsHitTestVisible = false };
            sp.Children.Add(new Image() { Source = ((item is Folder) ? IconFactory.FolderIcon : IconFactory.DocumentIcon), Width = 24, Height = 24 });
            sp.Children.Add(new TextBlock() { Text = item.Title, MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center });
            ListViewItem listViewItem = new ListViewItem() { Margin = new Thickness(2) };
            listViewItem.Content = sp;
            listViewItem.Tag = item;
            listViewItem.MouseDoubleClick += new MouseButtonEventHandler(
                (sender, e) => OnItemDoubleClicked(new ListableItemEventArgs((sender as ListViewItem).Tag as ListableItem)) //fire own event
            );
            return listViewItem;
        }

        #region Event triggers

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
