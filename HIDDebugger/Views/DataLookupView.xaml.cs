using System.Windows;
using HIDDebugger.ViewModels;

namespace HIDDebugger.Views
{
    public partial class DataLookupView : Window
    {
        public DataLookupView(DataLookupViewModel datacontext)
        {
            InitializeComponent();
            DataContext = datacontext;
        }
    }
}