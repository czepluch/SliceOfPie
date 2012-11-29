using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace SliceOfPie {
    /// <summary>
    /// This is a factory for standard icons in the SliceOfPie user interface.
    /// </summary>
    static class IconFactory {

        private static BitmapImage
            _projectIcon = createBitmapImage("project-icon"),
            _folderIcon = createBitmapImage("folder-icon"),
            _documentIcon = createBitmapImage("document-icon");

        /// <summary>
        /// This is the default icon for projects
        /// </summary>
        public static BitmapImage ProjectIcon {
            get { return _projectIcon; }
        }

        /// <summary>
        /// This is the default icon for projects
        /// </summary>
        public static BitmapImage FolderIcon {
            get { return _folderIcon; }
        }

        /// <summary>
        /// This is the default icon for projects
        /// </summary>
        public static BitmapImage DocumentIcon {
            get { return _documentIcon; }
        }

        /// <summary>
        /// This helper method returns a BitmapImage based on a filename.
        /// Note that this method only works with .bmp files.
        /// </summary>
        /// <param name="iconFileName">The name of the file without file extension</param>
        /// <returns>A BitmapImage version of the icon</returns>
        private static BitmapImage createBitmapImage(string iconFileName) {
            BitmapImage icon = new BitmapImage();
            icon.BeginInit();
            icon.UriSource = new Uri("pack://application:,,,/Icons/" + iconFileName + ".bmp");
            icon.EndInit();
            return icon;
        }
    }
}
