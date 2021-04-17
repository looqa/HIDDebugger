using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HIDDebugger.Base;
using HIDDebugger.SaverLoaderService;
using HIDDebugger.Model;
using HIDDebugger.SaverLoaderService;
using HIDDebugger.Views;
using HidLibrary;

namespace HIDDebugger.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private MainView _view;

        public Device Device { get; } = new Device();

        public int Vid
        {
            get => Device.VendorId;
            set
            {
                Device.VendorId = value;
                OnPropertyChanged((nameof(Vid)));
                OnPropertyChanged((nameof(CanConnect)));
            }
        }

        public int Pid
        {
            get => Device.ProductId;
            set
            {
                Device.ProductId = value;
                OnPropertyChanged(nameof(Pid));
                OnPropertyChanged(nameof(CanConnect));
            }
        }

        private bool _isReceivingReports = false;

        public bool IsReceivingReports
        {
            get => _isReceivingReports;
            set
            {
                _isReceivingReports = value;
                OnPropertyChanged(nameof(IsReceivingReports));
                OnPropertyChanged(nameof(CanStartReceiveReports));
                OnPropertyChanged(nameof(CanStopReceiveReports));
            }
        }


        private RelayCommand _connectCommand;
        
        public RelayCommand ConnectCommand => _connectCommand ??= new RelayCommand(_ => Connect());
        
        public bool CanConnect => !Device.IsConnected && Vid != 0 && Pid != 0;

        private RelayCommand _disconnectCommand;

        public RelayCommand DisconnectCommand =>
            _disconnectCommand ??= new RelayCommand(_ => Disconnect());
        
        public bool CanDisconnect => !CanConnect && Device.IsConnected;

        private RelayCommand _addReportCommand;

        public RelayCommand AddReportCommand =>
            _addReportCommand ??= new RelayCommand(_ => AddReport());

        private RelayCommand _sendOneReportCommand;

        public RelayCommand SendOneReportCommand =>
            _sendOneReportCommand ??= new RelayCommand(SendReport);

        private RelayCommand _startReceivingReportsCommand;

        public RelayCommand StartReceivingReportsCommand =>
            _startReceivingReportsCommand ??= new RelayCommand(_ => StartReceivingReports());

        public bool CanStartReceiveReports => Device.IsConnected && !IsReceivingReports;

        private RelayCommand _stopReceivingReportsCommand;

        public RelayCommand StopReceivingReportsCommand =>
            _stopReceivingReportsCommand ??= new RelayCommand(_ => StopReceivingReports());

        public bool CanStopReceiveReports => Device.IsConnected && IsReceivingReports;

        private RelayCommand _dataLookupCommand;

        public RelayCommand DataLookupCommand =>
            _dataLookupCommand ??= new RelayCommand(DataLookup);

        public bool CanLookupData => DataLookupSelectedIndex >= 0;

        private RelayCommand _editReportCommand;

        public RelayCommand EditReportCommand =>
            _editReportCommand ??= new RelayCommand(EditReport);

        private RelayCommand _deleteReportCommand;

        public RelayCommand DeleteReportCommand =>
            _deleteReportCommand ??= new RelayCommand(DeleteReport);

        private RelayCommand _loadConfigCommand;

        public RelayCommand LoadConfigCommand =>
            _loadConfigCommand ??= new RelayCommand(execute: _ => LoadConfig());

        private RelayCommand _saveConfigCommand;

        public RelayCommand SaveConfigCommand =>
            _saveConfigCommand ??= new RelayCommand(_ => SaveConfig());

        public bool SendGroupButtonActive => Device.IsConnected && IsReportToSendSelected;

        private ObservableCollection<Report> _reportsCollection = new ObservableCollection<Report>();

        public ObservableCollection<Report> ReportsCollection
        {
            get => _reportsCollection;
            set
            {
                _reportsCollection = value;
                OnPropertyChanged(nameof(ReportsCollection));
            }
        }

        private int _sendReportSelectedIndex = -1;

        public int SendReportSelectedIndex
        {
            get => _sendReportSelectedIndex;
            set
            {
                _sendReportSelectedIndex = value;
                OnPropertyChanged(nameof(SendReportSelectedIndex));
                OnPropertyChanged(nameof(SendGroupButtonActive));
            }
        }

        public bool IsReportToSendSelected => SendReportSelectedIndex >= 0;

        private ObservableCollection<Report> _receivedReportsCollection = new ObservableCollection<Report>();

        public ObservableCollection<Report> ReceivedReportsCollection
        {
            get => _receivedReportsCollection;
            set
            {
                _receivedReportsCollection = value;
                OnPropertyChanged(nameof(ReceivedReportsCollection));
            }
        }

        private int _dataLookupSelectedIndex = -1;

        public int DataLookupSelectedIndex
        {
            get => _dataLookupSelectedIndex;
            set
            {
                _dataLookupSelectedIndex = value;
                OnPropertyChanged((nameof(DataLookupSelectedIndex)));
                OnPropertyChanged((nameof(CanLookupData)));
            }
        }

        private ObservableCollection<Report> _sentReportsCollection = new ObservableCollection<Report>();

        public ObservableCollection<Report> SentReportsCollection
        {
            get => _sentReportsCollection;
            set
            {
                _sentReportsCollection = value;
                OnPropertyChanged(nameof(SentReportsCollection));
            }
        }

        public MainViewModel(MainView view)
        {
            _view = view;
            Device.OnDisconnect += Disconnect;
        }


        private void Connect()
        {
            Task.Run(() =>
            {
                Device.Connect();
                OnPropertyChanged(nameof(CanConnect));
                OnPropertyChanged(nameof(CanDisconnect));
                OnPropertyChanged(nameof(CanStartReceiveReports));
                OnPropertyChanged(nameof(CanStopReceiveReports));
            });
        }


        private void Disconnect()
        {
            Device.Disconnect();
            StopReceivingReports();
            OnPropertyChanged(nameof(CanConnect));
            OnPropertyChanged(nameof(CanDisconnect));
            OnPropertyChanged(nameof(CanStartReceiveReports));
        }

        private void AddReport()
        {
            var addReportView = new AddReportView();
            var vm = new AddReportViewModel(addReportView);
            addReportView.DataContext = vm;
            var res = addReportView.ShowDialog();
            if (!res.HasValue) return;
            if (!vm.IsApplied) return;
            var rep = vm.CurrentReport;
            rep.TimeCreated = DateTime.Now.TimeOfDay;
            ReportsCollection.Add(rep);
        }

        private void SendReport(object obj)
        {
            if (obj == null) return;
            var rep = obj as Report;
            rep.Pending += 1;
            Task.Run(async () =>
            {
                var result = await Device.SendReport(rep, SentReportsCollection);
                rep.Pending -= 1;
            });
        }

        private void StartReceivingReports()
        {
            IsReceivingReports = true;
            Task.Run(() => Device.ReadReports(ReceivedReportsCollection));
        }

        private static void DataLookup(object obj)
        {
            var rep = obj as Report;
            var vm = new DataLookupViewModel(rep);
            var dlw = new DataLookupView(vm);
            dlw.Show();
        }

        private void StopReceivingReports()
        {
            if (IsReceivingReports)
                Device.CancelReceiveReports.Cancel();
            IsReceivingReports = false;
        }

        private static void EditReport(object obj)
        {
            var rep = obj as Report;
            var arw = new AddReportView();
            var vm = new AddReportViewModel(arw, rep);
            arw.DataContext = vm;
            var result = arw.ShowDialog();
        }

        private void LoadConfig()
        {
            var loadService = new DataSaverLoader();
            if (DataSaverLoader.Load(Device, ReportsCollection))
            {
                Disconnect();
                OnPropertyChanged(nameof(Vid));
                OnPropertyChanged(nameof(Pid));
            }
        }

        private void SaveConfig()
        {
            var saveService = new DataSaverLoader();
            DataSaverLoader.Save(Device, ReportsCollection);
        }

        private void DeleteReport(object obj)
        {
            var rep = obj as Report;
            ReportsCollection.Remove(ReportsCollection.Single(i => rep != null && i.TimeCreated == rep.TimeCreated));
        }
    }
}