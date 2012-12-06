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
using System.Windows.Controls.Primitives;

namespace SliceOfPie.Client {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Controller controller;
        private FolderContentView folderContentView;
        private TextEditor textEditor;
        private ContextMenu projectContextMenu, folderContextMenu, documentContextMenu;

        private IListableItem currentContextItem;
        private Popup currentActivePopUp;

        /// <summary>
        /// Creates the Main Window for the Slice of Pie application
        /// </summary>
        public MainWindow() {
            controller = Controller.Instance;
            InitializeComponent();
            CreateContextMenus();
            
            folderContentView = new FolderContentView();
            folderContentView.ItemDoubleClicked += new EventHandler<ListableItemEventArgs>(FolderContentView_DoubleClick);
            folderContentView.CreateDocumentButtonClicked += (new RoutedEventHandler(OpenCreateDocumentWindow));
            folderContentView.CreateFolderButtonClicked += (new RoutedEventHandler(OpenCreateFolderWindow));

            textEditor = new TextEditor();
            textEditor.SaveDocumentButtonClicked += (new RoutedEventHandler(SaveDocument));

            ReloadProjects();
        }

        #region Initialization of the Document Explorer
        /// <summary>
        /// Sets up the DocumentExplorer's context menus
        /// </summary>
        private void CreateContextMenus() {
            projectContextMenu = new ContextMenu();
            folderContextMenu = new ContextMenu();
            documentContextMenu = new ContextMenu();

            //create the project context menu
            MenuItem shareProjectProjectContext = new MenuItem() { Header = "Share project" };
            shareProjectProjectContext.Click += new RoutedEventHandler(OpenShareProjectWindow);

            MenuItem openProjectFolderProjectContext = new MenuItem() { Header = "Open project folder" };
            openProjectFolderProjectContext.Click += new RoutedEventHandler(OpenItemOnContextMenuClick);

            MenuItem addFolderProjectContext = new MenuItem() { Header = "Add folder" };
            addFolderProjectContext.Click += new RoutedEventHandler(OpenCreateFolderWindow);

            MenuItem addDocumentProjectContext = new MenuItem() { Header = "Add document" };
            addDocumentProjectContext.Click += new RoutedEventHandler(OpenCreateDocumentWindow);

            MenuItem removeProjectContext = new MenuItem() { Header = "Remove project" };
            removeProjectContext.Click += new RoutedEventHandler(RemoveItemOnContextMenuClick);

            

            projectContextMenu.Items.Add(shareProjectProjectContext);
            projectContextMenu.Items.Add(openProjectFolderProjectContext);
            projectContextMenu.Items.Add(addFolderProjectContext);
            projectContextMenu.Items.Add(addDocumentProjectContext);
            projectContextMenu.Items.Add(removeProjectContext);

            //create the folder context menu
            MenuItem openFolderFolderContext = new MenuItem() { Header = "Open folder" };
            openFolderFolderContext.Click += new RoutedEventHandler(OpenItemOnContextMenuClick);

            MenuItem addFolderFolderContext = new MenuItem() { Header = "Add folder" };
            addFolderFolderContext.Click += new RoutedEventHandler(OpenCreateFolderWindow);

            MenuItem addDocumentFolderContext = new MenuItem() { Header = "Add document" };
            addDocumentFolderContext.Click += new RoutedEventHandler(OpenCreateDocumentWindow);

            MenuItem removeFolderContext = new MenuItem() { Header = "Remove folder" };
            removeFolderContext.Click += new RoutedEventHandler(RemoveItemOnContextMenuClick);

            folderContextMenu.Items.Add(openFolderFolderContext);
            folderContextMenu.Items.Add(addFolderFolderContext);
            folderContextMenu.Items.Add(addDocumentFolderContext);
            folderContextMenu.Items.Add(removeFolderContext);

            //create the document context men
            MenuItem editDocumentDocumentContext = new MenuItem() { Header = "Edit document" };
            editDocumentDocumentContext.Click += new RoutedEventHandler(OpenItemOnContextMenuClick);

            MenuItem removeDocumentContext = new MenuItem() { Header = "Remove document" };
            removeDocumentContext.Click += new RoutedEventHandler(RemoveItemOnContextMenuClick);

            documentContextMenu.Items.Add(editDocumentDocumentContext);
            documentContextMenu.Items.Add(removeDocumentContext);
        }

        #endregion

        /// <summary>
        /// This generates and shows a context menu for the document explorer at runtime
        /// </summary>
        /// <param name="item">The item which was clicked on the document explorer</param>
        private void ShowContextMenu(IListableItem item) {
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
        /// This method refreshes the document explorer.
        /// </summary>
        /// <param name="itemToOpen">The item to open, when the projects are reloaded</param>
        private void ReloadProjects(IListableItem itemToOpen = null) {
            DocumentExplorer.Items.Clear();
            //Add each project
            foreach (Project project in controller.GetProjects("local")) {
                TreeViewItem projectItem = CreateDocumentExplorerItem(project);
                AddProjectToDocExplorer(projectItem);
            }
            //Expand to the current context item
            if (itemToOpen != null) {
                
                foreach (TreeViewItem container in DocumentExplorer.Items) {
                    ExpandToAndOpenItem(container, itemToOpen);
                }
            }
            //or just select the top project (there will always be at least one project)
            else {
                TreeViewItem topProject = DocumentExplorer.Items[0] as TreeViewItem;
                topProject.IsSelected = true;
                currentContextItem = topProject.Tag as IListableItem;
                Open(currentContextItem);
            }
        }

        /// <summary>
        /// Fills the MainContent with useful information for the specific item
        /// </summary>
        /// <param name="item">The item which mainContent will use as a context</param>
        private void Open(IListableItem item) {
            currentContextItem = item;
            if (item is IItemContainer) {
                folderContentView.ItemContainer = item as IItemContainer;
                MainContent.Content = folderContentView;
            } else {
                textEditor.Document = item as Document;
                MainContent.Content = textEditor;
            }
        }

        /// <summary>
        /// This method expands the DocumentExplorers items from a start container down to a given item.
        /// </summary>
        /// <param name="container">The starter container for the search</param>
        /// <param name="item">The item to be found. This item is also expanded if found </param>
        /// <returns>Returns true if the item was found</returns>
        private bool ExpandToAndOpenItem(TreeViewItem container, IListableItem item) {
            IListableItem containerListable = container.Tag as IListableItem;
            if (containerListable == item) {
                container.IsSelected = true;
                container.IsExpanded = true;
                currentContextItem = item;
                Open(currentContextItem);
                return true;
            }
            else if (containerListable is IItemContainer) { //possibility that it's in a subitem
                foreach (TreeViewItem subItem in container.Items) { //repeat search for each subitem
                    if (ExpandToAndOpenItem(subItem, item)) {
                        container.IsExpanded = true;
                        return true;
                    }
                }
            }
            return false;
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
                                                                                                             * fallback on the general treeview handling (rightclick for the context menu).                                                                                                */
            sp.Children.Add(image);
            //Create the text block
            TextBlock itemText = new TextBlock() { Text = text, Margin = new Thickness(5, 0, 0, 0), IsHitTestVisible = false };
            sp.Children.Add(itemText);
            thisTreeViewItem.Header = sp;
            //set up event handlers
            thisTreeViewItem.MouseLeftButtonUp += new MouseButtonEventHandler(DocumentExplorerItemMouseLeftButtonUp);
            thisTreeViewItem.MouseDoubleClick += new MouseButtonEventHandler(DocumentExplorerItemMouseDoubleClick);
            thisTreeViewItem.MouseRightButtonUp += new MouseButtonEventHandler(DocumentExplorerItemMouseRightButtonUp);
            thisTreeViewItem.MouseRightButtonDown += new MouseButtonEventHandler(DocumentExplorerItemMouseRightButtonDown);
            thisTreeViewItem.KeyUp += new KeyEventHandler(DocumentExplorerItemKeyUp);

            //recursive traversal of structure for Item Containers
            if (item is IItemContainer) {
                //First add folders
                foreach (Folder folder in (item as IItemContainer).GetFolders()) {
                    AddSubItemToDocExplorer(thisTreeViewItem, CreateDocumentExplorerItem(folder));
                }
                //then documents
                foreach (Document document in (item as IItemContainer).GetDocuments()) {
                    AddSubItemToDocExplorer(thisTreeViewItem, CreateDocumentExplorerItem(document));
                }
            }
            return thisTreeViewItem;
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// A default event handler for changing the main content due to a click
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void OpenItemOnContextMenuClick(object sender, RoutedEventArgs e) {
            Open(currentContextItem);
        }

        /// <summary>
        /// A default event handler for changing the main content due to a click
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void RemoveItemOnContextMenuClick(object sender, RoutedEventArgs e) {
            if (currentContextItem is Project) {
                controller.RemoveProject(currentContextItem as Project);
            } else if (currentContextItem is Folder) {
                controller.RemoveFolder(currentContextItem as Folder);
            } else {
                controller.RemoveDocument(currentContextItem as Document);
            }
            ReloadProjects();
        }

        /// <summary>
        /// This event handler opens the CreateProject pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void OpenCreateProjectWindow(object sender, RoutedEventArgs e) {
            IsEnabled = false;
            CreateProject.IsOpen = true;
            currentActivePopUp = CreateProject;
            CreateProjectTextBox.Focus();
        }

        /// <summary>
        /// This event handler opens the ShareProject pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void OpenShareProjectWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            IsEnabled = false;
            ShareProject.IsOpen = true;
            currentActivePopUp = ShareProject;
            ShareProjectTextBox.Focus();
        }

        /// <summary>
        /// This event handler opens the CreateFolder pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void OpenCreateFolderWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            IsEnabled = false;
            CreateFolder.IsOpen = true;
            currentActivePopUp = CreateFolder;
            CreateFolderTextBox.Focus();
        }

        /// <summary>
        /// This event handler opens the CreateDocument pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void OpenCreateDocumentWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            IsEnabled = false;
            CreateDocument.IsOpen = true;
            currentActivePopUp = CreateDocument;
            CreateDocumentTextBox.Focus();
        }

        /// <summary>
        /// This makes right click select an item in the Document Explorer and show the appropiate context menu for the item.
        /// </summary>
        /// <param name="sender">The object that send the event</param>
        /// <param name="e">The MouseButtonEventArgs for the event</param>
        private void DocumentExplorerItemMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            e.Handled = true; //Disabling the default doubleclick event in a treeview list item.
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
                currentContextItem = item.Tag as IListableItem;
                item.IsSelected = true;
                ShowContextMenu(item.Tag as IListableItem);
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
                item.IsSelected = true; //selected for visual feedback. It is not considered the active item though.
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
                currentContextItem = item.Tag as IListableItem;
                item.IsSelected = true;
                item.IsExpanded = true;
                Open(item.Tag as IListableItem);
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
                    currentContextItem = item.Tag as IListableItem;
                    item.IsSelected = true;
                    item.IsExpanded = true;
                    Open(item.Tag as IListableItem);
                }
            }
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateProjectCancelButton_Click(object sender, RoutedEventArgs e) {
            currentActivePopUp = null;
            CreateProject.IsOpen = false;
            IsEnabled = true;
            CreateProjectTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Create button in the CreateProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateProjectCreateButton_Click(object sender, RoutedEventArgs e) {
            //Create project asynchronously and then reload the projects when done and open the document
            controller.BeginCreateProject(CreateProjectTextBox.Text, "local", (iar) => { Project project = controller.EndCreateProject(iar); ReloadProjects(project); }, null);
            //Close popup
            currentActivePopUp = null;
            CreateProject.IsOpen = false;
            IsEnabled = true;
            CreateProjectTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateFolder pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateFolderCancelButton_Click(object sender, RoutedEventArgs e) {
            currentActivePopUp = null;
            CreateFolder.IsOpen = false;
            IsEnabled = true;
            CreateFolderTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Create button in the CreateFolder pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateFolderCreateButton_Click(object sender, RoutedEventArgs e) {
            //Create folder asynchronously and then reload the projects when done and open the document
            controller.BeginCreateFolder(CreateFolderTextBox.Text, "local", currentContextItem as IItemContainer, (iar) => { Folder folder = controller.EndCreateFolder(iar); ReloadProjects(folder); }, null);
            //Close popup
            currentActivePopUp = null;
            CreateFolder.IsOpen = false;
            IsEnabled = true;
            CreateFolderTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateDocument pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateDocumentCancelButton_Click(object sender, RoutedEventArgs e) {
            currentActivePopUp = null;
            CreateDocument.IsOpen = false;
            IsEnabled = true;
            CreateDocumentTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Create button in the CreateDocument pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateDocumentCreateButton_Click(object sender, RoutedEventArgs e) {
            //Create document asynchronously and then reload the projects when done and open the document
            controller.BeginCreateDocument(CreateDocumentTextBox.Text, "local", currentContextItem as IItemContainer, (iar) => { Document document = controller.EndCreateDocument(iar); ReloadProjects(document); }, null);
            //Close popup
            currentActivePopUp = null;
            CreateDocument.IsOpen = false;
            IsEnabled = true;
            CreateDocumentTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the ShareProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void ShareProjectCancelButton_Click(object sender, RoutedEventArgs e) {
            currentActivePopUp = null;
            ShareProject.IsOpen = false;
            IsEnabled = true;
            ShareProjectTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Share button in the ShareProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void ShareProjectShareButton_Click(object sender, RoutedEventArgs e) {
            controller.ShareProject(currentContextItem as Project, ShareProjectTextBox.Text.Split(','));
            
            currentActivePopUp = null;
            ShareProject.IsOpen = false;
            IsEnabled = true;
            ShareProjectTextBox.Clear();
        }

        /// <summary>
        /// This method is sent as a mouseeventhandler to the FolderContentView class.
        /// Adds custom behaviour to that class.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void FolderContentView_DoubleClick(object sender, ListableItemEventArgs e) {
            //Find the corresponding item in the document explorer and fold out tree, so it is visible
            foreach (TreeViewItem project in DocumentExplorer.Items) {
                ExpandToAndOpenItem(project, e.Item);
            }
        }

        /// <summary>
        /// This is the click handler for the Sync button in the main window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void Synchronize_Click(object sender, RoutedEventArgs e) {
            controller.BeginSyncProjects("local", (iar) => ReloadProjects(), null);
        }

        /// <summary>
        /// This is the click handler for the Save Document button in the Text Editor
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void SaveDocument(object sender, RoutedEventArgs e) {
            controller.BeginSaveDocument(textEditor.Document, null, null);
        }

        #endregion   
    }
}
