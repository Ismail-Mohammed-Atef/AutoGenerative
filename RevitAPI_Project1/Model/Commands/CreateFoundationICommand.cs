using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using RevitAPI_Project1.App.Services;
using RevitAPI_Project1.ViewModel;

namespace RevitAPI_Project1
{
    [Transaction(TransactionMode.Manual)]
    internal class CreateFoundationICommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            double distanceBetweenVerticalGrid = GridCreator.DistanceBetweenVerticalGrid;
            double distanceBetweenHorizontalGrid = GridCreator.DistanceBetweenHorizontalGrid;

            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UiDoc.Document;

            var foundationTypes = new FilteredElementCollector(Doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(f => f.Family.FamilyCategory.Name == "Structural Foundations")
                .ToList();

            var levels = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            if (foundationTypes.Count == 0 || levels.Count == 0)
            {
                TaskDialog.Show("Error", "Missing foundation types or levels.");
                return Result.Failed;
            }

            List<string> foundationNames = foundationTypes.Select(c => c.Name).ToList();
            List<string> levelNames = levels.Select(l => l.Name).ToList();

            FoundationSelectionWindow selectionWindow = new FoundationSelectionWindow(foundationNames, levelNames);
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(selectionWindow);
            helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;

            bool? dialogResult = selectionWindow.ShowDialog();
            if (dialogResult != true)
                return Result.Cancelled;

            string selectedName = selectionWindow.SelectedFoundationType;
            string selectedLevelName = selectionWindow.SelectedLevelName;

            FamilySymbol selectedSymbol = foundationTypes.FirstOrDefault(c => c.Name == selectedName);
            Level level = levels.FirstOrDefault(l => l.Name == selectedLevelName);

            XYZ originalstart = new XYZ();

            try
            {
                using (Transaction transaction = new Transaction(Doc, "Create Foundations"))
                {
                    transaction.Start();

                    if (!selectedSymbol.IsActive)
                        selectedSymbol.Activate();

                    for (int i = 0; i < GridCreator.HorizontalGrids.Count; i++)
                    {
                        for (int j = 0; j < GridCreator.VerticalGrids.Count; j++)
                        {
                            XYZ point = originalstart + new XYZ(j * distanceBetweenVerticalGrid, i * distanceBetweenHorizontalGrid, 0);
                            Doc.Create.NewFamilyInstance(point, selectedSymbol, level, StructuralType.Footing);
                        }
                    }

                    transaction.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }

}
