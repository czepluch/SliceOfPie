using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace SliceOfPie.Client {
    /// <summary>
    /// This is a factory for standard icons in the SliceOfPie user interface.
    /// </summary>
    public static class IListableItemExtension {

        private static BitmapImage
            projectIcon = ImageUtil.CreateBitmapImage("/Icons/project-icon.png"),
            folderIcon = ImageUtil.CreateBitmapImage("/Icons/folder-icon.png"),
            documentIcon = ImageUtil.CreateBitmapImage("/Icons/document-icon.png"),
            projectConflictIcon = ImageUtil.CreateBitmapImage("/Icons/project-icon-conflict.png"),
            folderConflictIcon = ImageUtil.CreateBitmapImage("/Icons/folder-icon-conflict.png"),
            documentConflictIcon = ImageUtil.CreateBitmapImage("/Icons/document-icon-conflict.png");

        /// <summary>
        /// Returns the icon for this IListabeItem
        /// </summary>
        /// <param name="item">The IListableItem this method is invoked on.</param>
        /// <returns>The icon of the item</returns>
        public static BitmapImage GetIcon(this IListableItem item) {
            if (item is Project) {
                return item.hasConflict() ? projectConflictIcon : projectIcon;
            }
            else if (item is Folder) {
                return item.hasConflict() ? folderConflictIcon : folderIcon;
            }
            else {
                return item.hasConflict()? documentConflictIcon : documentIcon;
            }
        }

        /// <summary>
        /// Returns whether or not this item is/has a merged document
        /// </summary>
        /// <param name="item">The IListableItem this method is invoked on.</param>
        /// <returns>True if the item is/has a merged document.</returns>
        private static bool hasConflict(this IListableItem item) {
            //If this is a document and is set as merged
            if (item is Document) {
                return (item as Document).IsMerged;
            }
            else { //or if it's a folder containing a document set as merged
                foreach (Document subDoc in (item as IItemContainer).GetDocuments()) {
                    if (subDoc.IsMerged) return true;
                }
                //or if it contains a folder set as merged (recursively)
                foreach (IListableItem subFolder in (item as IItemContainer).GetFolders()) {
                    if (subFolder.hasConflict()) return true;
                }
                return false;
            }
        }
    }
}
