using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HIDDebugger.Model;
using Microsoft.Win32;

namespace HIDDebugger.SaverLoaderService
{
    public class DataSaverLoader
    {
        public static void Save(Device device, ObservableCollection<Report> reports)
        {
            var sfd = new SaveFileDialog {Filter = "HID Debugger file (*.hdc)|*.hdc"};
            if (sfd.ShowDialog() != true) return;
            var dc = new DataCollection
            {
                ProductId = device.ProductId, VendorId = device.VendorId, ReportCollection = reports
            };
            var bf = new BinaryFormatter();
            using (var fileStream =
                new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bf.Serialize(fileStream, dc);
            }
        }

        public static bool Load(Device device, ObservableCollection<Report> reports)
        {
            var ofd = new OpenFileDialog {Filter = "HID Debugger file (*.hdc)|*.hdc"};
            if (ofd.ShowDialog() != true) return false;
            var bf = new BinaryFormatter();
            using (var fsFileStream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var dc = (DataCollection) bf.Deserialize(fsFileStream);
                device.ProductId = dc.ProductId;
                device.VendorId = dc.VendorId;
                reports.Clear();
                foreach (var item in dc.ReportCollection)
                {
                    reports.Add(item);
                }

                return true;
            }

        }
    }
}