using System.Windows;
using RevitAPI_Project1.ViewModel;

namespace RevitAPI_Project1.View
{
    /// <summary>
    /// Interaction logic for CreateGridsView.xaml
    /// </summary>
    public partial class CreateGridsView : Window
    {
        public CreateGridsView()
        {
            InitializeComponent();
            DataContext = new CreateGridsVM();

        }
    }
}
