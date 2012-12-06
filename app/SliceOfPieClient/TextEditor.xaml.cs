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
using SliceOfPie;

namespace SliceOfPie.Client {
    /// <summary>
    /// Interaction logic for the Text Editor User Control.
    /// </summary>
    public partial class TextEditor : UserControl {
        private Document _document; //backing field

        /// <summary>
        /// This is the Document currently shown in the Text Editor
        /// </summary>
        public Document Document {
            get {
                if (_document != null) { //if it has been set at least once
                    _document.CurrentRevision = TextField.Text; //Update revision based on text before returning
                }
                return _document;
            }
            set {
                _document = value;
                TextField.Text = _document.CurrentRevision;
            }
        }

        /// <summary>
        /// This event is fired when the button to save the active document is clicked
        /// </summary>
        public event RoutedEventHandler SaveDocumentButtonClicked;

        /// <summary>
        /// Creates a Text Editor with the functionality to show and edit Documents
        /// </summary>
        public TextEditor() {
            InitializeComponent();

            SaveDocumentButton.Click += new RoutedEventHandler(
                (sender, e) => OnSaveDocumentButtonClicked(e) //fire own event
            );
        }

        #region Event triggers

        /// <summary>
        /// This method triggers the SaveDocumentButtonClicked event
        /// </summary>
        /// <param name="e">The event arguments</param>
        private void OnSaveDocumentButtonClicked(RoutedEventArgs e) {
            if (SaveDocumentButtonClicked != null) {
                SaveDocumentButtonClicked(this, e);
            }
        }

        #endregion
    }
}
