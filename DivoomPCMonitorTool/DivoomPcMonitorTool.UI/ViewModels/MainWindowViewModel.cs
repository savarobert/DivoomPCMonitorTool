using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using DivoomPcMonitor.Domain.Contracts;
using DivoomPcMonitorTool.UI.Views;
using LibreHardwareMonitor.Hardware;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DivoomPcMonitorTool.UI.ViewModels
{
    public class HardwareInfoVm
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public bool IsActive { get; set; }
    }
    public partial class MainWindowViewModel : ViewModelBase
    {
        ListBox _devicesListBox;
        ListBox _lcdList;
        Button _refreshList;
        TextBox _cpuUse;
        TextBox _cpuTemp;
        TextBox _gpuUse;
        TextBox _gpuTemp;
        TextBox _RamUse;
        TextBox _hddUse;
        public string[] MyItems { get; set; } = new string[] { "2", "2" , "3", "4"};
        public string _lcdMsg { get; } = "LCD DIsplays";
        public string _deviceListMsg { get; } = "Divoom Devices on LAN";
        public string _hardwareInfo { get; } =  "Hardware Information";

        public ObservableCollection<HardwareInfoVm> HardwareInfos { get; } = new();

        private DivoomDeviceList _localList;
        private int _selectedLcdId;
        private string _deviceIpAddr;
        private int _lcdIndependence;

        private UpdateVisitor _updateVisitor;
        private Computer _computer;
        private SensorValues _sensorValues;

        private const string UrlInfo = "http://app.divoom-gz.com/Device/ReturnSameLANDevice";
        public string Greeting { get; } = "Welcome to Avalonia!";

        public IAsyncRelayCommand SettingsCommand { get; }
        public IRelayCommand ExitCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }

        private double _progress;
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _statusText = string.Empty;
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public MainWindowViewModel()
        {
            IsBusy = true;
            SettingsCommand = new AsyncRelayCommand(OpenSettingsAsync);
            ExitCommand = new RelayCommand(ExitApp);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);

            _updateVisitor = new UpdateVisitor();

            HardwareInfos.Add(new HardwareInfoVm { Name = "Option 1" });
            HardwareInfos.Add(new HardwareInfoVm { Name = "Option 2" });
            HardwareInfos.Add(new HardwareInfoVm { Name = "Option 3" });

            IsBusy = false;
        }

        private async Task RefreshAsync()
        {
            // Simple refresh stub: update status and toggle busy state.
            try
            {
                IsBusy = true;
                StatusText = "Refreshing...";

                // TODO: place actual refresh logic here (e.g., re-scan devices, read sensors)
                await Task.Delay(500);

                StatusText = "Refresh completed";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenSettingsAsync()
        {
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var settings = new SettingsWindow();

            if (lifetime?.MainWindow is Window owner)
            {
                // Show as modal dialog when running as classic desktop app
                await settings.ShowDialog<object>(owner);
            }
            else
            {
                // Fallback: just show non-modal
                settings.Show();
            }
        }

        private static void ExitApp()
        {
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            lifetime?.Shutdown();
        }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware)
            {
                subHardware.Accept(this);
                foreach (ISensor sensor in subHardware.Sensors)
                {
                    sensor.Traverse(this);
                }

            }
        }

        public void VisitSensor(ISensor sensor) { }

        public void VisitParameter(IParameter parameter) { }
    }
}
