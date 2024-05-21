using Autodesk.Revit.UI;
using ScannerParameters.ViewModels;
using System.Windows;

namespace ScannerParameters
{
    /// <summary>
    /// Interaction logic for ScannerParameterForm.xaml
    /// </summary>
    public partial class ScannerParameterForm : Window
    {
        private readonly UIApplication _uiApp;
        public ScannerParameterForm(UIApplication _uiApplication)
        {
            InitializeComponent();
            _uiApp = _uiApplication;
            DataContext = new ScannerParametersViewModel(_uiApp);
        }
    }
}
