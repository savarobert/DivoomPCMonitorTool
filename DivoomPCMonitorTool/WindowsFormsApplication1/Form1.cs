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

namespace DivoomPCMonitorTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _lcdMsg.Visible = false;
            _lcdList.Visible = false;
            DivoomUpdateDeviceList();
        }
        public static int HttpPost(string url, string sendData, out string reslut)
        {
            reslut = "";
            try
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(sendData);
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);  // 制备web请求
                wbRequest.Proxy = null;     //现场测试注释掉也可以上传
                wbRequest.Method = "POST";
                wbRequest.ContentType = "application/json";
                wbRequest.ContentLength = data.Length;
                wbRequest.Timeout = 1000;

                #region //
                using (Stream wStream = wbRequest.GetRequestStream())         //using(){}作为语句，用于定义一个范围，在此范围的末尾将释放对象。
                {
                    wStream.Write(data, 0, data.Length);
                }
                #endregion

                //获取响应
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream, Encoding.UTF8))      //using(){}作为语句，用于定义一个范围，在此范围的末尾将释放对象。
                    {
                        reslut = sReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                reslut = e.Message;     //输出捕获到的异常，用OUT关键字输出
                return -1;              //出现异常，函数的返回值为-1
            }
            return 0;
        }

        public static string HttpPost2(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            Encoding encoding = Encoding.UTF8;
            byte[] postData = encoding.GetBytes(postDataStr);
            request.ContentLength = postData.Length;
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(postData, 0, postData.Length);
            myRequestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        //GET方法
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        private void DivoomSendHttpInfo(object sender, EventArgs e)
        {
            if (_deviceIpAddr == null || _localList == null || _localList.DeviceList == null || _localList.DeviceList.Length == 0)
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
                    else if (_computer.Hardware[i].HardwareType == HardwareType.GpuNvidia ||
                        _computer.Hardware[i].HardwareType == HardwareType.GpuAmd || _computer.Hardware[i].HardwareType == HardwareType.GpuIntel)
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
                }

                MEMORYSTATUSEX memInfo = new MEMORYSTATUSEX();
                memInfo.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));

                GlobalMemoryStatusEx(ref memInfo);

                _sensorValues.RamUse_value = ((memInfo.ullTotalPhys - memInfo.ullAvailPhys) * 100 / memInfo.ullTotalPhys).ToString().Substring(0, 2) + "%";
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
                string response_info;
                HttpPost("http://" + _deviceIpAddr + ":80/post", para_info, out response_info);
                Console.WriteLine("get info:" + response_info);
            }
        }
        private void DivoomUpdateDeviceList()
        {
            int i;
            string url_info = "http://app.divoom-gz.com/Device/ReturnSameLANDevice";
            string device_list = HttpGet(url_info, "");
            // Console.WriteLine(device_list);
            _localList = JsonConvert.DeserializeObject<DivoomDeviceList>(device_list);
            _devicesListBox.Items.Clear();
            for (i = 0; _localList.DeviceList != null && i < _localList.DeviceList.Length; i++)
            {
                _devicesListBox.Items.Add(_localList.DeviceList[i].DeviceName);
            }

        }
        private void refreshList_Click(object sender, EventArgs e)
        {
            DivoomUpdateDeviceList();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;      //可用物理内存
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll")]
        public static extern void GlobalMemoryStatusEx(ref MEMORYSTATUSEX stat);
        private void DivoomSendSelectClock()
        {
            _deviceIpAddr = _localList.DeviceList[_devicesListBox.SelectedIndex].DevicePrivateIP;
            Console.WriteLine("selece items:" + _deviceIpAddr);

            if (_localList.DeviceList[_devicesListBox.SelectedIndex].Hardware == 400)
            {
                //get the Independence index of timegate 
                string url_info = "http://app.divoom-gz.com/Channel/Get5LcdInfoV2?DeviceType=LCD&DeviceId=" + _localList.DeviceList[_devicesListBox.SelectedIndex].DeviceId;
                string IndependenceStr = HttpGet(url_info, "");
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
            string response_info;
            HttpPost("http://" + _deviceIpAddr + ":80/post", para_info, out response_info);

        }
        private void divoomList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DivoomSendSelectClock();



        }
        private void LCDList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string raw_value = _lcdList.SelectedItems[0].ToString();
            _selectedLcdId = Convert.ToInt32(raw_value) - 1;

            if(_localList != null && _localList.DeviceList!=null && _localList.DeviceList.Count() > 0)
            {
                if (_devicesListBox.SelectedIndex > 0 && _devicesListBox.SelectedIndex < _localList.DeviceList.Count())
                {
                    DivoomSendSelectClock();
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
