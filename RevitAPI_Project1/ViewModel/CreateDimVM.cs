using Autodesk.Revit.DB;
using RevitAPI_Project1.App.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RevitAPI_Project1.ViewModel
{
    public class CreateDimVM :INotifyPropertyChanged
    {
        #region Notify Property Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void onPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Fields And Prpoperties
        private bool _totalDimensions;
        private bool _indvDimensions;
        private double _totalDimOffset;
        private double _indvDimOffset;

        public bool TotalDimensions
        {
            get { return _totalDimensions; }
            set
            {
                _totalDimensions = value;
                onPropertyChanged();
            }
        }
        public bool IndvDimensions
        {
            get { return _indvDimensions; }
            set
            {
                _indvDimensions = value;
                onPropertyChanged();
            }
        }
        public double TotalDimOffset
        {
            get { return _totalDimOffset; }
            set
            {
                _totalDimOffset = value;
                onPropertyChanged();
            }
        }
        public double IndvDimOffset
        {
            get { return _indvDimOffset; }
            set
            {
                _indvDimOffset = value;
                onPropertyChanged();
            }
        }

        public MyCommand CreateDimensionCommand { get; set; }
        #endregion

        public CreateDimVM()
        {
            CreateDimensionCommand = new MyCommand(createDimension);
        }
        private void createDimension()
        {
            Document Doc = CreateDimensionsCommand.document;

            using (Transaction dimensionTransaction = new Transaction(Doc, "Create Dimensions"))
            {
                dimensionTransaction.Start();

                CreateDimensions(Doc, CreateGridCommand.horizontalGrids, true, CreateGridCommand.DistanceBetweenHorizontalGrid ,TotalDimOffset,IndvDimOffset,TotalDimensions,IndvDimensions );
                CreateDimensions(Doc, CreateGridCommand.verticalGrids, false, CreateGridCommand.DistanceBetweenVerticalGrid,TotalDimOffset,IndvDimOffset, TotalDimensions, IndvDimensions);

                dimensionTransaction.Commit();
            }
        }


        private void CreateDimensions(Document doc, List<Autodesk.Revit.DB.Grid> grids, bool isHorizontal, double gridSpacing, double totalDimOffest, double IndvDimOffest, bool totalDim , bool indvDim)
        {
            // Collect references
            List<Reference> references = new List<Reference>();
            foreach (var grid in grids)
            {
                if (grid.Curve == null)
                {
                    continue; // Skip invalid grids
                }
                references.Add(new Reference(grid));
            }

            if (references.Count < 2)
                return; // Need at least two grids to create dimensions

            // Define offsets for dimension placement
            double totalDimensionOffset = gridSpacing * totalDimOffest; // Offset for total dimension
            double individualDimensionOffset = gridSpacing * IndvDimOffest; // Offset for individual dimensions

            // Total dimension offset direction (opposite to individual dimensions)
            XYZ totalOffsetDirection = isHorizontal ? new XYZ(-totalDimensionOffset, 0, 0) : new XYZ(0, -totalDimensionOffset, 0);
            XYZ totalOffsetDirectionEnd = isHorizontal ? new XYZ(totalDimensionOffset, 0, 0) : new XYZ(0, totalDimensionOffset, 0);

            // Individual dimension offset direction (opposite to total dimension)
            XYZ individualOffsetDirection = isHorizontal ? new XYZ(-individualDimensionOffset, 0, 0) : new XYZ(0, -individualDimensionOffset, 0);
            XYZ individualOffsetDirectionEnd = isHorizontal ? new XYZ(individualDimensionOffset, 0, 0) : new XYZ(0, individualDimensionOffset, 0);

            // Total Dimension (First to Last)
            Line totalDimensionLine = Line.CreateBound(
                grids[0].Curve.GetEndPoint(0) + totalOffsetDirection,
                grids[grids.Count - 1].Curve.GetEndPoint(0) + totalOffsetDirection
            );
            Line totalDimensionLineEnd = Line.CreateBound(
                grids[0].Curve.GetEndPoint(1) + totalOffsetDirectionEnd,
                grids[grids.Count - 1].Curve.GetEndPoint(1) + totalOffsetDirectionEnd
            );

            ReferenceArray totalReferenceArray = new ReferenceArray();
            totalReferenceArray.Append(references[0]);
            totalReferenceArray.Append(references[grids.Count - 1]);
            if (totalDim)
            {
                // Create total dimension
                Dimension totalDimension = doc.Create.NewDimension(doc.ActiveView, totalDimensionLine, totalReferenceArray);
                Dimension totalDimensionEnd = doc.Create.NewDimension(doc.ActiveView, totalDimensionLineEnd, totalReferenceArray);
            }
            // Individual Dimensions
            if (indvDim)
            {
                for (int i = 0; i < references.Count - 1; i++)
                {
                    Line individualDimensionLine = Line.CreateBound(
                        grids[i].Curve.GetEndPoint(0) + individualOffsetDirection,
                        grids[i + 1].Curve.GetEndPoint(0) + individualOffsetDirection
                    );
                    Line individualDimensionLineEnd = Line.CreateBound(
                        grids[i].Curve.GetEndPoint(1) + individualOffsetDirectionEnd,
                        grids[i + 1].Curve.GetEndPoint(1) + individualOffsetDirectionEnd
                    );

                    ReferenceArray individualRefs = new ReferenceArray();
                    individualRefs.Append(references[i]);
                    individualRefs.Append(references[i + 1]);

                    Dimension individualDimension = doc.Create.NewDimension(doc.ActiveView, individualDimensionLine, individualRefs);
                    Dimension individualDimensionEnd = doc.Create.NewDimension(doc.ActiveView, individualDimensionLineEnd, individualRefs);

                }
            }
        }
    }
}
