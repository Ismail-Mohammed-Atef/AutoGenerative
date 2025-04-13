using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAPI_Project1.App.Command;
using RevitAPI_Project1.App.Services;
using RvtFloorType = RevitAPI_Project1.Model.DataContext.RvtFloorType;
using RvtLevel = RevitAPI_Project1.Model.DataContext.RvtLevel;
namespace RevitAPI_Project1.ViewModel
{
    public class CreateFloorVM : INotifyPropertyChanged
    {
        #region Constructor
        private readonly ExternalCommandData _commandData;
        private readonly Window window;

        public CreateFloorVM(ExternalCommandData commanddata = null,
                            List<RvtLevel> levels = null,
                            List<RvtFloorType> floorTypes = null,
                            Window _window = null)
        {
            _commandData = commanddata;
            window = _window;

            // Initialize collections
            Levels = levels ?? new List<RvtLevel>();
            FloorTypes = floorTypes ?? new List<RvtFloorType>();

            // Set default selections if available
            if (Levels.Count > 0)
                SelectedLevel = Levels.First();

            if (FloorTypes.Count > 0)
                SelectedFloorType = FloorTypes.First();

            CreateFloorCommand = new MyCommand(ExcuteCmd, CanExcuteCmd);
        }
        #endregion



        #region Properties
        public List<RvtFloorType> FloorTypes { get; set; }

        public List<RvtLevel> Levels { get; set; }


        private RvtLevel _selectedLevel { get; set; }

        public RvtLevel SelectedLevel
        {
            get { return _selectedLevel; }
            set
            {
                if (_selectedLevel != value)
                {
                    _selectedLevel = value;
                    OnPropertyChanged();
                }
            }
        }

        private RvtFloorType _selectedFloorType { get; set; }
        public RvtFloorType SelectedFloorType
        {
            get { return _selectedFloorType; }
            set
            {
                if (_selectedFloorType != value)
                {
                    _selectedFloorType = value;
                    OnPropertyChanged();
                }
            }
        } 
        #endregion



        #region Methods

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string Name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public void ExcuteCmd(object parameter)
        {
            if (_commandData == null) return;

            Document doc = _commandData.Application.ActiveUIDocument.Document;

            try
            {
                using (Transaction transaction = new Transaction(doc, "Create Grid"))
                {
                    transaction.Start();

                    FloorCreator.CreateFloor(doc, SelectedFloorType, SelectedLevel);


                    transaction.Commit();
                }

                window?.Close();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
        }

        public bool CanExcuteCmd(object parameter)
        {
            return true;
        }

        #endregion



        #region Commands
        public MyCommand CreateFloorCommand { get; set; } 
        #endregion


    }
}
