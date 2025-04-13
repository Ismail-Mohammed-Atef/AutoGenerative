using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitAPI_Project1.Model.DataContext
{
    public class RvtLevel
    {
        public string LevelName { get; set; }
        
        public Level Level { get; set; }
    }
}
