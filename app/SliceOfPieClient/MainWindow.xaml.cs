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
using System.Threading;

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

        /// <summary>
        /// Creates the Main Window for the Slice of Pie application
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            CreateContextMenus();

            controller = Controller.Instance;

            //documentexplorer is "instantiated" in the xaml
            documentExplorer.ItemMouseLeftButtonUp += new EventHandler<ListableItemEventArgs>(DocumentExplorerItemMouseLeftButtonUp);
            documentExplorer.ItemMouseRightButtonUp += new EventHandler<ListableItemEventArgs>(DocumentExplorerItemMouseRightButtonUp);
            documentExplorer.ItemEnterKeyUp += new EventHandler<ListableItemEventArgs>(DocumentExplorerItemEnterKeyUp);

            folderContentView = new FolderContentView();
            folderContentView.ItemDoubleClicked += new EventHandler<ListableItemEventArgs>(FolderContentView_DoubleClick);
            folderContentView.CreateDocumentButtonClicked += (new RoutedEventHandler(OpenCreateDocumentWindow));
            folderContentView.CreateFolderButtonClicked += (new RoutedEventHandler(OpenCreateFolderWindow));

            textEditor = new TextEditor();
            textEditor.SaveDocumentButtonClicked += (new RoutedEventHandler(SaveDocument_Click));

            ReloadProjects();
        }

        /// <summary>
        /// This method reloads projects from the controller
        /// </summary>
        /// <param name="itemToOpen">The item to open, when the projects are reloaded. Default is the top project.</param>
        private void ReloadProjects(IListableItem itemToOpen = null) {
            //Using controllers APM to load the projects into the Document Explorer
            SynchronizationContext context = SynchronizationContext.Current; //Call callback on own thread.
            controller.BeginGetProjects("local", (iar) => {
                //Callback posted in UI-context
                context.Post((o) => {
                    documentExplorer.Projects = controller.EndGetProjects(iar).ToList();
                    if (itemToOpen != null) {
                        documentExplorer.ExpandTo(itemToOpen, Open);
                    }
                    else {
                        documentExplorer.CallbackSelected(Open);
                    }
                }, null);
            }, null);
        }

        /// <summary>
        /// Fills the MainContent with useful information for the specific item
        /// </summary>
        /// <param name="item">The item which mainContent will use as a context.</param>
        private void Open(IListableItem item) {
            currentContextItem = item;
            if (item is IItemContainer) {
                folderContentView.ItemContainer = item as IItemContainer;
                mainContent.Content = folderContentView;
            }
            else {
                textEditor.Document = item as Document;
                mainContent.Content = textEditor;
            }
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
        /// <param name="item">The item to show a context menu for.</param>
        /// <returns>The context menu</returns>
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

        #region EventHandlers

        /// <summary>
        /// A default event handler for changing the main content due to a click
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The RoutedEventArgs.</param>
        private void OpenItemOnContextMenuClick(object sender, RoutedEventArgs e) {
            Open(currentContextItem);
        }

        /// <summary>
        /// A default event handler for changing the main content due to a click
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The RoutedEventArgs.</param>
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
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenCreateProjectWindow(object sender, RoutedEventArgs e) {
            IsEnabled = false;
            createProjectPopUp.IsOpen = true;
            createProjectPopUPTextBox.Focus();
        }

        /// <summary>
        /// This event handler opens the ShareProject pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenShareProjectWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            IsEnabled = false;
            shareProjectPopUP.IsOpen = true;
            shareProjectPopUPTextBox.Focus();
        }

        /// <summary>
        /// This event handler opens the CreateFolder pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenCreateFolderWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            IsEnabled = false;
            createFolderPopUP.IsOpen = true;
            createFolderPopUPTextBox.Focus();
        }

        /// <summary>
        /// This event handler opens the CreateDocument pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenCreateDocumentWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            IsEnabled = false;
            createDocumentPopUP.IsOpen = true;
            createDocumentPopUPTextBox.Focus();
        }

        /// <summary>
        /// This event handler opens the Login pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenLoginWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            IsEnabled = false;
            loginPopUp.IsOpen = true;
            loginPopUPUserTextBox.Focus();
        }

        /// <summary>
        /// This is the event handler for a right click (up event) in the Document Explorer
        /// </summary>
        /// <param name="sender">The object that send the event.</param>
        /// <param name="e">The event arguments.</param>
        private void DocumentExplorerItemMouseRightButtonUp(object sender, ListableItemEventArgs e) {
            currentContextItem = e.Item;
            documentExplorer.ShowContextMenuForSelected(GetContextMenu(e.Item));
        }

        /// <summary>
        /// This is the event handler for a left click (up event) in the Document Explorer
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void DocumentExplorerItemMouseLeftButtonUp(object sender, ListableItemEventArgs e) {
            currentContextItem = e.Item;
            Open(e.Item);
        }

        /// <summary>
        /// This is the event handler for an enter key event in the Document Explorer
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void DocumentExplorerItemEnterKeyUp(object sender, ListableItemEventArgs e) {
            currentContextItem = e.Item;
            Open(e.Item);
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CreateProjectPopUpCancelButton_Click(object sender, RoutedEventArgs e) {
            createProjectPopUp.IsOpen = false;
            IsEnabled = true;
            createProjectPopUPTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Create button in the CreateProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CreateProjectPopUpCreateButton_Click(object sender, RoutedEventArgs e) {
            Project project = controller.CreateProject(createProjectPopUPTextBox.Text, "local");
            createProjectPopUp.IsOpen = false;
            IsEnabled = true;
            createProjectPopUPTextBox.Clear();
            ReloadProjects(project);
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateFolder pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CreateFolderPopUpCancelButton_Click(object sender, RoutedEventArgs e) {
            createFolderPopUP.IsOpen = false;
            IsEnabled = true;
            createFolderPopUPTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Create button in the CreateFolder pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CreateFolderPopUpCreateButton_Click(object sender, RoutedEventArgs e) {
            Folder folder = controller.CreateFolder(createFolderPopUPTextBox.Text, "local", currentContextItem as IItemContainer);
            createFolderPopUP.IsOpen = false;
            IsEnabled = true;
            createFolderPopUPTextBox.Clear();
            ReloadProjects(folder);
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the CreateDocument pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CreateDocumentPopUpCancelButton_Click(object sender, RoutedEventArgs e) {
            createDocumentPopUP.IsOpen = false;
            IsEnabled = true;
            createDocumentPopUPTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Create button in the CreateDocument pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CreateDocumentPopUpCreateButton_Click(object sender, RoutedEventArgs e) {
            Document document = controller.CreateDocument(createDocumentPopUPTextBox.Text, "local", currentContextItem as IItemContainer);
            createDocumentPopUP.IsOpen = false;
            IsEnabled = true;
            createDocumentPopUPTextBox.Clear();
            ReloadProjects(document);
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the ShareProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ShareProjectPopUpCancelButton_Click(object sender, RoutedEventArgs e) {
            shareProjectPopUP.IsOpen = false;
            IsEnabled = true;
            shareProjectPopUPTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Share button in the ShareProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ShareProjectPopUpShareButton_Click(object sender, RoutedEventArgs e) {
            controller.ShareProject(currentContextItem as Project, shareProjectPopUPTextBox.Text.Split(','));
            shareProjectPopUP.IsOpen = false;
            IsEnabled = true;
            shareProjectPopUPTextBox.Clear();
        }


        /// <summary>
        /// This is the click handler for the Cancel button in the ShareProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LoginPopUpCancelButton_Click(object sender, RoutedEventArgs e) {
            loginPopUp.IsOpen = false;
            IsEnabled = true;
            loginPopUPUserTextBox.Clear();
            loginPopUPPasswordTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Share button in the ShareProject pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void loginPopUpLoginButton_Click(object sender, RoutedEventArgs e) {
            //controller.SyncProjects(loginPopUPUserTextBox.Text);
            ReloadProjects();
           
            loginPopUp.IsOpen = false;
            IsEnabled = true;
            loginPopUPUserTextBox.Clear();
            loginPopUPPasswordTextBox.Clear();
        }

        /// <summary>
        /// This method is sent as a mouse event handler to the FolderContentView class.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void FolderContentView_DoubleClick(object sender, ListableItemEventArgs e) {
            documentExplorer.ExpandTo(e.Item, Open);
        }

        /// <summary>
        /// This is the click handler for the Sync button in the main window
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Synchronize_Click(object sender, RoutedEventArgs e) {
            OpenLoginWindow(sender, e);
        }

        /// <summary>
        /// This is the click handler for the Save Document button in the Text Editor
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveDocument_Click(object sender, RoutedEventArgs e) {
            controller.BeginSaveDocument(textEditor.Document, null, null);
        }

        #endregion
    }
}
