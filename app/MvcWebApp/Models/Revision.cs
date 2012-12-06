using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace MvcWebApp.Models
{
    public class Revision
    {
        public int Id { get; set; }
        public int DocumentID { get; set; }
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public Int32 ContentHash { get; set; }
    }
}