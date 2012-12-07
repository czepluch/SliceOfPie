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
        private DocumentExplorer documentExplorer;
        private FolderContentView folderContentView;
        private TextEditor textEditor;

        private ContextMenu projectContextMenu, folderContextMenu, documentContextMenu;

        private IListableItem currentContextItem;
        private Popup currentActivePopUp;

        /// <summary>
        /// Creates the Main Window for the Slice of Pie application
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            CreateContextMenus();

            controller = Controller.Instance;

            documentExplorer = new DocumentExplorer();
            documentExplorer.ItemMouseLeftButtonUp += DocumentExplorerItemMouseLeftButtonUp;
            documentExplorer.ItemMouseRightButtonUp += DocumentExplorerItemMouseRightButtonUp;
            documentExplorer.ItemEnterKeyUp += DocumentExplorerItemEnterKeyUp;

            folderContentView = new FolderContentView();
            folderContentView.ItemDoubleClicked += new EventHandler<ListableItemEventArgs>(FolderContentView_DoubleClick);
            folderContentView.CreateDocumentButtonClicked += (new RoutedEventHandler(OpenCreateDocumentWindow));
            folderContentView.CreateFolderButtonClicked += (new RoutedEventHandler(OpenCreateFolderWindow));

            textEditor = new TextEditor();
            textEditor.SaveDocumentButtonClicked += (new RoutedEventHandler(SaveDocument));

            //Using controllers APM to load the projects DocumentExplorer
            controller.BeginGetProjects("local", (iar) => documentExplorer.Projects = controller.EndGetProjects(iar), null);
        }

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

            //create the document context menu
            MenuItem editDocumentDocumentContext = new MenuItem() { Header = "Edit document" };
            editDocumentDocumentContext.Click += new RoutedEventHandler(OpenItemOnContextMenuClick);

            MenuItem removeDocumentContext = new MenuItem() { Header = "Remove document" };
            removeDocumentContext.Click += new RoutedEventHandler(RemoveItemOnContextMenuClick);

            documentContextMenu.Items.Add(editDocumentDocumentContext);
            documentContextMenu.Items.Add(removeDocumentContext);
        }


        /// <summary>
        /// This generates a suitable context menu for a given listable item
        /// </summary>
        /// <param name="item">The item to show a context menu for</param>
        private ContextMenu GetContextMenu(IListableItem item) {
            if (item is Project) {
                return projectContextMenu;
            }
            else if (item is Folder) {
                return folderContextMenu;
            }
            else {
                return documentContextMenu;
            }
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
            }
            else {
                textEditor.Document = item as Document;
                MainContent.Content = textEditor;
            }
        }



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
            }
            else if (currentContextItem is Folder) {
                controller.RemoveFolder(currentContextItem as Folder);
            }
            else {
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
        private void DocumentExplorerItemMouseRightButtonUp(object sender, ListableItemEventArgs e) {
            currentContextItem = e.Item;
            documentExplorer.ShowContextMenuForSelected(GetContextMenu(e.Item));
        }

        /// <summary>
        /// This makes the main content in the main window change upon double-click on an item
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void DocumentExplorerItemMouseLeftButtonUp(object sender, ListableItemEventArgs e) {
            currentContextItem = e.Item;
            Open(e.Item);
        }

        /// <summary>
        /// This is the event handler for key events in the Document Explorer
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        private void DocumentExplorerItemEnterKeyUp(object sender, ListableItemEventArgs e) {
            currentContextItem = e.Item;
            Open(e.Item);
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
            Project project = controller.CreateProject(CreateProjectTextBox.Text, "local");
            currentActivePopUp = null;
            CreateProject.IsOpen = false;
            IsEnabled = true;
            CreateProjectTextBox.Clear();
            ReloadProjects(project);
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
            Folder folder = controller.CreateFolder(CreateFolderTextBox.Text, "local", currentContextItem as IItemContainer);
            currentActivePopUp = null;
            CreateFolder.IsOpen = false;
            IsEnabled = true;
            CreateFolderTextBox.Clear();
            ReloadProjects(folder);
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
            Document document = controller.CreateDocument(CreateDocumentTextBox.Text, "local", currentContextItem as IItemContainer);
            currentActivePopUp = null;
            CreateDocument.IsOpen = false;
            IsEnabled = true;
            CreateDocumentTextBox.Clear();
            ReloadProjects(document);
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
            //Call to controller shares the project. Awaiting controller method before implementation
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
            //TODO sync current changes here
            ReloadProjects();
            TreeViewItem topProject = DocumentExplorer.Items[0] as TreeViewItem; //Note there's always at least one project
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
