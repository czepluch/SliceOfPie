using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebApp.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public int ProjectID { get; set; }
        public int FolderID { get; set; }
        public string Title { get; set; }
    }
}