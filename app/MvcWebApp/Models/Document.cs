using System;
using System.Collections.Generic;

namespace MvcWebApp.Models
{
    public class Document
    {
        public int DocumentID { get; set; }
        public int ProjectID { get; set; }
        public int FolderID { get; set; }
        public string Title { get; set; }
        public string CurrentRevision { get; set; }
    }

}