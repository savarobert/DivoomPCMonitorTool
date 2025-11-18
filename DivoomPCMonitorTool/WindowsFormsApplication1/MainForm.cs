using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DivoomPcMonitor.Domain.Contracts;
using LibreHardwareMonitor.Hardware;
using Newtonsoft.Json;
using DivoomPcMonitor.Infrastructure;
using System.Net.Http;
using System.Threading.Tasks;
using DivoomPcMonitor.Domain.Clients;

namespace DivoomPCMonitorTool
{
    public partial class MainForm : Form
    {
        private readonly IHttpServiceClient _httpServiceClient;

        public MainForm(IHttpServiceClient httpServiceClient)
        {
            _httpServiceClient = httpServiceClient;
            InitializeComponent();
            _lcdMsg.Visible = false;
            _lcdList.Visible = false;
            _ = DivoomUpdateDeviceList();
        }

        private async void DivoomSendHttpInfo(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_deviceIpAddr) || _localList is null || _localList.DeviceList.Length == 0)
            {
                return;

            }

            var postItem = new DevicePostItem { LcdId = _selectedLcdId, DispData = new string[6] };
            var postInfo = new DevicePostList { Command = "Device/UpdatePCParaInfo", ScreenList = new[] { postItem } };

            if (_deviceIpAddr.Length <= 0) return;
            _computer.Accept(_updateVisitor);
            foreach (var hardware in _computer.Hardware)
            {
                switch (hardware.HardwareType)
                {
                    //查找硬件类型为CPU
                    case HardwareType.Cpu:
                    {
                        foreach (var t in hardware.Sensors)
                        {
                            switch (t.SensorType)
                            {
                                //找到温度传感器
                                case SensorType.Temperature:
                                    _sensorValues.CpuTempValue = t.Value.ToString();
                                    _sensorValues.CpuTempValue += "C";
                                    break;
                                case SensorType.Load:
                                {
                                    _sensorValues.CpuUseValue = t.Value.ToString();
                                    if (_sensorValues.CpuUseValue.Length > 2)
                                    {
                                        _sensorValues.CpuUseValue = _sensorValues.CpuUseValue.Substring(0, 2);
                                    }
                                    _sensorValues.CpuUseValue += "%";
                                    break;
                                }
                                case SensorType.Voltage:
                                case SensorType.Current:
                                case SensorType.Power:
                                case SensorType.Clock:
                                case SensorType.Frequency:
                                case SensorType.Fan:
                                case SensorType.Flow:
                                case SensorType.Control:
                                case SensorType.Level:
                                case SensorType.Factor:
                                case SensorType.Data:
                                case SensorType.SmallData:
                                case SensorType.Throughput:
                                case SensorType.TimeSpan:
                                case SensorType.Timing:
                                case SensorType.Energy:
                                case SensorType.Noise:
                                case SensorType.Conductivity:
                                case SensorType.Humidity:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        break;
                    }
                    case HardwareType.GpuNvidia or HardwareType.GpuAmd or HardwareType.GpuIntel:
                        {
                            foreach (var t in hardware.Sensors)
                            {
                                switch (t.SensorType)
                                {
                                    //找到温度传感器
                                    case SensorType.Temperature:
                                        _sensorValues.GpuTempValue = t.Value.ToString();
                                        _sensorValues.GpuTempValue += "C";
                                        break;
                                    case SensorType.Load:
                                        {
                                            _sensorValues.GpuUseValue = t.Value.ToString();
                                            if (_sensorValues.GpuUseValue.Length > 2)
                                            {
                                                _sensorValues.GpuUseValue = _sensorValues.GpuUseValue.Substring(0, 2);
                                            }
                                            _sensorValues.GpuUseValue += "%";
                                            break;
                                        }
                                    case SensorType.Voltage:
                                    case SensorType.Current:
                                    case SensorType.Power:
                                    case SensorType.Clock:
                                    case SensorType.Frequency:
                                    case SensorType.Fan:
                                    case SensorType.Flow:
                                    case SensorType.Control:
                                    case SensorType.Level:
                                    case SensorType.Factor:
                                    case SensorType.Data:
                                    case SensorType.SmallData:
                                    case SensorType.Throughput:
                                    case SensorType.TimeSpan:
                                    case SensorType.Timing:
                                    case SensorType.Energy:
                                    case SensorType.Noise:
                                    case SensorType.Conductivity:
                                    case SensorType.Humidity:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }

                            break;
                        }
                    case HardwareType.Storage:
                        {
                            foreach (var t in hardware.Sensors)
                            {
                                switch (t.SensorType)
                                {
                                    //HDD TEMP
                                    case SensorType.Temperature:
                                        _sensorValues.HardDiskUseValue = t.Value.ToString();
                                        _sensorValues.HardDiskUseValue += "C";
                                        break;
                                    case SensorType.Voltage:
                                    case SensorType.Current:
                                    case SensorType.Power:
                                    case SensorType.Clock:
                                    case SensorType.Load:
                                    case SensorType.Frequency:
                                    case SensorType.Fan:
                                    case SensorType.Flow:
                                    case SensorType.Control:
                                    case SensorType.Level:
                                    case SensorType.Factor:
                                    case SensorType.Data:
                                    case SensorType.SmallData:
                                    case SensorType.Throughput:
                                    case SensorType.TimeSpan:
                                    case SensorType.Timing:
                                    case SensorType.Energy:
                                    case SensorType.Noise:
                                    case SensorType.Conductivity:
                                    case SensorType.Humidity:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }

                            break;
                        }
                    case HardwareType.Memory when hardware.Name.Equals("Total Memory"):
                        {
                            foreach (var t in hardware.Sensors)
                            {
                                switch (t.SensorType)
                                {
                                    //MEMORY USAGE
                                    case SensorType.Load:
                                        _sensorValues.RamUseValue = t.Value.ToString()[..4];
                                        _sensorValues.RamUseValue += "%";
                                        break;
                                }
                            }

                            break;
                        }
                }
            }

            postItem.DispData[2] = _sensorValues.CpuTempValue;
            postItem.DispData[0] = _sensorValues.CpuUseValue;
            postItem.DispData[3] = _sensorValues.GpuTempValue;
            postItem.DispData[1] = _sensorValues.GpuUseValue;
            postItem.DispData[5] = _sensorValues.HardDiskUseValue;
            postItem.DispData[4] = _sensorValues.RamUseValue;
            _cpuTemp.Text = "CpuTemp: " + _sensorValues.CpuTempValue;
            _cpuUse.Text = "CpuUse: " + _sensorValues.CpuUseValue;
            _gpuTemp.Text = "GpuTemp: " + _sensorValues.GpuTempValue;
            _gpuUse.Text = "GpuUse: " + _sensorValues.GpuUseValue;
            _hddUse.Text = "HddUse: " + _sensorValues.HardDiskUseValue;
            _RamUse.Text = "RamUse: " + _sensorValues.RamUseValue;
            string para_info = JsonConvert.SerializeObject(postInfo);
            Console.WriteLine("request info:" + para_info);
            string response_info = await _httpServiceClient.PostJsonAsync("http://" + _deviceIpAddr + ":80/post", para_info);
            Console.WriteLine("get info:" + response_info);
        }
        private async Task DivoomUpdateDeviceList()
        {
            int i;
            string deviceList = await _httpServiceClient.GetAsync(UrlInfo);
            // Console.WriteLine(device_list);
            _localList = JsonConvert.DeserializeObject<DivoomDeviceList>(deviceList);
            _devicesListBox.Items.Clear();
            for (i = 0; _localList.DeviceList != null && i < _localList.DeviceList.Length; i++)
            {
                _devicesListBox.Items.Add(_localList.DeviceList[i].DeviceName);
            }

        }
        private async void refreshList_Click(object sender, EventArgs e)
        {
            await DivoomUpdateDeviceList();
        }

        private async Task DivoomSendSelectClock()
        {
            _deviceIpAddr = _localList.DeviceList[_devicesListBox.SelectedIndex].DevicePrivateIP;
            Console.WriteLine("selece items:" + _deviceIpAddr);

            if (_localList.DeviceList[_devicesListBox.SelectedIndex].Hardware == 400)
            {
                //get the Independence index of timegate 
                string url_info = "http://app.divoom-gz.com/Channel/Get5LcdInfoV2?DeviceType=LCD&DeviceId=" + _localList.DeviceList[_devicesListBox.SelectedIndex].DeviceId;
                string IndependenceStr = await _httpServiceClient.GetAsync(url_info);
                if (IndependenceStr != null && IndependenceStr.Length > 0)
                {
                    TimeGateIndependenceInfo IndependenceInfo = JsonConvert.DeserializeObject<TimeGateIndependenceInfo>(IndependenceStr);

                    _lcdIndependence = IndependenceInfo.LcdIndependence;

                }
                _lcdMsg.Visible = true;
                _lcdList.Visible = true;

            }
            else
            {
                _lcdMsg.Visible = false;
                _lcdList.Visible = false;
            }

            var PostInfo = new DeviceSelectClockInfo { LcdIndependence = _lcdIndependence, Command = "Channel/SetClockSelectId", LcdIndex = _lcdList.SelectedIndex, ClockId = 625 };
            string para_info = JsonConvert.SerializeObject(PostInfo);
            Console.WriteLine("request info:" + para_info);
            string response_info = await _httpServiceClient.PostJsonAsync("http://" + _deviceIpAddr + ":80/post", para_info);

        }
        private async void divoomList_SelectedIndexChanged(object sender, EventArgs e)
        {
            await DivoomSendSelectClock();
        }
        private async void LCDList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string raw_value = _lcdList.SelectedItems[0].ToString();
            _selectedLcdId = Convert.ToInt32(raw_value) - 1;

            if (_localList != null && _localList.DeviceList != null && _localList.DeviceList.Count() > 0)
            {
                if (_devicesListBox.SelectedIndex > 0 && _devicesListBox.SelectedIndex < _localList.DeviceList.Count())
                {
                    await DivoomSendSelectClock();
                }

            }


        }
        public void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("exit");
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
