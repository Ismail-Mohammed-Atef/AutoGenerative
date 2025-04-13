using Aspose.Cells.Charts;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using RevitAPI_Project1.App.Services;
using RevitAPI_Project1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RevitAPI_Project1
{
    [Transaction(TransactionMode.Manual)]
    internal class CreateColumnsInIntersectionsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            double distanceBetweenVerticalGrid = GridCreator.DistanceBetweenVerticalGrid; 
            double distanceBetweenHorizontalGrid = GridCreator.DistanceBetweenHorizontalGrid;

            if (GridCreator.HorizontalGrids.Count == 0 || GridCreator.VerticalGrids.Count == 0)
            {
                TaskDialog.Show("Error", "No grids found. Please create grids first.");
                return Result.Failed;
            }

            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UiDoc.Document;

            var columnTypes = new FilteredElementCollector(Doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(f => f.Family.FamilyCategory.Name == "Structural Columns")
                .ToList();

            var levels = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            if (columnTypes.Count == 0 || levels.Count == 0)
            {
                TaskDialog.Show("Error", "Missing column types or levels.");
                return Result.Failed;
            }

            List<string> columnNames = columnTypes.Select(c => c.Name).ToList();
            List<string> levelNames = levels.Select(l => l.Name).ToList();

            ColumnSelectionWindow selectionWindow = new ColumnSelectionWindow(columnNames, levelNames);
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(selectionWindow);
            helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;

            bool? dialogResult = selectionWindow.ShowDialog();
            if (dialogResult != true)
                return Result.Cancelled;

            string selectedName = selectionWindow.SelectedColumnType;
            string selectedLevelName = selectionWindow.SelectedLevelName;
            double offset = selectionWindow.OffsetValue;
            double rotationDegrees = selectionWindow.RotationValue;

            FamilySymbol selectedSymbol = columnTypes.FirstOrDefault(c => c.Name == selectedName);
            Level level = levels.FirstOrDefault(l => l.Name == selectedLevelName);

            XYZ originalstart = new XYZ(0, 0, 0);

            double offsetFeet = UnitUtils.ConvertToInternalUnits(offset, UnitTypeId.Meters);
            double rotationRadians = rotationDegrees * Math.PI / 180.0;

            try
            {
                using (Transaction transaction = new Transaction(Doc, "Add Columns"))
                {
                    transaction.Start();

                    if (!selectedSymbol.IsActive)
                        selectedSymbol.Activate();

                    for (int i = 0; i < GridCreator.HorizontalGrids.Count; i++)
                    {
                        for (int j = 0; j < GridCreator.VerticalGrids.Count; j++)
                        {
                            XYZ point = originalstart + new XYZ(j * distanceBetweenVerticalGrid, i * distanceBetweenHorizontalGrid, offsetFeet);
                            FamilyInstance column = Doc.Create.NewFamilyInstance(point, selectedSymbol, level, StructuralType.Column);

                            if (rotationRadians != 0)
                            {
                                Line axis = Line.CreateBound(point, point + XYZ.BasisZ);
                                ElementTransformUtils.RotateElement(Doc, column.Id, axis, rotationRadians);
                            }
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