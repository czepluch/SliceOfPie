using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebApp.Models
{
    public class ProjectUser
    {
        public int Id { get; set; }
        public int ProjectID { get; set; }
        public string UserEmail { get; set; }
    }
}