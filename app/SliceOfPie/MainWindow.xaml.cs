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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static ContextMenu projectContextMenu, folderContextMenu, documentContextMenu;
        private static BitmapImage projectIcon, folderIcon, documentIcon;
        private static Controller controller;

        public MainWindow() {
            controller = Controller.Instance;

            InitializeComponent();
            InitializeDocumentExplorer();
            RefreshDocumentExplorer();
            GenerateContent((DocumentExplorer.Items[0] as TreeViewItem).Tag as ListableItem); //Note there's always at least one project
        }

        /// <summary>
        /// Sets up the DocumentExplorer's icons and context menus
        /// </summary>
        private void InitializeDocumentExplorer() {
            //Setup icons
            projectIcon = new BitmapImage();
            projectIcon.BeginInit();
            projectIcon.UriSource = new Uri("pack://application:,,,/Icons/project-icon.bmp");
            projectIcon.EndInit();

            folderIcon = new BitmapImage();
            folderIcon.BeginInit();
            folderIcon.UriSource = new Uri("pack://application:,,,/Icons/folder-icon.bmp");
            folderIcon.EndInit();

            documentIcon = new BitmapImage();
            documentIcon.BeginInit();
            documentIcon.UriSource = new Uri("pack://application:,,,/Icons/document-icon.bmp");
            documentIcon.EndInit();

            //Setup Project Context Menu for the Document Explorer
            projectContextMenu = new ContextMenu();
            projectContextMenu.Items.Add(new MenuItem() { Header = "Share project" });
            MenuItem projectMenuItem1 = new MenuItem() { Header = "Open project folder" };
            projectMenuItem1.Click += new RoutedEventHandler(generateContentClickEvent);
            projectContextMenu.Items.Add(projectMenuItem1);
            projectContextMenu.Items.Add(new MenuItem() { Header = "Add folder" });
            projectContextMenu.Items.Add(new MenuItem() { Header = "Add document" });

            //Setup Folder Context Menu for the Document Explorer
            folderContextMenu = new ContextMenu();
            MenuItem folderMenuItem1 = new MenuItem() { Header = "Open project folder" };
            folderMenuItem1.Click += new RoutedEventHandler(generateContentClickEvent);
            folderContextMenu.Items.Add(folderMenuItem1);
            folderContextMenu.Items.Add(new MenuItem() { Header = "Add folder" });
            folderContextMenu.Items.Add(new MenuItem() { Header = "Add document" });

            //Setup Document Context Menu for the Document Explorer
            documentContextMenu = new ContextMenu();
            MenuItem documentMenuItem1 = new MenuItem() { Header = "Edit document" };
            documentMenuItem1.Click += new RoutedEventHandler(generateContentClickEvent);
            documentContextMenu.Items.Add(documentMenuItem1);
        }

        /// <summary>
        /// A default event handler for changing the main content due to a click
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void generateContentClickEvent(object sender, RoutedEventArgs e) {
            GenerateContent((DocumentExplorer.SelectedItem as TreeViewItem).Tag as ListableItem);
        }

        

        /// <summary>
        /// This generates and shows a context menu for the document explorer at runtime
        /// </summary>
        /// <param name="item">The item which was clicked on the document explorer</param>
        private void generateContextMenu(ListableItem item) {
            if (item is Project) {
                DocumentExplorer.ContextMenu = projectContextMenu;
            } else if (item is Folder) {
                DocumentExplorer.ContextMenu = folderContextMenu;
            } else {
                DocumentExplorer.ContextMenu = documentContextMenu;
            }
            DocumentExplorer.ContextMenu.IsOpen = true;
        }

      

        /// <summary>
        /// This method is supposed to refresh the document explorer.
        /// It exists as a placeholder and testing method untill the model/controller allows local file traversal.
        /// </summary>
        private void RefreshDocumentExplorer() {
            foreach (Project project in controller.GetProjects()) {
                TreeViewItem myProject = createDocumentExplorerItem(project);
                AddProjectToDocExplorer(myProject);
            }
        }

        #region Helper methods for the Document Explorer

        /// <summary>
        /// Adds a new project to the root of the document explorer
        /// </summary>
        /// <param name="project">The project to add</param>
        private void AddProjectToDocExplorer(TreeViewItem project) {
            DocumentExplorer.Items.Add(project);
        }

        /// <summary>
        /// Adds a subitem to the existing item
        /// </summary>
        /// <param name="existingItem">The existing item</param>
        /// <param name="subItem">The subitem to add</param>
        private void AddSubItemToDocExplorer(TreeViewItem existingItem, TreeViewItem subItem) {
            existingItem.Items.Add(subItem);
        }

        /// <summary>
        /// Adds a "sibling" (item at the same level) to the existing item
        /// </summary>
        /// <param name="existingItem">The existing item</param>
        /// <param name="sibling">The sibling item to add</param>
        private void AddSiblingItemToDocExplorer(TreeViewItem existingItem, TreeViewItem sibling) {
            if (existingItem.Parent is TreeViewItem) {
                (existingItem.Parent as TreeViewItem).Items.Add(sibling);
            } else { //must be at root
                documentContextMenu.Items.Add(sibling);
            }
        }

        /// <summary>
        /// This is a helper method for creating an item in the document explorer.
        /// </summary>
        /// <param name="text">The text to be shown in the item</param>
        /// <param name="item">The type of the item. Can be either "project", "folder", or "document"</param>
        /// <returns></returns>
        private TreeViewItem createDocumentExplorerItem(ListableItem item) {
            TreeViewItem thisTreeViewItem = new TreeViewItem() { Tag = item };
            //StackPanel for image and text block
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
            //Create the image
            BitmapImage icon;
            string text;
            if (item is Project) {
                icon = projectIcon;
                text = (item as Project).Title;
            }
            else if (item is Folder) {
                icon = folderIcon;
                text = (item as Folder).Title;
            }
            else {
                icon = documentIcon;
                text = (item as Document).Title;
            }
            Image image = new Image() { Source = icon, Height = 15, Width = 15, IsHitTestVisible = false }; /* note that IsHitTestVisible=false disables event handling for this element -
                                                                                                             * fallback on the general treeview handling (rightclick for the context menu).                                                                                                */
            sp.Children.Add(image);
            //Create the text block
            TextBlock itemText = new TextBlock() { Text = text, Margin = new Thickness(5, 0, 0, 0), IsHitTestVisible = false };
            sp.Children.Add(itemText);
            thisTreeViewItem.Header = sp;

            //recursive traversal of structure for Item Containers
            if (item is IItemContainer) {
                //First add folders
                foreach(Folder folder in (item as IItemContainer).GetFolders()) {
                    AddSubItemToDocExplorer(thisTreeViewItem, createDocumentExplorerItem(folder));
                }
                //then documents
                foreach (Document document in (item as IItemContainer).GetDocuments()) {
                    AddSubItemToDocExplorer(thisTreeViewItem, createDocumentExplorerItem(document));
                }
            }
            return thisTreeViewItem;
        }

        #endregion

        /// <summary>
        /// Fills the MainContent with useful information for the specific item
        /// </summary>
        /// <param name="item">The item which mainContent will use as a context</param>
        private void GenerateContent(ListableItem item) {
            if (item is IItemContainer) {
                FolderContentView folderContentView = new FolderContentView();
                foreach(Folder folder in (item as IItemContainer).GetFolders()) {
                    StackPanel sp = new StackPanel() { Width = 50, Height = 50, Orientation = Orientation.Vertical, IsHitTestVisible=false };
                    sp.Children.Add(new Image() { Source = folderIcon, Width = 24, Height = 24 });
                    sp.Children.Add(new TextBlock() { Text = folder.Title, MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center });
                    ListViewItem listViewItem = new ListViewItem() { Margin = new Thickness(2) };
                    listViewItem.Content = sp;
                    listViewItem.Tag = folder;
                    listViewItem.MouseDoubleClick += new MouseButtonEventHandler(FolderContentView_DoubleClick);
                    folderContentView.FolderListView.Items.Add(listViewItem);
                }
                foreach (Document document in (item as IItemContainer).GetDocuments()) {
                    StackPanel sp = new StackPanel() { Width = 50, Height = 50, Orientation = Orientation.Vertical };
                    sp.Children.Add(new Image() { Source = documentIcon, Width = 24, Height = 24 });
                    sp.Children.Add(new TextBlock() { Text = document.Title, MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center });
                    ListViewItem listViewItem = new ListViewItem() { Margin = new Thickness(2) };
                    listViewItem.Content = sp;
                    listViewItem.Tag = document;
                    listViewItem.MouseDoubleClick += new MouseButtonEventHandler(FolderContentView_DoubleClick);
                    folderContentView.FolderListView.Items.Add(listViewItem);
                } 
                
                MainContent.Content = folderContentView;
            } else {
                TextEditor t = new TextEditor();
                //I'm currently using Title as a placeholder - Document.content is not implemented yet.
                t.TextField.Text = "This is the text editor for:\n" + (item as Document).Title;
                MainContent.Content = t;
            }
        }

        private void FolderContentView_DoubleClick(object sender, MouseButtonEventArgs e) {
            GenerateContent((sender as ListViewItem).Tag as ListableItem);
        }


        /// <summary>
        /// This makes right click select an item in the Document Explorer and show the appropiate context menu for the item.
        /// </summary>
        /// <param name="sender">The object that send the event</param>
        /// <param name="e">The MouseButtonEventArgs for the event</param>
        private void DocumentExplorer_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            TreeViewItem item = e.Source as TreeViewItem;
            if (item != null) {
                item.IsSelected = true;
                generateContextMenu(item.Tag as ListableItem);
            }
        }

        /// <summary>
        /// This makes the main content in the main window change upon double-click on an item
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void DocumentExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            TreeViewItem item = ((TreeView)sender).SelectedItem as TreeViewItem;
            if (item != null) {
                item.IsSelected = true;
                GenerateContent(item.Tag as ListableItem);
            }
        }

        private void DocumentExplorer_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key.Equals(System.Windows.Input.Key.Enter)) {
                TreeViewItem item = e.Source as TreeViewItem;
                if (item != null) {
                    item.IsSelected = true;
                    item.IsExpanded = !item.IsExpanded;
                    GenerateContent(item.Tag as ListableItem);
                }
            }
        }

        private void DocumentExplorer_MouseDown(object sender, MouseButtonEventArgs e) {

        }
    }

}
