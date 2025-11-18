using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DivoomPcMonitor.Domain.Clients;
using DivoomPcMonitor.Domain.Contracts;
using DivoomPcMonitorTool.UI.Views;
using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DivoomPcMonitorTool.UI.ViewModels
{
    public class HardwareInfoVm
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public bool IsActive { get; set; }
    }
    public class DeviceVm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
    }
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IHttpServiceClient _httpServiceClient;

        [ObservableProperty] private ObservableCollection<DeviceVm> _devicesListBox = new();
        [ObservableProperty]
        private DeviceVm? _selectedDevice;
        public ListBox LcdList { get; set; } = new ListBox();
        public TextBox CpuUse { get; set; } = new TextBox();
        public TextBox CpuTemp { get; set; } = new TextBox();
        public TextBox GpuUse { get; set; } = new TextBox();
        public TextBox GpuTemp { get; set; } = new TextBox();
        public TextBox RamUse { get; set; } = new TextBox();
        public TextBox HddUse { get; set; } = new TextBox();
        public string[] MyItems { get; set; } = new string[] { "2", "2" , "3", "4"};
        public string LcdMessage { get; } = "LCD Displays";
        public string DeviceListMessage { get; set; } = "Divoom Devices on LAN";
        public string HardwareInfoMessage { get; } =  "Hardware Information";
        public bool ShowLcd { get; set; }
        public bool ShowDevicesList { get; set; }
        public bool ShowHardwareInfo { get; set; }

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

        public MainWindowViewModel(IHttpServiceClient httpServiceClient)
        {
            _httpServiceClient = httpServiceClient;
            ShowLcd = ShowDevicesList = ShowHardwareInfo = false;

            IsBusy = true;
            SettingsCommand = new AsyncRelayCommand(OpenSettingsAsync);
            ExitCommand = new RelayCommand(ExitApp);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);

            _updateVisitor = new UpdateVisitor();

            HardwareInfos.Add(new HardwareInfoVm { Name = "Option 1" });
            HardwareInfos.Add(new HardwareInfoVm { Name = "Option 2" });
            HardwareInfos.Add(new HardwareInfoVm { Name = "Option 3" });

            _sensorValues = new SensorValues();

            _updateVisitor = new UpdateVisitor();
            _computer = new Computer
            {
                IsBatteryEnabled = true,
                IsControllerEnabled = true,
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsNetworkEnabled = false,
                IsPsuEnabled = true,
                IsStorageEnabled = true
            };
            _computer.Open();

            _ = DivoomUpdateDeviceList();

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
                await DivoomUpdateDeviceList();

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

        private async Task DivoomUpdateDeviceList()
        {
            int i;
            string deviceList = await _httpServiceClient.GetAsync(UrlInfo);
            // Console.WriteLine(device_list);
            _localList = JsonSerializer.Deserialize<DivoomDeviceList>(deviceList);
            DevicesListBox.Clear();
            for (i = 0; _localList.DeviceList != null && i < _localList.DeviceList.Length; i++)
            {
                DevicesListBox.Add(new DeviceVm
                {
                    Name = _localList.DeviceList[i].DeviceName,
                    Ip = _localList.DeviceList[i].DevicePrivateIP,
                    Id = _localList.DeviceList[i].DeviceId.ToString()
                });
            }

        }

        private async Task DivoomSendSelectClock(DeviceVm? value)
        {
            _deviceIpAddr = value.Ip;
            Console.WriteLine("selece items:" + _deviceIpAddr);

            if (_localList.DeviceList.FirstOrDefault(x=>x.DeviceId.ToString( )== value.Id)?.Hardware == 400)
            {
                //get the Independence index of timegate 
                string url_info = "http://app.divoom-gz.com/Channel/Get5LcdInfoV2?DeviceType=LCD&DeviceId=" + value.Id;
                string IndependenceStr = await _httpServiceClient.GetAsync(url_info);
                if (IndependenceStr != null && IndependenceStr.Length > 0)
                {
                    TimeGateIndependenceInfo IndependenceInfo = JsonSerializer.Deserialize<TimeGateIndependenceInfo>(IndependenceStr);

                    _lcdIndependence = IndependenceInfo.LcdIndependence;

                }
                ShowLcd = true;

            }
            else
            {
                ShowLcd = false;
            }

            var PostInfo = new DeviceSelectClockInfo { LcdIndependence = _lcdIndependence, Command = "Channel/SetClockSelectId", LcdIndex = LcdList.SelectedIndex, ClockId = 625 };
            string para_info = JsonSerializer.Serialize(PostInfo);
            Console.WriteLine("request info:" + para_info);
            string response_info = await _httpServiceClient.PostJsonAsync("http://" + _deviceIpAddr + ":80/post", para_info);

        }
        partial void OnSelectedDeviceChanged(DeviceVm? value)
        {
            // This is called every time selection changes
            if (value is null) return;

            // Do whatever you want here
            // e.g. update details, start polling, etc.
            _= DivoomSendSelectClock(value);
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
