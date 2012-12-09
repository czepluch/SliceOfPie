using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie.Tests {
    public static class TestHelper {
        public static void ClearDatabase(string email) {
            using (var dbContext = new sliceofpieEntities2()) {

            }
        }

        public static void ClearFolder(string path) {
            if (Directory.Exists(path)) {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo file in dir.GetFiles()) {
                    file.Delete();
                }
                foreach (DirectoryInfo folder in dir.GetDirectories()) {
                    ClearFolder(folder.FullName);
                    folder.Delete();
                }
            }
        }
    }
}
