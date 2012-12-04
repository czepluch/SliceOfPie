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
        private static Controller controller;

        public MainWindow() {
            controller = Controller.Instance;

            InitializeComponent();
            InitializeDocumentExplorer();
            RefreshDocumentExplorer();
            TreeViewItem topProject = DocumentExplorer.Items[0] as TreeViewItem; //Note there's always at least one project
            topProject.IsSelected = true;
            GenerateContent(topProject.Tag as ListableItem); 
        }

        /// <summary>
        /// Sets up the DocumentExplorer's icons and context menus
        /// </summary>
        private void InitializeDocumentExplorer() {
            //Setup Project Context Menu for the Document Explorer
            projectContextMenu = new ContextMenu();
            folderContextMenu = new ContextMenu();
            documentContextMenu = new ContextMenu();

            //create the project context menu
            MenuItem shareProjectProjectContext = new MenuItem() { Header = "Share project" };
            shareProjectProjectContext.Click += new RoutedEventHandler(openShareProjectWindow);

            MenuItem openProjectFolderProjectContext = new MenuItem() { Header = "Open project folder" };
            openProjectFolderProjectContext.Click += new RoutedEventHandler(generateContentContextMenu_Click);

            MenuItem addFolderProjectContext = new MenuItem() { Header = "Add folder" };
            addFolderProjectContext.Click += new RoutedEventHandler(openCreateFolderWindow);

            MenuItem addDocumentProjectContext = new MenuItem() { Header = "Add document" };
            addDocumentProjectContext.Click += new RoutedEventHandler(openCreateDocumentWindow);

            projectContextMenu.Items.Add(shareProjectProjectContext);
            projectContextMenu.Items.Add(openProjectFolderProjectContext);
            projectContextMenu.Items.Add(addFolderProjectContext);
            projectContextMenu.Items.Add(addDocumentProjectContext);

            //create the folder context menu
            MenuItem openFolderFolderContext = new MenuItem() { Header = "Open folder" };
            openFolderFolderContext.Click += new RoutedEventHandler(generateContentContextMenu_Click);

            MenuItem addFolderFolderContext = new MenuItem() { Header = "Add folder" };
            addFolderFolderContext.Click += new RoutedEventHandler(openCreateFolderWindow);

            MenuItem addDocumentFolderContext = new MenuItem() { Header = "Add document" };
            addDocumentFolderContext.Click += new RoutedEventHandler(openCreateDocumentWindow);

            folderContextMenu.Items.Add(openFolderFolderContext);
            folderContextMenu.Items.Add(addFolderFolderContext);
            folderContextMenu.Items.Add(addDocumentFolderContext);

            //create the document context men
            MenuItem editDocumentDocumentContext = new MenuItem() { Header = "Edit document" };
            editDocumentDocumentContext.Click += new RoutedEventHandler(generateContentContextMenu_Click);

            documentContextMenu.Items.Add(editDocumentDocumentContext);  
        }

        /// <summary>
        /// A default event handler for changing the main content due to a click
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void generateContentContextMenu_Click(object sender, RoutedEventArgs e) {
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
            DocumentExplorer.Items.Clear();
            foreach (Project project in controller.GetProjects("local")) {
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
                                                                                                             * fallback on the general treeview handling (rightclick for the context menu).                                                                                                */
            sp.Children.Add(image);
            //Create the text block
            TextBlock itemText = new TextBlock() { Text = text, Margin = new Thickness(5, 0, 0, 0), IsHitTestVisible = false };
            sp.Children.Add(itemText);
            thisTreeViewItem.Header = sp;
            //set up event handlers
            thisTreeViewItem.MouseLeftButtonUp += new MouseButtonEventHandler(DocumentExplorerItemMouseLeftButtonUp);
            thisTreeViewItem.MouseRightButtonUp += new MouseButtonEventHandler(DocumentExplorerItemMouseRightButtonUp);
            thisTreeViewItem.MouseRightButtonDown += new MouseButtonEventHandler(DocumentExplorerItemMouseRightButtonDown);
            thisTreeViewItem.KeyUp += new KeyEventHandler(DocumentExplorerItemKeyUp);

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
                FolderContentView folderContentView = new FolderContentView(item as IItemContainer, new MouseButtonEventHandler(FolderContentView_DoubleClick));
                folderContentView.setCreateFolderHandler(new RoutedEventHandler(openCreateFolderWindow));
                folderContentView.setCreateDocumentHandler(new RoutedEventHandler(openCreateDocumentWindow));
                MainContent.Content = folderContentView;
            } else {
                MainContent.Content = new TextEditor(item as Document);
            }
        }

        #region EventHandlers

        /// <summary>
        /// This event handler opens the CreateProject pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void openCreateProjectWindow(object sender, RoutedEventArgs e) {
            CreateProject.IsOpen = true;
        }

        /// <summary>
        /// This event handler opens the ShareProject pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void openShareProjectWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            ShareProject.IsOpen = true;
        }

        /// <summary>
        /// This event handler opens the CreateFolder pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void openCreateFolderWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            CreateFolder.IsOpen = true;
        }

        /// <summary>
        /// This event handler opens the CreateDocument pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void openCreateDocumentWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            CreateDocument.IsOpen = true;
        }

        /// <summary>
        /// This makes right click select an item in the Document Explorer and show the appropiate context menu for the item.
        /// </summary>
        /// <param name="sender">The object that send the event</param>
        /// <param name="e">The MouseButtonEventArgs for the event</param>
        private void DocumentExplorerItemMouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null) {
                e.Handled = true;
                item.IsSelected = true;
                generateContextMenu(item.Tag as ListableItem);
            }
        }

        /// <summary>
        /// This makes right click select an item on the keydown event.
        /// </summary>
        /// <param name="sender">The object that send the event</param>
        /// <param name="e">The MouseButtonEventArgs for the event</param>
        private void DocumentExplorerItemMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null) {
                e.Handled = true;
                item.IsSelected = true;
            }
        }

        /// <summary>
        /// This makes the main content in the main window change upon double-click on an item
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void DocumentExplorerItemMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null) {
                e.Handled = true;
                item.IsSelected = true;
                item.IsExpanded = true;
                GenerateContent(item.Tag as ListableItem);
            }
        }

        /// <summary>
        /// This is the event handler for key events in the Document Explorer
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void DocumentExplorerItemKeyUp(object sender, KeyEventArgs e) {
            if (e.Key.Equals(System.Windows.Input.Key.Enter)) {
                TreeViewItem item = sender as TreeViewItem;
                if (item != null) {
                    e.Handled = true;
                    item.IsSelected = true;
                    item.IsExpanded = true;
                    GenerateContent(item.Tag as ListableItem);
                }
            }
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateProjectCancelButton_Click(object sender, RoutedEventArgs e) {
            CreateProject.IsOpen = false;
            CreateProjectTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Create button in the CreateProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateProjectCreateButton_Click(object sender, RoutedEventArgs e) {
            Controller.Instance.CreateProject(CreateProjectTextBox.Text, "local");
            CreateProject.IsOpen = false;
            CreateProjectTextBox.Clear();
            RefreshDocumentExplorer();
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateFolder pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateFolderCancelButton_Click(object sender, RoutedEventArgs e) {
            CreateFolder.IsOpen = false;
            CreateFolderTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Create button in the CreateFolder pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateFolderCreateButton_Click(object sender, RoutedEventArgs e) {
            Controller.Instance.CreateFolder(CreateFolderTextBox.Text, "local", (DocumentExplorer.SelectedItem as TreeViewItem).Tag as IItemContainer);
            CreateFolder.IsOpen = false;
            CreateFolderTextBox.Clear();
            RefreshDocumentExplorer();
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateDocument pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateDocumentCancelButton_Click(object sender, RoutedEventArgs e) {
            CreateDocument.IsOpen = false;
            CreateDocumentTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Create button in the CreateDocument pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateDocumentCreateButton_Click(object sender, RoutedEventArgs e) {
            Controller.Instance.CreateDocument(CreateDocumentTextBox.Text, "local", (DocumentExplorer.SelectedItem as TreeViewItem).Tag as IItemContainer);
            CreateDocument.IsOpen = false;
            CreateDocumentTextBox.Clear();
            RefreshDocumentExplorer();
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the ShareProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void ShareProjectCancelButton_Click(object sender, RoutedEventArgs e) {
            ShareProject.IsOpen = false;
            ShareProjectTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Share button in the ShareProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void ShareProjectShareButton_Click(object sender, RoutedEventArgs e) {
            Controller.Instance.ShareProject((DocumentExplorer.SelectedItem as TreeViewItem).Tag as Project, ShareProjectTextBox.Text.Split(','));
            //Call to controller shares the project. Awaiting controller method before implementation
            ShareProject.IsOpen = false;
            ShareProjectTextBox.Clear();
            RefreshDocumentExplorer();
        }


        /// <summary>
        /// This method is sent as a mouseeventhandler to the FolderContentView class.
        /// Adds custom behaviour to that class.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void FolderContentView_DoubleClick(object sender, MouseButtonEventArgs e) {
            ListableItem itemClicked = (sender as ListViewItem).Tag as ListableItem;
            //Find the corresponding item in the document explorer and fold out tree, so it is visible
            TreeViewItem treeViewParent = DocumentExplorer.SelectedItem as TreeViewItem;
            if (treeViewParent.IsExpanded == false) { //Expand parentfolder of the clicked item if it's not expanded
                treeViewParent.IsExpanded = true;
            }
            foreach (TreeViewItem item in treeViewParent.Items) {
                if (itemClicked.Equals(item.Tag)) {
                    item.IsSelected = true;
                    item.IsExpanded = true;
                }
            }
            GenerateContent(itemClicked);
        }

        #endregion
    }

}
