﻿using System;
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
    /// </summary>
    public partial class DocumentExplorer : UserControl {
        private IEnumerable<Project> _projects;

        public event EventHandler<ListableItemEventArgs> ItemMouseLeftButtonUp, ItemMouseRightButtonUp, ItemEnterKeyUp;

        public IEnumerable<Project> Projects {
            get { return _projects; }
            set {
                _projects = value;
                RefreshProjects();
            }
        }

        public DocumentExplorer() {
            InitializeComponent();
        }

        private void OnItemMouseLeftButtonUp(ListableItemEventArgs e) {
            if (ItemMouseLeftButtonUp != null) {
                ItemMouseLeftButtonUp(this, e);
            }
        }

        private void OnItemMouseRightButtonUp(ListableItemEventArgs e) {
            if (ItemMouseRightButtonUp != null) {
                ItemMouseRightButtonUp(this, e);
            }
        }

        private void OnItemEnterKeyUp(ListableItemEventArgs e) {
            if (ItemEnterKeyUp != null) {
                ItemEnterKeyUp(this, e);
            }
        }


        /// <summary>
        /// This is a helper method for creating an item in the document explorer.
        /// </summary>
        /// <param name="text">The text to be shown in the item</param>
        /// <param name="item">The type of the item. Can be either "project", "folder", or "document"</param>
        /// <returns></returns>
        private TreeViewItem CreateDocumentExplorerItem(IListableItem item) {
            TreeViewItem thisTreeViewItem = new TreeViewItem() { Tag = item };
            //StackPanel for image and text block
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal, IsHitTestVisible = false };
            //Create the image
            BitmapImage icon;
            string text;
            if (item is Project) {
                icon = IconFactory.ProjectIcon;
                text = (item as Project).Title;
            }
            else if (item is Folder) {
                icon = IconFactory.FolderIcon;
                text = (item as Folder).Title;
            }
            else {
                icon = IconFactory.DocumentIcon;
                text = (item as Document).Title;
            }
            Image image = new Image() { Source = icon, Height = 15, Width = 15 }; /* note that IsHitTestVisible=false disables event handling for this element -
                                                                                                             * fallback on the general treeview handling (rightclick for the context menu).  */
            sp.Children.Add(image);
            //Create the text block
            TextBlock itemText = new TextBlock() { Text = text, Margin = new Thickness(5, 0, 0, 0), IsHitTestVisible = false };
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
                    thisTreeViewItem.Items.Add(CreateDocumentExplorerItem(folder));
                }
                //then documents
                foreach (Document document in (item as IItemContainer).GetDocuments()) {
                    thisTreeViewItem.Items.Add(CreateDocumentExplorerItem(document));
                }
            }
            return thisTreeViewItem;
        }

        /// <summary>
        /// This method refreshes the document explorer.
        /// </summary>
        /// <param name="itemToOpen">The item to open, when the projects are reloaded</param>
        private void RefreshProjects() {
            TreeView.Items.Clear();
            if (Projects != null) {
                //Add each project
                foreach (Project project in Projects) {
                    TreeViewItem projectItem = CreateDocumentExplorerItem(project);
                    TreeView.Items.Add(projectItem);
                }
                TreeViewItem topProject = TreeView.Items[0] as TreeViewItem;
                topProject.IsSelected = true;
            }
        }

        /// <summary>
        /// This method expands the DocumentExplorers items from a start container down to a given item.
        /// </summary>
        /// <param name="container">The starter container for the search</param>
        /// <param name="item">The item to be found. This item is also expanded if found </param>
        /// <returns>Returns true if the item was found</returns>
        public void ExpandTo(IListableItem item, Action<IListableItem> callback) {
            foreach (TreeViewItem project in TreeView.Items) {
                if (ExpandTo(project, item, callback)) return; //return when found
            }
        }

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

        public void ShowContextMenuForSelected(ContextMenu contextMenu) {
            if (TreeView.SelectedItem != null) {
                (TreeView.SelectedItem as TreeViewItem).ContextMenu = contextMenu;
                contextMenu.IsOpen = true;
            }
        }

        public void CallbackSelected(Action<IListableItem> callback) {
            if (TreeView.SelectedItem != null) {
                callback((TreeView.SelectedItem as TreeView).Tag as IListableItem);
            }
        }
    }
}
