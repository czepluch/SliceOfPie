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
using SliceOfPie.ApmHelpers;

namespace SliceOfPie.Client {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private static SynchronizationContext syncContext = SynchronizationContext.Current; //for callbacks to be called on the UI thread

        private Controller controller;
        private ContentWrapper containerContentViewWrapper, textEditorWrapper;
        private ContainerContentView containerContentView;
        private TextEditor textEditor;

        private ContextMenu projectContextMenu, folderContextMenu, documentContextMenu;

        private IListableItem currentContextItem;
        private string imageStartTag = "<img src=\"", imageEndTag = "\" alt=\"\">";

        /// <summary>
        /// Creates the Main Window for the Slice of Pie application
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            CreateContextMenus();

            controller = Controller.Instance;

            //itemexplorer is "instantiated" in the xaml
            itemExplorer.ItemMouseLeftButtonUp += new EventHandler<ListableItemEventArgs>(ItemExplorerItemMouseLeftButtonUp);
            itemExplorer.ItemMouseRightButtonUp += new EventHandler<ListableItemEventArgs>(ItemExplorerItemMouseRightButtonUp);
            itemExplorer.ItemEnterKeyUp += new EventHandler<ListableItemEventArgs>(ItemExplorerItemEnterKeyUp);

            //ContainerContentViewWrapper
            containerContentViewWrapper = new ContentWrapper();
            containerContentViewWrapper.addMenuButton("Create document", "/Images/new-document.png", new RoutedEventHandler(OpenCreateDocumentWindow));
            containerContentViewWrapper.addMenuButton("Create folder", "/Images/new-folder.png", new RoutedEventHandler(OpenCreateFolderWindow));

            //Create the underlying containerContentView
            containerContentView = new ContainerContentView();
            containerContentView.ItemDoubleClicked += new EventHandler<ListableItemEventArgs>(ContainerContentView_DoubleClick);
            //Add to wrapper
            containerContentViewWrapper.Content = containerContentView;

            //TextEditorViewWrapper
            textEditorWrapper = new ContentWrapper();
            textEditorWrapper.addMenuButton("Save document", "/Images/save-document.png", new RoutedEventHandler(SaveDocumentButton_Click));
            textEditorWrapper.addMenuButton("Insert image", "/Images/insert-image.png", new RoutedEventHandler(OpenInsertImagePopUp));
            textEditorWrapper.addMenuButton("Show history", "/Images/show-history.png", new RoutedEventHandler(OpenHistoryPopUp));

            //Create the underlying text editor
            textEditor = new TextEditor();
            //Add to wrapper
            textEditorWrapper.Content = textEditor;

            RefreshLocalProjects();
        }

        /// <summary>
        /// This method refreshes the projects (local only)
        /// </summary>
        /// <param name="itemToOpen">The item to open, when the projects are reloaded. If this is null, the top project will be opened.</param>
        private void RefreshLocalProjects(IListableItem itemToOpen = null) {
            //Using controllers APM to load the projects into the Item Explorer
            controller.BeginGetProjects("local", (iar) =>
                Refresh(controller.EndGetProjects(iar), itemToOpen)
            , null);
        }

        /// <summary>
        /// This method refreshes the ui based on projects.
        /// </summary>
        /// <param name="projects">The projects to update the ui with</param>
        /// <param name="itemToOpen">The item to open, when the projects are reloaded. If this is null, the top project will be opened.</param>
        private void Refresh(IEnumerable<Project> projects, IListableItem itemToOpen = null) {
            //Callback posted in UI-context
            CallOnUIThread(() => {
                itemExplorer.Projects = projects;
                if (itemToOpen != null) {
                    itemExplorer.ExpandTo(itemToOpen, Open);
                }
                else {
                    itemExplorer.CallbackSelected(Open);
                }
            });
        }

        /// <summary>
        /// Calls a delegate on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to call</param>
        private void CallOnUIThread(Action action) {
            //Callback posted in UI-context
            syncContext.Post((o) => action(), null);
        }

        /// <summary>
        /// Fills the MainContent with useful information for the specific item
        /// </summary>
        /// <param name="item">The item which mainContent will use as a context.</param>
        private void Open(IListableItem item) {
            currentContextItem = item;
            if (item is IItemContainer) {
                containerContentView.ItemContainer = item as IItemContainer;
                mainContent.Content = containerContentViewWrapper;
            }
            else {
                textEditor.Document = item as Document;
                mainContent.Content = textEditorWrapper;
            }
        }

        /// <summary>
        /// Sets up the ItemExplorer's context menus
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
            controller.BeginCreateProject(createProjectPopUPTextBox.Text, "local",
                (iar) => {
                    try {
                        RefreshLocalProjects(controller.EndCreateProject(iar));
                    }
                    catch (Exception ex) {
                        if (ex is AsyncException) {
                            CallOnUIThread(() => {
                                OpenMessagePopUp("An error occured. The project was not created.");
                            });
                        }
                        else {
                            throw;
                        }
                    }
                }, null);
            createProjectPopUp.IsOpen = false;
            IsEnabled = true;
            createProjectPopUPTextBox.Clear();
            ;
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
            controller.BeginCreateFolder(createFolderPopUPTextBox.Text, "local", currentContextItem as IItemContainer,
                (iar) => {
                    try {
                        RefreshLocalProjects(controller.EndCreateFolder(iar));
                    }
                    catch (Exception ex) {
                        if (ex is AsyncException) {
                            CallOnUIThread(() => {
                                OpenMessagePopUp("An error occured. The folder was not created.");
                            });
                        }
                        else {
                            throw;
                        }
                    }
                }, null);
            createFolderPopUP.IsOpen = false;
            IsEnabled = true;
            createFolderPopUPTextBox.Clear();

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
            controller.BeginCreateDocument(createDocumentPopUPTextBox.Text, "local", currentContextItem as IItemContainer,
                (iar) => {
                    try {
                        RefreshLocalProjects(controller.EndCreateDocument(iar));
                    }
                    catch (Exception ex) {
                        if (ex is AsyncException) {
                            CallOnUIThread(() => {
                                OpenMessagePopUp("An error occured. The document was not created.");
                            });
                        }
                        else {
                            throw;
                        }
                    }
                }, null);
            createDocumentPopUP.IsOpen = false;
            IsEnabled = true;
            createDocumentPopUPTextBox.Clear();
        }

        /// <summary>
        /// This event handler opens the Login pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenLoginWindow(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            IsEnabled = false;
            loginPopUpErrorLabel.Content = " "; //to prevent the lines jumping when the error label gets set and changes height
            loginPopUp.IsOpen = true;
            loginPopUpUserTextBox.Focus();
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the Login pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LoginPopUpCancelButton_Click(object sender, RoutedEventArgs e) {
            loginPopUp.IsOpen = false;
            IsEnabled = true;
            loginPopUpUserTextBox.Clear();
            loginPopUpPasswordBox.Clear();
            loginPopUpErrorLabel.Content = "";
        }

        /// <summary>
        /// This is the click handler for the Share button in the Login pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void loginPopUpLoginButton_Click(object sender, RoutedEventArgs e) {
            if (loginPopUpUserTextBox.Text.Length > 0 && loginPopUpPasswordBox.Password.Length > 0) {
                loginPopUpCancelButton.IsEnabled = false;
                loginPopUpLoginButton.IsEnabled = false;
                syncingPopUp.IsOpen = true;
                controller.BeginSyncProjects(loginPopUpUserTextBox.Text, loginPopUpPasswordBox.Password,
                    (iar) => {
                        try {
                            Refresh(controller.EndSyncProjects(iar));
                            CallOnUIThread(() => {
                                syncingPopUp.IsOpen = false;
                                loginPopUpCancelButton.IsEnabled = true;
                                loginPopUpLoginButton.IsEnabled = true;
                                loginPopUp.IsOpen = false;
                                IsEnabled = true;
                                loginPopUpUserTextBox.Clear();
                                loginPopUpPasswordBox.Clear();
                                loginPopUpErrorLabel.Content = "";
                            });
                        }
                        catch (Exception ex) {
                            if (ex is AsyncException) {
                                CallOnUIThread(() => {
                                    loginPopUpErrorLabel.Content = "Synchronization failed.";
                                    syncingPopUp.IsOpen = false;
                                    loginPopUpCancelButton.IsEnabled = true;
                                    loginPopUpLoginButton.IsEnabled = true;
                                });
                            }
                            else {
                                throw;
                            }
                        }
                    }, null);
            }
            else {
                loginPopUpErrorLabel.Content = "Please enter both email and password.";
            }
        }

        /// <summary>
        /// Opens a pop-up with a message
        /// </summary>
        private void OpenMessagePopUp(string topLine = null, string bottomLine = null) {
            IsEnabled = false;
            messagePopUpLabel1.Content = topLine ?? " ";
            messagePopUpLabel2.Content = bottomLine ?? " ";
            messagePopUp.IsOpen = true;

        }

        /// <summary>
        /// This is the click handler for the Close button in the Message pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void MessagePopUpOkButton_Click(object sender, RoutedEventArgs e) {
            messagePopUp.IsOpen = false;
            IsEnabled = true;
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
        /// This method is sent as a mouse event handler to the ContainerContentView class.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ContainerContentView_DoubleClick(object sender, ListableItemEventArgs e) {
            itemExplorer.ExpandTo(e.Item, Open);
        }

        #region Item Explorer related
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
            RefreshLocalProjects();
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
            controller.BeginShareProject(currentContextItem as Project, shareProjectPopUPTextBox.Text.Split(','),
                (iar) => {
                    try {
                        controller.EndShareProject(iar);
                    }
                    catch (Exception ex) {
                        if (ex is AsyncException) {
                            CallOnUIThread(() => {
                                OpenMessagePopUp("There was an error locating your project on the server.", "Make sure it is synchronized and check your internet connection.");
                            });
                        }
                        else {
                            throw;
                        }
                    }
                }
                , null);
            shareProjectPopUP.IsOpen = false;
            IsEnabled = true;
            shareProjectPopUPTextBox.Clear();
        }

        /// <summary>
        /// This is the event handler for a right click (up event) in the Item Explorer
        /// </summary>
        /// <param name="sender">The object that send the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ItemExplorerItemMouseRightButtonUp(object sender, ListableItemEventArgs e) {
            currentContextItem = e.Item;
            itemExplorer.ShowContextMenuForSelected(GetContextMenu(e.Item));
        }

        /// <summary>
        /// This is the event handler for a left click (up event) in the Item Explorer
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ItemExplorerItemMouseLeftButtonUp(object sender, ListableItemEventArgs e) {
            currentContextItem = e.Item;
            Open(e.Item);
        }

        /// <summary>
        /// This is the event handler for an enter key event in the Item Explorer
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ItemExplorerItemEnterKeyUp(object sender, ListableItemEventArgs e) {
            currentContextItem = e.Item;
            Open(e.Item);
        }

        #endregion

        #region Text editor related

        /// <summary>
        /// Saves the current document
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveDocumentButton_Click(object sender, RoutedEventArgs e) {
            Document docToSave = textEditor.Document;
            docToSave.CurrentRevision = textEditor.Text; //Update current revision based on text before returning
            controller.BeginSaveDocument(docToSave,
                (iar) => {
                    try {
                        controller.EndSaveDocument(iar);
                    }
                    catch (Exception ex) {
                        if (ex is AsyncException) {
                            CallOnUIThread(() => {
                                OpenMessagePopUp("The document could not be saved.");
                            });
                        }
                        else {
                            throw;
                        }
                    }
                }, null);
        }

        /// <summary>
        /// This event handler opens the Insert Image pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenInsertImagePopUp(object sender, RoutedEventArgs e) {
            //note that the textbox is cleared when the popups were last closed
            IsEnabled = false;
            insertImagePopUp.IsOpen = true;
            insertImagePopUpTextBox.Focus();
        }

        /// <summary>
        /// This event handler opens the History pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenHistoryPopUp(object sender, RoutedEventArgs e) {
            Document documentWhenClicked = textEditor.Document;
            //setup history popup
            syncingPopUp.IsOpen = true;
            controller.BeginDownloadRevisions(documentWhenClicked,
                (iar) => {
                    CallOnUIThread(() => historyList.Items.Clear());
                    try {
                        IEnumerable<Revision> revisions = controller.EndDownloadRevisions(iar);
                        foreach (Revision revision in revisions) {
                            string revisionContent = revision.Content;
                            DateTime? revisionTimeStamp = revision.Timestamp;
                            CallOnUIThread(() => {
                                ListBoxItem item = new ListBoxItem() { Content = revisionTimeStamp };
                                item.Selected += new RoutedEventHandler((itemSelected, eventArgs) => historyPopUpTextBox.Text = revisionContent);
                                historyList.Items.Add(item);
                            });
                        }
                        //If there was any revisions - select the first
                        if (revisions.Count() > 0) {
                            CallOnUIThread(() => {
                                (historyList.Items[0] as ListBoxItem).IsSelected = true;
                            });
                        }
                        CallOnUIThread(() => {
                            historyPopUpTopLabel.Content = "History for " + documentWhenClicked.Title;
                            IsEnabled = false;
                            syncingPopUp.IsOpen = false;
                            historyPopUp.IsOpen = true;
                        });
                    }
                    catch (Exception ex) {
                        if (ex is AsyncException) {
                            CallOnUIThread(() => {
                                syncingPopUp.IsOpen = false;
                                OpenMessagePopUp("There was an error locating your document on the server.", "Make sure it is synchronized and check your internet connection.");
                            });
                        }
                        else {
                            throw;
                        }
                    }
                }, null);
        }

        /// <summary>
        /// This is the click handler for the Cancel button in the Insert Image pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void InsertImagePopUpCancelButton_Click(object sender, RoutedEventArgs e) {
            insertImagePopUp.IsOpen = false;
            IsEnabled = true;
            insertImagePopUpTextBox.Clear();
        }

        /// <summary>
        /// This is the click handler for the Insert button in the Insert Image pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void InsertImagePopUpInsertButton_Click(object sender, RoutedEventArgs e) {
            string link = insertImagePopUpTextBox.Text;
            int caretIndex = textEditor.CaretIndex;
            textEditor.Text = textEditor.Text.Insert(caretIndex, imageStartTag + link + imageEndTag);
            textEditor.CaretIndex = caretIndex + imageStartTag.Length + link.Length + imageEndTag.Length;
            insertImagePopUp.IsOpen = false;
            IsEnabled = true;
            insertImagePopUpTextBox.Clear();
            textEditor.FocusText();
        }

        /// <summary>
        /// This is the click handler for the Close button in the History pop-up
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HistoryPopUpCloseButton_Click(object sender, RoutedEventArgs e) {
            historyPopUp.IsOpen = false;
            IsEnabled = true;
        }

        #endregion

        #endregion












    }
}
