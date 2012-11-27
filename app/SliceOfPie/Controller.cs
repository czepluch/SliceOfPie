using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class Controller {
		FileModel fileModel = FileModel.Instance;
		
		public IEnumerable<Project> GetProjects() {
			foreach(Project p in fileModel.GetProjects()) {
				yield return p;
			}
		}
		
    }
}
