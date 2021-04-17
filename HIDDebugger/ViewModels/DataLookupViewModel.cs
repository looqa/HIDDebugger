using HIDDebugger.Base;
using HIDDebugger.Model;

namespace HIDDebugger.ViewModels
{
    public class DataLookupViewModel : PropertyChangedBase
    {
        private Report _currentReport;
        public Report CurrentReport
        {
            get => _currentReport;
            set
            {
                _currentReport = value;
                OnPropertyChanged(nameof(CurrentReport));
            }
        }
        public DataLookupViewModel(Report currentReport)
        {
            CurrentReport = currentReport;
        }
    }
}