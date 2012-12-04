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
        private Document document;

        public TextEditor(Document document) {
            InitializeComponent();
            this.document = document;
            TextField.Text = document.CurrentRevision;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            if (!TextField.Text.Equals(document.CurrentRevision)) { //if the text has been changed.
                document.CurrentRevision = TextField.Text;
                Controller.Instance.SaveDocument(document);
            }
        }
    }
}
