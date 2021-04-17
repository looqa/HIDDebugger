using System;
using HIDDebugger.Base;

namespace HIDDebugger.Model
{
    [Serializable]
    public class ByteModel : PropertyChangedBase
    {
        private TimeSpan _timeCreated;

        public TimeSpan TimeCreated
        {
            get => _timeCreated;
            set => _timeCreated = value;
        }

        private byte _value;

        public byte Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(HexValue));
                OnPropertyChanged(nameof(BinValue));
            }
        }

        public string HexValue
        {
            get => Value.ToString("X");
        }

        public string BinValue
        {
            get => Convert.ToString(Value, 2);
        }
    }
}