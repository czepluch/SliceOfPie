using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace SliceOfPie.Client {
    public static class ImageUtil {
        /// <summary>
        /// This helper method returns a BitmapImage based on a filename.
        /// </summary>
        /// <param name="relativePath">The relative path to the icon. E.g. "/img/example.jpg" .</param>
        /// <returns>A BitmapImage version of the image</returns>
        public static BitmapImage CreateBitmapImage(string relativePath) {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,," + relativePath);
            image.EndInit();
            return image;
        }
    }
}
