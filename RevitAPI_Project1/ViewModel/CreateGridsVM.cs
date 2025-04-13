using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAPI_Project1.App.Command;
using RevitAPI_Project1.App.Services;

namespace RevitAPI_Project1.ViewModel
{
    public class CreateGridsVM : INotifyPropertyChanged
    {

        #region Constructor

        private readonly ExternalCommandData _commandData;
        private readonly Window _window;
        public CreateGridsVM(ExternalCommandData commandData = null, Window window = null)
        {
            _commandData = commandData;
            _window = window;
            CreateGridsCommand = new MyCommand(ExcuteCmd, CanExcuteCmd);
        }
        #endregion


        #region Properties

        private double _distanceBetweenVerticalGrid { get; set; }
        public double DistanceBetweenVerticalGrid
        {
            get { return _distanceBetweenVerticalGrid; }
            set
            {
                if (_distanceBetweenVerticalGrid != value)
                {
                    _distanceBetweenVerticalGrid = value;
                    OnPropertyChanger();
                }
            }
        }

        private int _numberOfVerticalGrid { get; set; }
        public int NumberOfVerticalGrid
        {
            get { return _numberOfVerticalGrid; }
            set
            {
                if (_numberOfVerticalGrid != value)
                {
                    _numberOfVerticalGrid = value;
                    OnPropertyChanger();
                }
            }
        }

        private double _distanceBetweenHorizontalGrid { get; set; }
        public double DistanceBetweenHorizontalGrid
        {
            get { return _distanceBetweenHorizontalGrid; }
            set
            {
                if (_distanceBetweenHorizontalGrid != value)
                {
                    _distanceBetweenHorizontalGrid = value;
                    OnPropertyChanger();
                }
            }
        }

        private int _numberOfHorizontalGrid { get; set; }
        public int NumberOfHorizontalGrid
        {
            get { return _numberOfHorizontalGrid; }
            set
            {
                if (_numberOfHorizontalGrid != value)
                {
                    _numberOfHorizontalGrid = value;
                    OnPropertyChanger();
                }
            }
        }

        #endregion


        #region Methods

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanger([CallerMemberName] string Name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
            CreateGridsCommand.RaiseCanExecuteChanged();

        }

        public void ExcuteCmd(object paremeter)
        {

            if (_commandData == null) return;

            Document doc = _commandData.Application.ActiveUIDocument.Document;

            try
            {
                using (Transaction transaction = new Transaction(doc, "Create Grid"))
                {
                    transaction.Start();

                    double distH = DistanceBetweenHorizontalGrid / 304.8;
                    double distV = DistanceBetweenVerticalGrid / 304.8;

                    GridCreator.CreateGrids(doc, distH, distV, NumberOfHorizontalGrid, NumberOfVerticalGrid);

                    transaction.Commit();
                }

                _window?.Close();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }

        }

        public bool CanExcuteCmd(object parameter)
        {
            // allow excution only if all values are valid
            return _commandData != null &&
                               NumberOfHorizontalGrid > 0 &&
                               NumberOfVerticalGrid > 0 &&
                               DistanceBetweenHorizontalGrid > 0 &&
                               DistanceBetweenVerticalGrid > 0;
        }

        #endregion


        #region Commadns

        public MyCommand CreateGridsCommand { get; set; }
        #endregion


    }
}
