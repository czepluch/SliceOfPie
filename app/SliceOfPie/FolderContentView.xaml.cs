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
    /// Interaction logic for the FolderContentView User Control.
    /// </summary>
    public partial class FolderContentView : UserControl {

        private IItemContainer _container; //backing field

        /// <summary>
        /// This is the IItemContainer which contains the currently shown content
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
        /// This event is fired when the button to create a Document is clicked
        /// </summary>
        public event RoutedEventHandler CreateDocumentButtonClicked;
        
        /// <summary>
        /// This event is fired when the button to create a Folder is clicked
        /// </summary>
        public event RoutedEventHandler CreateFolderButtonClicked;
        public event EventHandler<ListableItemEventArgs> ItemDoubleClicked;
        
        #endregion

        /// <summary>
        /// This UserControl shows a list of subfolders based on its ItemContainer property.
        /// </summary>
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

        /// <summary>
        /// This reloads the entire FolderListView based on the current ItemContainer
        /// </summary>
        private void ReloadItemContainerContents() {
            FolderListView.Items.Clear();
            foreach (Folder folder in ItemContainer.GetFolders()) { //Add folders first
                FolderListView.Items.Add(CreateListViewItem(folder));
            }
            foreach (Document document in ItemContainer.GetDocuments()) { //Then documents
                FolderListView.Items.Add(CreateListViewItem(document));
            }
        }

        /// <summary>
        /// This method creates a ListViewItem based on a given ListableItem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ListViewItem CreateListViewItem(IListableItem item) {
            StackPanel sp = new StackPanel() { Width = 50, Height = 50, Orientation = Orientation.Vertical, IsHitTestVisible = false };
            sp.Children.Add(new Image() { Source = ((item is Folder) ? IconFactory.FolderIcon : IconFactory.DocumentIcon), Width = 24, Height = 24 });
            sp.Children.Add(new TextBlock() { Text = item.Title, MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center });
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
        /// This method triggers the CreateDocumentButtonClicked event
        /// </summary>
        /// <param name="e">The event arguments</param>
        private void OnCreateDocumentButtonClicked(RoutedEventArgs e) {
            if (CreateDocumentButtonClicked != null) {
                //Invoke the delegates attached to this event
                CreateDocumentButtonClicked(this, e);
            }
        }

        /// <summary>
        /// This method triggers the CreateFolderButtonClicked event
        /// </summary>
        /// <param name="e">The event arguments</param>
        private void OnCreateFolderButtonClicked(RoutedEventArgs e) {
            if (CreateFolderButtonClicked != null) {
                //Invoke the delegates attached to this event
                CreateFolderButtonClicked(this, e);
            }
        }

        /// <summary>
        /// This method triggers the ItemDoubleClicked event
        /// </summary>
        /// <param name="e">The event arguments</param>
        private void OnItemDoubleClicked(ListableItemEventArgs e) {
            if (ItemDoubleClicked != null) {
                //Invoke the delegates attached to this event
                ItemDoubleClicked(this, e);
            }
        }

        #endregion
    }
}
