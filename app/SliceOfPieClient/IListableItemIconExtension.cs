using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace SliceOfPie.Client {
    /// <summary>
    /// This is a factory for standard icons in the SliceOfPie user interface.
    /// </summary>
    public static class IListableItemIconExtension {

        private static BitmapImage
            projectIcon = ImageUtil.CreateBitmapImage("/Icons/project-icon.png"),
            folderIcon = ImageUtil.CreateBitmapImage("/Icons/folder-icon.png"),
            documentIcon = ImageUtil.CreateBitmapImage("/Icons/document-icon.png"),
            documentConflictIcon = ImageUtil.CreateBitmapImage("/Icons/document-icon-conflict.png");

        /// <summary>
        /// Returns the icon for this IListabeItem
        /// </summary>
        /// <param name="item">The IListableItem this method is invoked on.</param>
        /// <returns>The icon of the item</returns>
        public static BitmapImage GetIcon(this IListableItem item) {
            if (item is Project) {
                return projectIcon;
            }
            else if (item is Folder) {
                return folderIcon;
            }
            else {
                return (item as Document).IsMerged? documentConflictIcon : documentIcon;
            }
        }

        
    }
}
