using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HIDDebugger.Base;
using HidLibrary;

namespace HIDDebugger.Model
{
    public class Device : PropertyChangedBase
    {
                public delegate void DeviceEventsHandler();
        public event DeviceEventsHandler OnDisconnect;

        private HidDevice dev;

        public CancellationTokenSource CancelReceiveReports;
        CancellationToken _cancelReceiveReportsToken;

        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }
        private int _vendorId;
        public int VendorId
        {
            get => _vendorId;
            set
            {
                _vendorId = value;
                OnPropertyChanged(nameof(VendorId));
            }
        }
        private int _productId;
        public int ProductId
        {
            get => _productId;
            set
            {
                _productId = value;
                OnPropertyChanged(nameof(ProductId));
            }
        }


        public bool Connect()
        {
            HidDevice device = HidDevices.Enumerate(VendorId, ProductId).FirstOrDefault();
            if (device == null)
            {
                return false;
            }
            else
            {
                dev = device;
                dev.MonitorDeviceEvents = true;
                dev.Removed += OnDeviceRemoved;
                IsConnected = true;
                return true;
            }
        }
        public Task<bool> Disconnect()
        {
            if (dev != null)
            {
                dev.Dispose();
                IsConnected = false;
            }
            return Task.FromResult<bool>(true);
        }

        private void OnDeviceRemoved()
        {
            OnDisconnect.Invoke();
        }
        public async Task<bool> SendReport(Report rep, ObservableCollection<Report> sentCollection)
        {
            var data = new byte[rep.Length+1];
            data[0] = rep.ReportId;
            
            for (var i = 0; i < rep.BytesCollection.Count(); i++)
            {
                data[i+1] = rep.BytesCollection[i].Value;
            }
            var isSent = await dev.WriteReportAsync(new HidReport(rep.Length+1, new HidDeviceData(data, HidDeviceData.ReadStatus.Success)));
            Report sentReport = new Report() { ActionTime = DateTime.Now.TimeOfDay, BytesCollection = rep.BytesCollection, Length = rep.Length, Name = rep.Name, ReportId = rep.ReportId, Success = isSent };
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                sentCollection.Add(sentReport);
            });
            return isSent;
        }

        public void ReadReports(ObservableCollection<Report> collection)
        {
            CancelReceiveReports = new CancellationTokenSource();
            _cancelReceiveReportsToken = CancelReceiveReports.Token;
            while (!_cancelReceiveReportsToken.IsCancellationRequested)
            {
                HidReport hr = dev.ReadReport();
                ObservableCollection<ByteModel> bytes = new ObservableCollection<ByteModel>();
                for (var i = 0; i < hr.Data.Length; i++)
                {
                    bytes.Add(new ByteModel() { Value = hr.Data[i] });
                }
                Report rep = new Report() { ActionTime = DateTime.Now.TimeOfDay, BytesCollection = bytes, ReportId = hr.ReportId, Length = hr.Data.Length };
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    collection.Add(rep);
                });
            }
        }
    }
}