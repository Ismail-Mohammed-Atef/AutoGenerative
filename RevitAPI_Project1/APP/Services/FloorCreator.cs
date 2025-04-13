using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAPI_Project1.Model.DataContext;

namespace RevitAPI_Project1.App.Services
{
    public class FloorCreator
    {
        static double distanceBetweenVerticalGrid = GridCreator.DistanceBetweenVerticalGrid;
        static double distanceBetweenHorizontalGrid = GridCreator.DistanceBetweenHorizontalGrid;

        static int NumberOfHorizontalGrid = GridCreator.NumberOfHorizontalGrids;
        static int NumberOfVerticalGrid = GridCreator.NumberOfVerticalGrids;

        public static void CreateFloor(Document doc , RvtFloorType SlabType , RvtLevel level)
        {
            int noOFHzGrid = NumberOfHorizontalGrid - 1;
            double horizontal = noOFHzGrid * distanceBetweenVerticalGrid;

            int noOFvcGrid = NumberOfVerticalGrid - 1;
            double vertical = noOFvcGrid * distanceBetweenHorizontalGrid;

            XYZ originalstart = new XYZ(0, 0, 0);

            Line line1 = Line.CreateBound(originalstart + new XYZ(0, 0, 300), originalstart + new XYZ(horizontal, 0, 300));
            Line line4 = Line.CreateBound(originalstart + new XYZ(horizontal, 0, 300), originalstart + new XYZ(horizontal, vertical, 300));
            Line line2 = Line.CreateBound(originalstart + new XYZ(horizontal, vertical, 300), originalstart + new XYZ(0, vertical, 300));
            Line line3 = Line.CreateBound(originalstart + new XYZ(0, vertical, 300), originalstart + new XYZ(0, 0, 300));

            CurveLoop curvloop = CurveLoop.Create(new List<Curve>()
                {
                    line1, line4, line2, line3
                });

            try
            {
                Floor.Create(doc, new List<CurveLoop>() { curvloop }, SlabType.FloorType.Id, level.Level.Id);
            }
            catch
            {
                TaskDialog.Show("Not found", "Floor type not found , please load the family for type (Generic 300mm)!");
            }
        }
    }
}
