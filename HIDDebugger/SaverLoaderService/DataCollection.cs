using System;
using System.Collections.ObjectModel;
using HIDDebugger.Model;

namespace HIDDebugger.SaverLoaderService
{
    [Serializable]
    public class DataCollection
    {
        public int VendorId { get; set; }
        public int ProductId { get; set; }
        public ObservableCollection<Report> ReportCollection;
    }
}