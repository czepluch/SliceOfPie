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
using SliceOfPie;

namespace SliceOfPie.Client {
    /// <summary>
    /// Interaction logic for the Text Editor User Control.
    /// This UserControl shows a Text Editor based on its Document property.
    /// </summary>
    public partial class TextEditor : UserControl {
        private Document _document; //backing field
        private Controller controller;

        /// <summary>
        /// This is the Document currently shown in the Text Editor
        /// Changing this will change what is shown.
        /// </summary>
        public Document Document {
            get {
                if (_document != null) { //if it has been set at least once
                    _document.CurrentRevision = textField.Text; //Update revision based on text before returning
                }
                return _document;
            }
            set {
                //remove potential open popups
                insertImagePopUp.IsOpen = false;
                IsEnabled = true;
                insertImagePopUpTextBox.Clear();
                //update ui to the new document
                _document = value;
                textField.Text = _document.CurrentRevision;
            }
        }

        /// <summary>
        /// This event is fired when the button to save the active document is clicked
        /// </summary>
        public event RoutedEventHandler SaveDocumentButtonClicked, InsertImageButtonClicked, ShowHistoryButtonClicked;
        
        /// <summary>
        /// Creates a Text Editor with the functionality to show and edit Documents
        /// For content to be shown, the Document property must be set.
        /// </summary>
        public TextEditor() {
            InitializeComponent();
            controller = Controller.Instance;
        }

        /// <summary>
        /// Saves the current document
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveDocumentButton_Click(object sender, RoutedEventArgs e) {
            controller.BeginSaveDocument(Document, null, null);
           
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
            textField.Text = textField.Text.Insert(textField.CaretIndex, "<IMAGEURL{" + insertImagePopUpTextBox.Text + "}>");
            insertImagePopUp.IsOpen = false;
            IsEnabled = true;            
            insertImagePopUpTextBox.Clear();
        }

        /// <summary>
        /// This event handler opens the History pop-up window
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenHistoryPopUp(object sender, RoutedEventArgs e) {
            //setup history popup
            SetUpHistoryPopUp();
            historyPopUpTopLabel.Content = "History for " + Document.Title;
            IsEnabled = false;
            historyPopUp.IsOpen = true;
            //Select top entry
        }

        private void SetUpHistoryPopUp() {
            historyList.Items.Clear();
            foreach (Revision revision in Document.Revisions) {
                ListBoxItem item = new ListBoxItem() { Content = revision.Timestamp};
                item.MouseLeftButtonUp += new MouseButtonEventHandler((sender, e) => historyTextBox.Text = revision.Content);
                historyList.Items.Add(item);
            }
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
    }
}
