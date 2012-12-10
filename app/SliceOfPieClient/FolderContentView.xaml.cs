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
    /// Interaction logic for the FolderContentView User Control.
    /// This UserControl shows a list of subfolders based on its ItemContainer property.
    /// </summary>
    public partial class FolderContentView : UserControl {

        private IItemContainer _container; //backing field

        /// <summary>
        /// This is the IItemContainer which contains the currently shown content
        /// Changing this will change what is shown.
        /// </summary>
        public IItemContainer ItemContainer {
            get { return _container; }
            set {
                _container = value;
                ReloadItemContainerContents();
            }
        }

        #region Events

        /// <summary>
        /// This event is fired when an item is double clicked
        /// </summary>
        public event EventHandler<ListableItemEventArgs> ItemDoubleClicked;
        
        #endregion

        /// <summary>
        /// Creates a new instance of a FolderContentView.
        /// For content to be shown, the ItemContainer property must be set.
        /// </summary>
        public FolderContentView() {
            InitializeComponent();
        }

        /// <summary>
        /// This reloads the entire FolderListView based on the current ItemContainer
        /// </summary>
        private void ReloadItemContainerContents() {
            folderListView.Items.Clear();
            foreach (Folder folder in ItemContainer.GetFolders()) { //Add folders first
                folderListView.Items.Add(CreateListViewItem(folder));
            }
            foreach (Document document in ItemContainer.GetDocuments()) { //Then documents
                folderListView.Items.Add(CreateListViewItem(document));
            }
        }

        /// <summary>
        /// This method creates a ListViewItem based on a given IListableItem
        /// </summary>
        /// <param name="item">The item to generate a ListViewItem for.</param>
        /// <returns>The created ListViewItem.</returns>
        private ListViewItem CreateListViewItem(IListableItem item) {
            StackPanel sp = new StackPanel() { Width = 50, Height = 50, Orientation = Orientation.Vertical, IsHitTestVisible = false };
            sp.Children.Add(new Image() { Source = item.GetIcon(), Width = 24, Height = 24 });
            sp.Children.Add(new Label() { Content = item.Title, MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center });
            ListViewItem listViewItem = new ListViewItem() { Margin = new Thickness(2) };
            listViewItem.Content = sp;
            listViewItem.Tag = item;
            listViewItem.MouseDoubleClick += new MouseButtonEventHandler(
                (sender, e) => OnItemDoubleClicked(new ListableItemEventArgs((sender as ListViewItem).Tag as IListableItem)) //fire own event
            );
            return listViewItem;
        }

        #region Event triggers

        /// <summary>
        /// This method triggers the ItemDoubleClicked event
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void OnItemDoubleClicked(ListableItemEventArgs e) {
            if (ItemDoubleClicked != null) {
                //Invoke the delegates attached to this event
                ItemDoubleClicked(this, e);
            }
        }

        #endregion
    }
}
