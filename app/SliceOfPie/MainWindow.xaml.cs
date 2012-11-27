﻿using System;
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
        }

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
            TreeViewItem t = createDocumentExplorerItem("test project", "project");
            t.Items.Add(createDocumentExplorerItem("test folder", "folder"));
            t.Items.Add(createDocumentExplorerItem("test document", "document"));
            DocumentExplorer.Items.Add(t);

        //    //var projects = Controller.getprojects();
        //    DocumentExplorer.Items.Clear();
        //    foreach (Project p in projects) {

        //item.Tag = "project";
        //        TreeViewItem project = new TreeViewItem() { Header=p.title, Tag="project" //Project node
        //        DocumentExplorer.Items.Add(TreeViewItem);
        //    }
        }
    }

}
