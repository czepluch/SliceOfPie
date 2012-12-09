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
    /// Interaction logic for DocumentExplorer.xaml
    /// This UserControl shows a hierarchical structure based on its Projects property.
    /// </summary>
    public partial class DocumentExplorer : UserControl {
        private IEnumerable<Project> _projects;

        #region Events

        /// <summary>
        /// This event is fired when an item is clicked with the left mouse button (when released)
        /// </summary>
        public event EventHandler<ListableItemEventArgs> ItemMouseLeftButtonUp;

        /// <summary>
        /// This event is fired when an item is clicked with the right mouse button (when released)
        /// </summary>
        public event EventHandler<ListableItemEventArgs> ItemMouseRightButtonUp;

        /// <summary>
        /// This event is fired when enter is pressed while an item is active.
        /// </summary>
        public event EventHandler<ListableItemEventArgs> ItemEnterKeyUp;

        #endregion

        /// <summary>
        /// This is the collection of projects which contains the currently shown content.
        /// Changing this will change what is shown (the top project will be selected in the Document Explorer)
        /// </summary>
        public IEnumerable<Project> Projects {
            get { return _projects; }
            set {
                _projects = value;
                RefreshProjects();
            }
        }

        /// <summary>
        /// Creates a new instance of a DocumentExplorer.
        /// For content to be shown, the Projects property must be set.
        /// </summary>
        public DocumentExplorer() {
            InitializeComponent();
        }


        /// <summary>
        /// This is a helper method for creating a  item in the document explorer.
        /// </summary>
        /// <param name="item">The IListableItem to associate with the TreeViewItem.</param>
        /// <returns>The created TreeViewItem.</returns>
        private TreeViewItem CreateTreeViewItem(IListableItem item) {
            TreeViewItem thisTreeViewItem = new TreeViewItem() { Tag = item };
            //StackPanel for image and text block
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal, IsHitTestVisible = false };
            //Create the image
            Image image = new Image() { Source = item.GetIcon(), Height = 15, Width = 15 };
            sp.Children.Add(image);
            //Create the text block
            TextBlock itemText = new TextBlock() { Text = item.Title, Margin = new Thickness(5, 0, 0, 0), IsHitTestVisible = false };
            sp.Children.Add(itemText);
            thisTreeViewItem.Header = sp;
            //set up event handlers
            thisTreeViewItem.MouseDoubleClick += new MouseButtonEventHandler((sender, e) => e.Handled = true); /*This User Control does not provide any double click behaviour - not even the default of TreeViewItems.
                                                                                                              * This is a design choice. We provide a MouseLeftButtonUp instead - having both would cause conflicting behaviour.
                                                                                                              * (The MouseEventArgs.Count property cannot be used as one would expect, since it's keyUp vs. 2 x keyDown).
                                                                                                              * Similar rejections of event bubbling in this TreeViewItem will be found in the methods below (TreeViewItem has a few unwanted default events)   */

            thisTreeViewItem.MouseLeftButtonUp += new MouseButtonEventHandler((sender, e) => {
                e.Handled = true;
                thisTreeViewItem.IsSelected = true;
                thisTreeViewItem.IsExpanded = true;
                OnItemMouseLeftButtonUp(new ListableItemEventArgs(item));
            });

            thisTreeViewItem.MouseRightButtonDown += new MouseButtonEventHandler((sender, e) => { e.Handled = true; thisTreeViewItem.IsSelected = true; });  //selected for visual feedback in line with our LeftMouseButtonDown.

            thisTreeViewItem.MouseRightButtonUp += new MouseButtonEventHandler((sender, e) => { e.Handled = true; OnItemMouseRightButtonUp(new ListableItemEventArgs(item)); });

            thisTreeViewItem.KeyUp += new KeyEventHandler((sender, e) => {
                if (e.Key.Equals(System.Windows.Input.Key.Enter)) {
                    e.Handled = true;
                    thisTreeViewItem.IsSelected = true;
                    thisTreeViewItem.IsExpanded = true;
                    OnItemEnterKeyUp(new ListableItemEventArgs(item));
                }
            });

            //recursive traversal of structure for Item Containers
            if (item is IItemContainer) {
                //First add folders
                foreach (Folder folder in (item as IItemContainer).GetFolders()) {
                    thisTreeViewItem.Items.Add(CreateTreeViewItem(folder));
                }
                //then documents
                foreach (Document document in (item as IItemContainer).GetDocuments()) {
                    thisTreeViewItem.Items.Add(CreateTreeViewItem(document));
                }
            }
            return thisTreeViewItem;
        }

        /// <summary>
        /// This method refreshes the document explorer.
        /// </summary>
        /// <param name="itemToOpen">The item to open, when the projects are reloaded.</param>
        private void RefreshProjects() {
            treeView.Items.Clear();
            if (Projects != null) {
                //Add each project
                foreach (Project project in Projects) {
                    TreeViewItem projectItem = CreateTreeViewItem(project);
                    treeView.Items.Add(projectItem);
                }
                TreeViewItem topProject = treeView.Items[0] as TreeViewItem;
                topProject.IsSelected = true;
            }
        }

        /// <summary>
        /// This method expands the DocumentExplorers items down to a given item.
        /// </summary>
        /// <param name="item">The item to be found. This item is also expanded if found (and selected).</param>
        /// <param name="callback">This function will be called on the item if found. Null is allowed if no additional action is wanted.</param>
        public void ExpandTo(IListableItem item, Action<IListableItem> callback) {
            foreach (TreeViewItem project in treeView.Items) {
                if (ExpandTo(project, item, callback)) return; //return when found
            }
        }

        /// <summary>
        /// This method expands the DocumentExplorers TreeViewItems from a start container down to a given item.
        /// </summary>
        /// <param name="containerItem">The starter container for the search.</param>
        /// <param name="searchItem">The item to be found. This item is also expanded if found (and selected).</param>
        /// <param name="callback">This function will be called on the item if found. Null is allowed if no additional action is wanted.</param>
        /// <returns>Returns true if the item was found.</returns>
        private bool ExpandTo(TreeViewItem containerItem, IListableItem searchItem, Action<IListableItem> callback) {
            IListableItem containerListable = containerItem.Tag as IListableItem;
            if (containerListable == searchItem) {
                containerItem.IsSelected = true;
                containerItem.IsExpanded = true;
                if (callback != null) callback(searchItem);
                //currentContextItem = searchItem;
                //Open(currentContextItem);
                return true;
            }
            else if (containerListable is IItemContainer) { //possibility that it's in a subitem
                foreach (TreeViewItem subItem in containerItem.Items) { //repeat search for each subitem
                    if (ExpandTo(subItem, searchItem, callback)) {
                        containerItem.IsExpanded = true;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Shows a specified context menu for the currently selected item.
        /// </summary>
        /// <param name="contextMenu">The Context Menu to be shown.</param>
        public void ShowContextMenuForSelected(ContextMenu contextMenu) {
            if (treeView.SelectedItem != null) {
                (treeView.SelectedItem as TreeViewItem).ContextMenu = contextMenu;
                contextMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// Calls a callback on the currently selected item.
        /// </summary>
        /// <param name="callback">This function will be called on the currently selected item.</param>
        public void CallbackSelected(Action<IListableItem> callback) {
            if (treeView.SelectedItem != null) {
                callback((treeView.SelectedItem as TreeViewItem).Tag as IListableItem);
            }
        }

        #region Event triggers

        /// <summary>
        /// This method triggers the ItemMouseLeftButtonUp event
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void OnItemMouseLeftButtonUp(ListableItemEventArgs e) {
            if (ItemMouseLeftButtonUp != null) {
                ItemMouseLeftButtonUp(this, e);
            }
        }

        /// <summary>
        /// This method triggers the ItemMouseRightButtonUp event
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void OnItemMouseRightButtonUp(ListableItemEventArgs e) {
            if (ItemMouseRightButtonUp != null) {
                ItemMouseRightButtonUp(this, e);
            }
        }

        /// <summary>
        /// This method triggers the ItemEnterKeyUp event
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void OnItemEnterKeyUp(ListableItemEventArgs e) {
            if (ItemEnterKeyUp != null) {
                ItemEnterKeyUp(this, e);
            }
        }

        #endregion
    }
}
