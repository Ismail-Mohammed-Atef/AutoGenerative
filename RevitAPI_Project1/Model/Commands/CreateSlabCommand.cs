using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAPI_Project1.Model.DataContext;
using RevitAPI_Project1.View;
using RevitAPI_Project1.ViewModel;


namespace RevitAPI_Project1  // Must match the namespace in FullClassName
{
    [Transaction(TransactionMode.Manual)]
    public class CreateSlabCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UiDoc.Document;

            var levels = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .Select(l => new RvtLevel { LevelName = l.Name, Level = l })
                .ToList();

            // Get floor types and convert to RvtFloorType list
            var slabTypes = new FilteredElementCollector(Doc)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .Select(ft => new RvtFloorType { FloorName = ft.Name, FloorType = ft })
                .ToList();

            var FloorView = new CreateFloorView();
            var FloorVM = new CreateFloorVM(commandData, levels, slabTypes, FloorView);
            FloorView.DataContext = FloorVM;
            FloorView.ShowDialog();

            return Result.Succeeded;
        }
    }
}