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
            if (_deviceIpAddr == null || _localList == null || _localList.DivoomDeviceList == null || _localList.DivoomDeviceList.Length == 0)
            {
                return;

            }
            string CpuTemp_value = "--", CpuUse_value = "--", GpuTemp_value = "--", GpuUse_value = "--", DispUse_value = "--", HardDiskUse_value = "--";

            DevicePostList PostInfo = new DevicePostList();
            DevicePostItem PostItem = new DevicePostItem();
            PostInfo.Command = "Device/UpdatePCParaInfo";
            PostInfo.ScreenList = new DevicePostItem[1];
            PostItem.DispData = new string[6];

            if (_deviceIpAddr.Length > 0)
            {
                PostItem.LcdId = _selectedLcdId;
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
                                CpuTemp_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                CpuTemp_value += "C";
                            }
                            else if (_computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                            {
                                CpuUse_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                if (CpuUse_value.Length > 2)
                                {
                                    CpuUse_value = CpuUse_value.Substring(0, 2);
                                }
                                CpuUse_value += "%";
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
                                GpuTemp_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                GpuTemp_value += "C";
                            }
                            else if (_computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                            {
                                GpuUse_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                if (GpuUse_value.Length > 2)
                                {
                                    GpuUse_value = GpuUse_value.Substring(0, 2);
                                }
                                GpuUse_value += "%";
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
                                HardDiskUse_value = _computer.Hardware[i].Sensors[j].Value.ToString();
                                HardDiskUse_value += "C";
                                break;
                            }
                        }
                    }
                }

                MEMORYSTATUSEX memInfo = new MEMORYSTATUSEX();
                memInfo.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));

                GlobalMemoryStatusEx(ref memInfo);

                DispUse_value = ((memInfo.ullTotalPhys - memInfo.ullAvailPhys) * 100 / memInfo.ullTotalPhys).ToString().Substring(0, 2) + "%";
                PostItem.DispData[2] = CpuTemp_value;
                PostItem.DispData[0] = CpuUse_value;
                PostItem.DispData[3] = GpuTemp_value;
                PostItem.DispData[1] = GpuUse_value;
                PostItem.DispData[5] = HardDiskUse_value;
                PostItem.DispData[4] = DispUse_value;
                PostInfo.ScreenList[0] = PostItem;
                _cpuTemp.Text = "CpuTemp:" + CpuTemp_value;
                _cpuUse.Text = "CpuUse:" + CpuUse_value;
                _gpuTemp.Text = "GpuTemp:" + GpuTemp_value;
                _gpuUse.Text = "GpuUse:" + GpuUse_value;
                _hddUse.Text = "HddUse:" + HardDiskUse_value;
                _dispUse.Text = "DispUse:" + DispUse_value;
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
            _localList = JsonConvert.DeserializeObject<DeviceList>(device_list);
            _devicesListBox.Items.Clear();
            for (i = 0; _localList.DivoomDeviceList != null && i < _localList.DivoomDeviceList.Length; i++)
            {
                _devicesListBox.Items.Add(_localList.DivoomDeviceList[i].DeviceName);
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
            _deviceIpAddr = _localList.DivoomDeviceList[_devicesListBox.SelectedIndex].DevicePrivateIP;
            Console.WriteLine("selece items:" + _deviceIpAddr);

            if (_localList.DivoomDeviceList[_devicesListBox.SelectedIndex].Hardware == 400)
            {
                //get the Independence index of timegate 
                string url_info = "http://app.divoom-gz.com/Channel/Get5LcdInfoV2?DeviceType=LCD&DeviceId=" + _localList.DivoomDeviceList[_devicesListBox.SelectedIndex].DeviceId;
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

            DeviceSelectClockInfo PostInfo = new DeviceSelectClockInfo();

            PostInfo.LcdIndependence = _lcdIndependence;
            PostInfo.Command = "Channel/SetClockSelectId";
            PostInfo.LcdIndex = _lcdList.SelectedIndex;
            PostInfo.ClockId = 625;
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

            if(_localList != null && _localList.DivoomDeviceList!=null && _localList.DivoomDeviceList.Count() > 0)
            {
                if (_devicesListBox.SelectedIndex > 0 && _devicesListBox.SelectedIndex < _localList.DivoomDeviceList.Count())
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
            foreach (IHardware subHardware in hardware.SubHardware)
                subHardware.Accept(this);
        }

        public void VisitSensor(ISensor sensor) { }

        public void VisitParameter(IParameter parameter) { }
    }

}
