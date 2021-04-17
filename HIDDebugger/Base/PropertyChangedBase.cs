using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HIDDebugger.Base
{
    [Serializable]
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        [field: NonSerialized] 
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") 
        {
            if (PropertyChanged != null) 
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}