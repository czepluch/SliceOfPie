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
        private ContextMenu projectContextMenu;
        private ContextMenu folderContextMenu;
        private ContextMenu documentContextMenu;
        public MainWindow() {
            InitializeComponent();
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
            documentContextMenu.Items.Add(new MenuItem() { Header = "Edit document" });
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
        /// This makes right click select an item in the Document Explorer and show the appropiate context menu for the item.
        /// </summary>
        /// <param name="sender">The object that send the event</param>
        /// <param name="e">The MouseButtonEventArgs for the event</param>
        private void DocumentExplorer_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            TreeViewItem item = e.Source as TreeViewItem;
            if (item != null) {
                item.IsSelected = true;
                generateContextMenu(item);
                e.Handled = true;
            }
        }

        // private void RefreshDocumentExplorer() {

        //    //var projects = Controller.getprojects();
        //    DocumentExplorer.Items.Clear();
        //    foreach (Project p in projects) {

        //item.Tag = "project";
        //        TreeViewItem project = new TreeViewItem() { Header=p.title, Tag="project" //Project node
        //        DocumentExplorer.Items.Add(TreeViewItem);
        //    }
        //}
    }

}
