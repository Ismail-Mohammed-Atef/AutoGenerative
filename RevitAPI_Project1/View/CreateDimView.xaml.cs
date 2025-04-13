using RevitAPI_Project1.ViewModel;
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

namespace RevitAPI_Project1.View
{
    /// <summary>
    /// Interaction logic for CreateDimView.xaml
    /// </summary>
    public partial class CreateDimView : Window
    {
        public CreateDimView()
        {
            DataContext = new CreateDimVM();
            InitializeComponent();
        }
    }
}
