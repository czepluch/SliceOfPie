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
    /// Interaction logic for TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl {
        private Document _document; //backing field

        public event RoutedEventHandler SaveDocumentButtonClicked;

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

        public TextEditor() {
            InitializeComponent();

            SaveDocumentButton.Click += new RoutedEventHandler(
                (sender, e) => OnSaveDocumentButtonClicked(e) //fire own event
            );
        }

        #region Event triggers

        private void OnSaveDocumentButtonClicked(RoutedEventArgs e) {
            if (SaveDocumentButtonClicked != null) {
                SaveDocumentButtonClicked(this, e);
            }
        }

        #endregion
    }
}
