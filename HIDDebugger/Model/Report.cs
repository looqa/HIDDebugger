using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using HIDDebugger.Base;

namespace HIDDebugger.Model
{
    [Serializable]
    public class Report : PropertyChangedBase
    {
        private int _length;

        public int Length
        {
            get => _length;
            set
            {
                _length = value;
                OnPropertyChanged(nameof(Length));
            }
        }

        private TimeSpan _timeCreated;

        public TimeSpan TimeCreated
        {
            get => _timeCreated;
            set => _timeCreated = value;
        }

        private byte _reportId;

        public byte ReportId
        {
            get => _reportId;
            set
            {
                _reportId = value;
                OnPropertyChanged(nameof(ReportId));
            }
        }

        private string _name = "";

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [field: NonSerialized] 
        private int _pending = 0;
        
        public int Pending
        {
            get => _pending;
            set
            {
                _pending = value;
                OnPropertyChanged(nameof(Pending));
            }
        }

        private ObservableCollection<ByteModel> _bytesCollection = new ObservableCollection<ByteModel>();

        public ObservableCollection<ByteModel> BytesCollection
        {
            get => _bytesCollection;
            set
            {
                _bytesCollection = value;
                OnPropertyChanged(nameof(BytesCollection));
            }
        }

        private TimeSpan _actionTime;

        public TimeSpan ActionTime
        {
            get => _actionTime;
            set
            {
                _actionTime = value;
                OnPropertyChanged(nameof(ActionTime));
            }
        }

        private bool _success;

        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                OnPropertyChanged(nameof(Success));
            }
        }
    }
}