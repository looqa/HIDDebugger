using System;
using System.Linq;
using System.Windows;
using HIDDebugger.Base;
using HIDDebugger.Model;
using HIDDebugger.Views;

namespace HIDDebugger.ViewModels
{
    public class AddReportViewModel : PropertyChangedBase
    {
               private readonly AddReportView _view;
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

        private RelayCommand _applyButtonCommand;
        public RelayCommand ApplyButtonCommand => _applyButtonCommand ??= new RelayCommand(obj => ApplyButton());
        private RelayCommand _addByteButtonCommand;
        public RelayCommand AddByteButtonCommand => _addByteButtonCommand ??= new RelayCommand(obj => AddByte());
        private RelayCommand _removeByteButtonCommand;
        public RelayCommand RemoveByteButtonCommand => _removeByteButtonCommand ??= new RelayCommand(RemoveByte);

        public bool CanAddNewByte
        {
            get
            {
                if (int.TryParse(NewByteValue, out int check))
                {
                    if (check <= 255 && check >= 0)
                        return true;
                }
                else return false;
                return false;
            }
        }

        public bool IsCompleted => CurrentReport.Length > 0 && CurrentReport.Name != "" &&
                                   CurrentReport.BytesCollection.Count <= CurrentReport.Length;
        public bool IsApplied = false;
        public bool CanRemoveByte => CurrentSelectedItem != null;
        private ByteModel _currentSelectedItem;
        public ByteModel CurrentSelectedItem
        {
            get => _currentSelectedItem;
            set
            {
                _currentSelectedItem = value;
                OnPropertyChanged(nameof(CurrentSelectedItem));
                OnPropertyChanged(nameof(CanRemoveByte));
            }
        }
        private string _newByteValue;
        public string NewByteValue
        {
            get => _newByteValue;
            set
            {
                _newByteValue = value;
                OnPropertyChanged(nameof(NewByteValue));
                OnPropertyChanged(nameof(CanAddNewByte));
            }
        }
        public AddReportViewModel(AddReportView view)
        {
            _view = view;
            _currentReport = new Report();
        }
        public AddReportViewModel(AddReportView view, Report report)
        {
            _view = view;
            _currentReport = report;
        }

        private void ApplyButton()
        {
            IsApplied = true;
            Window.GetWindow(_view)?.Close();
        }

        private void AddByte()
        {
            var add = Convert.ToByte(NewByteValue);
            CurrentReport.BytesCollection.Add(new ByteModel { Value = add, TimeCreated = DateTime.Now.TimeOfDay });
            OnPropertyChanged(nameof(IsCompleted));
        }
        private void RemoveByte(object obj)
        {
            var byteModel = obj as ByteModel;
            CurrentReport.BytesCollection.Remove(CurrentReport.BytesCollection.Single(i => byteModel != null && i.TimeCreated == byteModel.TimeCreated));
            OnPropertyChanged(nameof(IsCompleted));
        }
    }
}