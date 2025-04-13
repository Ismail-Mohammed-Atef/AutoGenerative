
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace RevitAPI_Project1
{
    /// <summary>
    /// Interaction logic for ColumnSelectionWindow.xaml
    /// </summary>
    public partial class ColumnSelectionWindow : Window
    {
        public string SelectedColumnType { get; private set; }
        public string SelectedLevelName { get; private set; }
        public double OffsetValue { get; private set; }
        public double RotationValue { get; private set; }

        public ColumnSelectionWindow(List<string> columnNames, List<string> levelNames)
        {
            InitializeComponent();
            ColumnComboBox.ItemsSource = columnNames;
            LevelComboBox.ItemsSource = levelNames;
            ColumnComboBox.SelectedIndex = 0;
            LevelComboBox.SelectedIndex = 0;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColumnType = ColumnComboBox.SelectedItem as string;
            SelectedLevelName = LevelComboBox.SelectedItem as string;

            double offset, rotation;

            if (!double.TryParse(OffsetBox.Text, out offset))

                offset = 0.0;
            if (!double.TryParse(RotationBox.Text, out rotation))
                rotation = 0.0;

            OffsetValue = offset;
            RotationValue = rotation;

            this.DialogResult = true;
        }
    }
}