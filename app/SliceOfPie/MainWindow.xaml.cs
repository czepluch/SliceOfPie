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
        public MainWindow() {
            InitializeComponent();
            //RefreshDocumentExplorer();
        }

        /// <summary>
        /// This generates and shows a context menu for the document explorer at runtime
        /// </summary>
        /// <param name="item">The item which was clicked on the document explorer</param>
        private void generateFolderContextMenu(TreeViewItem item) {
            ContextMenu c = new ContextMenu();
            string itemType = (string)item.Tag;
            if (itemType.Equals("project")) {
                c.Items.Add(new MenuItem() { Header = "Share project" });
                c.Items.Add(new MenuItem() { Header = "Add folder" });
                c.Items.Add(new MenuItem() { Header = "Add document" });
            }
            else if (itemType.Equals("folder")) {
                c.Items.Add(new MenuItem() { Header = "Add folder" });
                c.Items.Add(new MenuItem() { Header = "Add document" });
            }
            else {
                c.Items.Add(new MenuItem() { Header = "Edit document" });
            }
            DocumentExplorer.ContextMenu = c;
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
                generateFolderContextMenu(item);
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
