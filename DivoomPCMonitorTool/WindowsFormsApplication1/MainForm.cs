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

namespace DivoomPCMonitorTool
{
    public partial class MainForm : Form
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MainForm(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            InitializeComponent();
            _lcdMsg.Visible = false;
            _lcdList.Visible = false;
            _ = DivoomUpdateDeviceList();
        }
        public async Task<string> HttpPostAsync(string url, string sendData)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                using var content = new StringContent(sendData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> HttpPost2Async(string Url, string postDataStr)
        {
            using var client = _httpClientFactory.CreateClient();
            using var content = new StringContent(postDataStr, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync(Url, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        // GET方法
        public async Task<string> HttpGetAsync(string Url)
        {
            using var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(Url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private async void DivoomSendHttpInfo(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_deviceIpAddr) || _localList is null || _localList.DeviceList.Length == 0)
            {
                return;

            }

            var PostItem = new DevicePostItem { LcdId = _selectedLcdId, DispData = new string[6] };
            var PostInfo = new DevicePostList { Command = "Device/UpdatePCParaInfo", ScreenList = new[] { PostItem } };

            if (_deviceIpAddr.Length > 0)
            {
                _computer.Accept(_updateVisitor);
                for (int i = 0; i < _computer.Hardware.Count; i++)
                {
                    //查找硬件类型为CPU
                    if (_computer.Hardware[i].HardwareType == HardwareType.Cpu)
                    {
                        for (int j = 0; j < _computer.Hardware[i].Sensors.Length; j++)
                        {
                            //找到温度传感器
                            if (_computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                            {
                                _sensorValues.CpuTemp_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                _sensorValues.CpuTemp_value += "C";
                            }
                            else if (_computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                            {
                                _sensorValues.CpuUse_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                if (_sensorValues.CpuUse_value.Length > 2)
                                {
                                    _sensorValues.CpuUse_value = _sensorValues.CpuUse_value.Substring(0, 2);
                                }
                                _sensorValues.CpuUse_value += "%";
                            }
                        }
                    }
                    else if (_computer.Hardware[i].HardwareType is HardwareType.GpuNvidia or HardwareType.GpuAmd or HardwareType.GpuIntel)
                    {
                        for (int j = 0; j < _computer.Hardware[i].Sensors.Length; j++)
                        {
                            //找到温度传感器
                            if (_computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                            {
                                _sensorValues.GpuTemp_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                _sensorValues.GpuTemp_value += "C";
                            }
                            else if (_computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                            {
                                _sensorValues.GpuUse_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                if (_sensorValues.GpuUse_value.Length > 2)
                                {
                                    _sensorValues.GpuUse_value = _sensorValues.GpuUse_value.Substring(0, 2);
                                }
                                _sensorValues.GpuUse_value += "%";
                            }
                        }
                    }
                    else if (_computer.Hardware[i].HardwareType == HardwareType.Storage)
                    {
                        for (int j = 0; j < _computer.Hardware[i].Sensors.Length; j++)
                        {
                            //HDD TEMP
                            if (_computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                            {
                                _sensorValues.HardDiskUse_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                _sensorValues.HardDiskUse_value += "C";
                                break;
                            }
                        }
                    }
                    else if (_computer.Hardware[i].HardwareType == HardwareType.Memory && _computer.Hardware[i].Name.Equals("Total Memory"))
                    {
                        for (int j = 0; j < _computer.Hardware[i].Sensors.Length; j++)
                        {
                            //HDD TEMP
                            if (_computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                            {
                                _sensorValues.RamUse_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                _sensorValues.RamUse_value += "%";
                                break;
                            }
                        }
                    }
                }

                //_sensorValues.RamUse_value = ((memInfo.ullTotalPhys - memInfo.ullAvailPhys) * 100 / memInfo.ullTotalPhys).ToString().Substring(0, 2) + "%";
                PostItem.DispData[2] = _sensorValues.CpuTemp_value;
                PostItem.DispData[0] = _sensorValues.CpuUse_value;
                PostItem.DispData[3] = _sensorValues.GpuTemp_value;
                PostItem.DispData[1] = _sensorValues.GpuUse_value;
                PostItem.DispData[5] = _sensorValues.HardDiskUse_value;
                PostItem.DispData[4] = _sensorValues.RamUse_value;
                _cpuTemp.Text = "CpuTemp: " + _sensorValues.CpuTemp_value;
                _cpuUse.Text = "CpuUse: " + _sensorValues.CpuUse_value;
                _gpuTemp.Text = "GpuTemp: " + _sensorValues.GpuTemp_value;
                _gpuUse.Text = "GpuUse: " + _sensorValues.GpuUse_value;
                _hddUse.Text = "HddUse: " + _sensorValues.HardDiskUse_value;
                _RamUse.Text = "RamUse: " + _sensorValues.RamUse_value;
                string para_info = JsonConvert.SerializeObject(PostInfo);
                Console.WriteLine("request info:" + para_info);
                string response_info = await HttpPostAsync("http://" + _deviceIpAddr + ":80/post", para_info);
                Console.WriteLine("get info:" + response_info);
            }
        }
        private async Task DivoomUpdateDeviceList()
         {
             int i;
            string deviceList = await HttpGetAsync(UrlInfo);
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
                string IndependenceStr = await HttpGetAsync(url_info);
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
             string response_info = await HttpPostAsync("http://" + _deviceIpAddr + ":80/post", para_info);

         }
        private async void divoomList_SelectedIndexChanged(object sender, EventArgs e)
        {
            await DivoomSendSelectClock();
        }
        private async void LCDList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string raw_value = _lcdList.SelectedItems[0].ToString();
            _selectedLcdId = Convert.ToInt32(raw_value) - 1;

            if(_localList != null && _localList.DeviceList!=null && _localList.DeviceList.Count() > 0)
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
            foreach (IHardware subHardware in hardware.SubHardware){
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
