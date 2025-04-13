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
    /// Interaction logic for FoundationSelectionWindow.xaml
    /// </summary>
    public partial class FoundationSelectionWindow : Window
    {
        public string SelectedFoundationType { get; private set; }
        public string SelectedLevelName { get; private set; }

        public FoundationSelectionWindow(List<string> foundationNames, List<string> levelNames)
        {
            InitializeComponent();
            FoundationComboBox.ItemsSource = foundationNames;
            LevelComboBox.ItemsSource = levelNames;
            FoundationComboBox.SelectedIndex = 0;
            LevelComboBox.SelectedIndex = 0;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFoundationType = FoundationComboBox.SelectedItem as string;
            SelectedLevelName = LevelComboBox.SelectedItem as string;
            this.DialogResult = true;
        }
    }
}
