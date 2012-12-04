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

namespace SliceOfPie {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static ContextMenu projectContextMenu, folderContextMenu, documentContextMenu;
        private static Controller controller;

        private static ListableItem currentlyActiveItem;
        private static Popup currentlyActivePopUp;

        public MainWindow() {
            controller = Controller.Instance;

            InitializeComponent();
            InitializeDocumentExplorer();
            RefreshDocumentExplorer(); //Note there's always at least one project
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
            shareProjectProjectContext.Click += new RoutedEventHandler(OpenShareProjectWindow);

            MenuItem openProjectFolderProjectContext = new MenuItem() { Header = "Open project folder" };
            openProjectFolderProjectContext.Click += new RoutedEventHandler(SetActiveItemContextMenu_Click);

            MenuItem addFolderProjectContext = new MenuItem() { Header = "Add folder" };
            addFolderProjectContext.Click += new RoutedEventHandler(OpenCreateFolderWindow);

            MenuItem addDocumentProjectContext = new MenuItem() { Header = "Add document" };
            addDocumentProjectContext.Click += new RoutedEventHandler(OpenCreateDocumentWindow);

            projectContextMenu.Items.Add(shareProjectProjectContext);
            projectContextMenu.Items.Add(openProjectFolderProjectContext);
            projectContextMenu.Items.Add(addFolderProjectContext);
            projectContextMenu.Items.Add(addDocumentProjectContext);

            //create the folder context menu
            MenuItem openFolderFolderContext = new MenuItem() { Header = "Open folder" };
            openFolderFolderContext.Click += new RoutedEventHandler(SetActiveItemContextMenu_Click);

            MenuItem addFolderFolderContext = new MenuItem() { Header = "Add folder" };
            addFolderFolderContext.Click += new RoutedEventHandler(OpenCreateFolderWindow);

            MenuItem addDocumentFolderContext = new MenuItem() { Header = "Add document" };
            addDocumentFolderContext.Click += new RoutedEventHandler(OpenCreateDocumentWindow);

            folderContextMenu.Items.Add(openFolderFolderContext);
            folderContextMenu.Items.Add(addFolderFolderContext);
            folderContextMenu.Items.Add(addDocumentFolderContext);

            //create the document context men
            MenuItem editDocumentDocumentContext = new MenuItem() { Header = "Edit document" };
            editDocumentDocumentContext.Click += new RoutedEventHandler(SetActiveItemContextMenu_Click);

            documentContextMenu.Items.Add(editDocumentDocumentContext);  
        }

        /// <summary>
        /// A default event handler for changing the main content due to a click
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void SetActiveItemContextMenu_Click(object sender, RoutedEventArgs e) {
            SetMainContentFromItem(currentlyActiveItem);
        }

        

        /// <summary>
        /// This generates and shows a context menu for the document explorer at runtime
        /// </summary>
        /// <param name="item">The item which was clicked on the document explorer</param>
        private void GenerateContextMenu(ListableItem item) {
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
        private void RefreshDocumentExplorer(ListableItem itemToHighLight = null) {
            DocumentExplorer.Items.Clear();
            //Add each project
            foreach (Project project in controller.GetProjects("local")) {
                TreeViewItem projectItem = CreateDocumentExplorerItem(project);
                AddProjectToDocExplorer(projectItem);
            }
            if (itemToHighLight != null) {
                //Expand to the optional item
                foreach (TreeViewItem container in DocumentExplorer.Items) {
                    ExpandToItem(container, itemToHighLight);
                }
            }
            else {
                TreeViewItem topProject = DocumentExplorer.Items[0] as TreeViewItem;
                topProject.IsSelected = true;
                currentlyActiveItem = topProject.Tag as ListableItem;
                SetMainContentFromItem(currentlyActiveItem);
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
        private TreeViewItem CreateDocumentExplorerItem(ListableItem item) {
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
                foreach(Folder folder in (item as IItemContainer).GetFolders()) {
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

        /// <summary>
        /// Fills the MainContent with useful information for the specific item
        /// </summary>
        /// <param name="item">The item which mainContent will use as a context</param>
        private void SetMainContentFromItem(ListableItem item) {
            currentlyActiveItem = item;
            if (item is IItemContainer) {
                FolderContentView folderContentView = new FolderContentView(item as IItemContainer, new MouseButtonEventHandler(FolderContentView_DoubleClick));
                folderContentView.setCreateFolderHandler(new RoutedEventHandler(OpenCreateFolderWindow));
                folderContentView.setCreateDocumentHandler(new RoutedEventHandler(OpenCreateDocumentWindow));
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
        private void OpenCreateProjectWindow(object sender, RoutedEventArgs e) {
            IsEnabled = false;
            CreateProject.IsOpen = true;
            currentlyActivePopUp = CreateProject;
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
            currentlyActivePopUp = ShareProject;
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
            currentlyActivePopUp = CreateFolder;
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
            currentlyActivePopUp = CreateDocument;
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
                currentlyActiveItem = item.Tag as ListableItem;
                item.IsSelected = true;
                GenerateContextMenu(item.Tag as ListableItem);
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
                currentlyActiveItem = item.Tag as ListableItem;
                item.IsSelected = true;
                item.IsExpanded = true;
                SetMainContentFromItem(item.Tag as ListableItem);
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
                    currentlyActiveItem = item.Tag as ListableItem;
                    item.IsSelected = true;
                    item.IsExpanded = true;
                    SetMainContentFromItem(item.Tag as ListableItem);
                }
            }
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateProjectCancelButton_Click(object sender, RoutedEventArgs e) {
            currentlyActivePopUp = null;
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
            Project project = Controller.Instance.CreateProject(CreateProjectTextBox.Text, "local");
            currentlyActivePopUp = null;
            CreateProject.IsOpen = false;
            IsEnabled = true;
            CreateProjectTextBox.Clear();
            RefreshDocumentExplorer(project);
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateFolder pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateFolderCancelButton_Click(object sender, RoutedEventArgs e) {
            currentlyActivePopUp = null;
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
            Folder folder = Controller.Instance.CreateFolder(CreateFolderTextBox.Text, "local", currentlyActiveItem as IItemContainer);
            currentlyActivePopUp = null;
            CreateFolder.IsOpen = false;
            IsEnabled = true;
            CreateFolderTextBox.Clear();
            RefreshDocumentExplorer(folder);
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateDocument pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void CreateDocumentCancelButton_Click(object sender, RoutedEventArgs e) {
            currentlyActivePopUp = null;
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
            Document document = Controller.Instance.CreateDocument(CreateDocumentTextBox.Text, "local", currentlyActiveItem as IItemContainer);
            currentlyActivePopUp = null;
            CreateDocument.IsOpen = false;
            IsEnabled = true;
            CreateDocumentTextBox.Clear();
            SetMainContentFromItem(currentlyActiveItem); //show folder content and as such the new document
            RefreshDocumentExplorer(document);
        }

        private bool ExpandToItem(TreeViewItem container, ListableItem item) {
            ListableItem containerListable = container.Tag as ListableItem;
            if (containerListable == item) {
                container.IsSelected = true;
                currentlyActiveItem = item;
                SetMainContentFromItem(currentlyActiveItem); 
                return true;
            }
            else if (containerListable is IItemContainer) { //possibility that it's a subitem
                //
                foreach (TreeViewItem subItem in container.Items) { //repeat search for each subitem
                    if (ExpandToItem(subItem, item)) {
                        container.IsExpanded = true;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the ShareProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void ShareProjectCancelButton_Click(object sender, RoutedEventArgs e) {
            currentlyActivePopUp = null;
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
            Controller.Instance.ShareProject(currentlyActiveItem as Project, ShareProjectTextBox.Text.Split(','));
            //Call to controller shares the project. Awaiting controller method before implementation
            currentlyActivePopUp = null;
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
        private void FolderContentView_DoubleClick(object sender, MouseButtonEventArgs e) {
            ListableItem itemClicked = (sender as ListViewItem).Tag as ListableItem;
            //Find the corresponding item in the document explorer and fold out tree, so it is visible
            TreeViewItem treeViewParent = DocumentExplorer.SelectedItem as TreeViewItem;
            if (treeViewParent.IsExpanded == false) { //Expand parentfolder of the clicked item if it's not expanded
                treeViewParent.IsExpanded = true;
            }
            foreach (TreeViewItem item in treeViewParent.Items) {
                if (itemClicked.Equals(item.Tag)) {
                    currentlyActiveItem = item.Tag as ListableItem;
                    item.IsSelected = true;
                    item.IsExpanded = true;
                }
            }
            SetMainContentFromItem(itemClicked);
        }

        /// <summary>
        /// This is the click handler for the Sync button in the main window
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void Synchronize_Click(object sender, RoutedEventArgs e) {
            //TODO sync current changes here
            RefreshDocumentExplorer(currentlyActiveItem);
            TreeViewItem topProject = DocumentExplorer.Items[0] as TreeViewItem; //Note there's always at least one project
        }

        #endregion   
    }

}
