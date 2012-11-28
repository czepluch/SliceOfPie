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
        private ContextMenu projectContextMenu, folderContextMenu, documentContextMenu;
        private BitmapImage projectIcon, folderIcon, documentIcon;

        public MainWindow() {
            InitializeComponent();
            InitializeDocumentExplorer();
            RefreshDocumentExplorer();
            generateContent(DocumentExplorer.Items[0] as TreeViewItem); //Note there's always at least one project
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
            projectContextMenu.Items.Add(new MenuItem() { Header = "Add folder" });
            projectContextMenu.Items.Add(new MenuItem() { Header = "Add document" });

            //Setup Folder Context Menu for the Document Explorer
            folderContextMenu = new ContextMenu();
            folderContextMenu.Items.Add(new MenuItem() { Header = "Add folder" });
            folderContextMenu.Items.Add(new MenuItem() { Header = "Add document" });

            //Setup Document Context Menu for the Document Explorer
            documentContextMenu = new ContextMenu();
            MenuItem documentMenuItem1 = new MenuItem() { Header = "Edit document" };
            documentMenuItem1.Click += (object sender, RoutedEventArgs e) => { //lambda click handler
                    generateContent(DocumentExplorer.SelectedItem as TreeViewItem); //Opens the text editor for the document
                };
            documentContextMenu.Items.Add(documentMenuItem1);
        }

        /// <summary>
        /// This generates and shows a context menu for the document explorer at runtime
        /// </summary>
        /// <param name="item">The item which was clicked on the document explorer</param>
        private void generateContextMenu(TreeViewItem item) {
            string itemType = (string)item.Tag;
            if (itemType.Equals("project")) {
                DocumentExplorer.ContextMenu = projectContextMenu;
            }
            else if (itemType.Equals("folder")) {
                DocumentExplorer.ContextMenu = folderContextMenu;
            }
            else {
                DocumentExplorer.ContextMenu = documentContextMenu;
            }
            DocumentExplorer.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// This is a helper method for creating an item in the document explorer.
        /// </summary>
        /// <param name="text">The text to be shown in the item</param>
        /// <param name="itemType">The type of the item. Can be either "project", "folder", or "document"</param>
        /// <returns></returns>
        private TreeViewItem createDocumentExplorerItem(string text, string itemType) {
            TreeViewItem item = new TreeViewItem() {Tag = itemType };
            //StackPanel for image and text block
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
            //Create the image
            BitmapImage icon;
            if (itemType.Equals("project")) {
                icon = projectIcon;
            }
            else if (itemType.Equals("folder")) {
                icon = folderIcon;
            }
            else {
                icon = documentIcon;
            }
            Image image = new Image() { Source = icon, Height = 15, Width = 15, IsHitTestVisible = false }; /* note that IsHitTestVisible=false disables event handling for this element -
                                                                                                             * fallback on the general treeview handling (rightclick for the context menu).
                                                                                                             */
            sp.Children.Add(image);
            //Create the text block
            TextBlock itemText = new TextBlock() { Text = text, Margin = new Thickness(5, 0, 0, 0), IsHitTestVisible = false };
            sp.Children.Add(itemText);
            item.Header = sp;

            return item;
        }

        /// <summary>
        /// This method is supposed to refresh the document explorer.
        /// It exists as a placeholder and testing method untill the model/controller allows local file traversal.
        /// </summary>
        private void RefreshDocumentExplorer() {
            TreeViewItem t = createDocumentExplorerItem("My Project", "project");
            t.Items.Add(createDocumentExplorerItem("Work", "folder"));
            TreeViewItem schoolFolder = createDocumentExplorerItem("School", "folder");
            schoolFolder.Items.Add(createDocumentExplorerItem("BDSA_Report", "document"));
            t.Items.Add(schoolFolder);            

            TreeViewItem t2 = createDocumentExplorerItem("Other Project", "project");
            t2.Items.Add(createDocumentExplorerItem("Recipes", "folder"));
            t2.Items.Add(createDocumentExplorerItem("Tomato_soup", "document"));

            DocumentExplorer.Items.Add(t);
            DocumentExplorer.Items.Add(t2);

        //    //var projects = Controller.getprojects();
        //    DocumentExplorer.Items.Clear();
        //    foreach (Project p in projects) {

        //item.Tag = "project";
        //        TreeViewItem project = new TreeViewItem() { Header=p.title, Tag="project" //Project node
        //        DocumentExplorer.Items.Add(TreeViewItem);
        //    }
        }

        /// <summary>
        /// Fills the MainContent with useful information for the specific item
        /// </summary>
        /// <param name="item">The item which mainContent will use as a context</param>
        private void generateContent(TreeViewItem item) {
            string itemType = (string)item.Tag;
            if (itemType.Equals("project") || itemType.Equals("folder")) {
                FolderContentView f = new FolderContentView();

                for (int i = 0; i < 10; i++) {
                    StackPanel sp = new StackPanel() { Width = 50, Height = 50,  Orientation = Orientation.Vertical };
                    Image img = new Image() { Source = folderIcon, Width = 24, Height = 24 };
                    TextBlock label = new TextBlock() { Text = "Label", MaxWidth = 50, HorizontalAlignment = HorizontalAlignment.Center };
                    sp.Children.Add(img);
                    sp.Children.Add(label);
                    ListViewItem listViewItem = new ListViewItem() { Margin = new Thickness(2) };
                    listViewItem.Content = sp;
                    f.FolderListView.Items.Add(listViewItem);
                }
                MainContent.Content = f;
            }
            else {
                MainContent.Content = new TextEditor();
            }
            
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
                generateContextMenu(item);
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
                  generateContent(item);
              }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {

        }
    }

}
