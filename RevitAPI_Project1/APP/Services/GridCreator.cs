using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Autodesk.Revit.DB;

namespace RevitAPI_Project1.App.Services
{
    public class GridCreator
    {
        public static List<Grid> HorizontalGrids { get;  set; } = new List<Grid>();
        public static List<Grid> VerticalGrids { get;  set; } = new List<Grid>();

        public static double DistanceBetweenHorizontalGrid { get; set; }
        public static double DistanceBetweenVerticalGrid { get; set; }
        public static int NumberOfHorizontalGrids { get; set; }
        public static int NumberOfVerticalGrids { get; set; }

        public static void CreateGrids(Document doc,
                                    double distanceBetweenHorizontalGrid,
                                    double distanceBetweenVerticalGrid,
                                    int numberOfHorizontalGrid,
                                    int numberOfVerticalGrid)
        {
            HorizontalGrids.Clear();
            VerticalGrids.Clear();

            DistanceBetweenHorizontalGrid = distanceBetweenHorizontalGrid;
            DistanceBetweenVerticalGrid = distanceBetweenVerticalGrid;
            NumberOfHorizontalGrids = numberOfHorizontalGrid;
            NumberOfVerticalGrids = numberOfVerticalGrid;


            XYZ originalStart = new XYZ(0, 0, 0);

            // Create horizontal grids (in Y direction)
            for (int i = 0; i < numberOfHorizontalGrid; i++)
            {
                XYZ startPoint = originalStart + new XYZ(-20, i * distanceBetweenHorizontalGrid, 0);
                XYZ endPoint = startPoint + new XYZ(((numberOfVerticalGrid - 1) * distanceBetweenVerticalGrid) + 40, 0, 0);

                Line gridLine = Line.CreateBound(startPoint, endPoint);
                Grid grid = Grid.Create(doc, gridLine);
                HorizontalGrids.Add(grid);
            }

            // Create vertical grids (in X direction)
            for (int i = 0; i < numberOfVerticalGrid; i++)
            {
                XYZ startPoint = originalStart + new XYZ(i * distanceBetweenVerticalGrid, -20, 0);
                XYZ endPoint = startPoint + new XYZ(0, ((numberOfHorizontalGrid - 1) * distanceBetweenHorizontalGrid) + 40, 0);

                Line gridLine = Line.CreateBound(startPoint, endPoint);
                Grid grid = Grid.Create(doc, gridLine);
                VerticalGrids.Add(grid);
            }
        }
    }
}
