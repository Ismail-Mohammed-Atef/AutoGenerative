using Autodesk.Revit.DB;

namespace RevitAPI_Project1.Model.DataContext
{
    public class RvtFloorType
    {
        public string FloorName { get; set; }

        public FloorType FloorType { get; set; }
    }
}
