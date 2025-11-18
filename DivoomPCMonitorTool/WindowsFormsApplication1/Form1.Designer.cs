using LibreHardwareMonitor.Hardware;

namespace WindowsFormsApplication1
{
    public class DivoomDeviceInfo
    {
        public int DeviceId { get; set; }

        public int Hardware { get; set; }

        public string DeviceName { get; set; }
        public string DevicePrivateIP { get; set; }
        public string DeviceMac { get; set; }

    }
    public class DivoomDeviceList
    {

        public DivoomDeviceInfo[] DeviceList { get; set; }

    }
    partial class Form1
    {
        private System.Windows.Forms.ListBox _devicesListBox;
        private System.Windows.Forms.ListBox _lcdList;
        private System.Windows.Forms.Button _refreshList;
        private System.Windows.Forms.TextBox _cpuUse;
        private System.Windows.Forms.TextBox _cpuTemp;
        private System.Windows.Forms.TextBox _gpuUse;
        private System.Windows.Forms.TextBox _gpuTemp;
        private System.Windows.Forms.TextBox _dispUse;
        private System.Windows.Forms.TextBox _hddUse;
        private System.Windows.Forms.Label _lcdMsg;
        private System.Windows.Forms.Label _deviceListMsg;
        private System.Windows.Forms.Label _hardwareInfo;
        private DivoomDeviceList _localList;
        private int _selectedLcdId;
        private System.Windows.Forms.Timer _timer;
        private string _deviceIpAddr;
        private int _lcdIndependence;

        private UpdateVisitor _updateVisitor;
        private Computer _computer;

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            _refreshList = new System.Windows.Forms.Button();
            _cpuUse = new System.Windows.Forms.TextBox();
            _cpuTemp = new System.Windows.Forms.TextBox();
            _gpuUse = new System.Windows.Forms.TextBox();
            _gpuTemp = new System.Windows.Forms.TextBox();
            _dispUse = new System.Windows.Forms.TextBox();
            _hddUse = new System.Windows.Forms.TextBox();
            _devicesListBox = new System.Windows.Forms.ListBox();
            _lcdList = new System.Windows.Forms.ListBox();
            _lcdMsg = new System.Windows.Forms.Label();
            _deviceListMsg = new System.Windows.Forms.Label();
            _hardwareInfo = new System.Windows.Forms.Label();
            SuspendLayout();

            // 
            // DeviceListMsg
            // 
            _deviceListMsg.Location = new System.Drawing.Point(100, 10);
            _deviceListMsg.Name = "DeviceListMsg";
            _deviceListMsg.Size = new System.Drawing.Size(100, 20);
            _deviceListMsg.Text = "Device list";
            // 
            // devicesListBox
            // 
            _devicesListBox.Location = new System.Drawing.Point(100, 30);
            _devicesListBox.Name = "DeviceList";
            _devicesListBox.Size = new System.Drawing.Size(100, 230);
            _devicesListBox.TabIndex = 2;//CheckedListBoxes
            _devicesListBox.SelectedIndexChanged += new System.EventHandler(divoomList_SelectedIndexChanged);
            // 
            // refresh list
            // 
            _refreshList.Location = new System.Drawing.Point(100, 260);
            _refreshList.Name = "refresh list";
            _refreshList.Size = new System.Drawing.Size(100, 23);
            _refreshList.TabIndex = 0;
            _refreshList.Text = "refresh list";
            _refreshList.UseVisualStyleBackColor = true;
            _refreshList.Click += new System.EventHandler(refreshList_Click);

            // 
            // LCDMsg
            // 
            _lcdMsg.Location = new System.Drawing.Point(20, 10);
            _lcdMsg.Name = "DeviceListMsg";
            _lcdMsg.Size = new System.Drawing.Size(80, 20);
            _lcdMsg.Text = "Select LCD";
            // LCDList
            // 
            _lcdList.Location = new System.Drawing.Point(20, 30);
            _lcdList.Name = "LCDList";
            _lcdList.Size = new System.Drawing.Size(20, 100);
            _lcdList.TabIndex = 2;//CheckedListBoxes
            _lcdList.SelectedIndexChanged += new System.EventHandler(LCDList_SelectedIndexChanged);
            _lcdList.Items.Add("1");
            _lcdList.Items.Add("2");
            _lcdList.Items.Add("3");
            _lcdList.Items.Add("4");
            _lcdList.Items.Add("5");
            _lcdList.SetSelected(0, true);


            // 
            // HardwareInfo
            // 
            _hardwareInfo.Location = new System.Drawing.Point(220, 10);
            _hardwareInfo.Name = "HardwareInfo";
            _hardwareInfo.Size = new System.Drawing.Size(180, 20);
            _hardwareInfo.Text = "Hardware information";

            // 
            // CpuUse
            // 
            _cpuUse.Location = new System.Drawing.Point(220, 30);
            _cpuUse.Name = "PCUser";
            _cpuUse.Size = new System.Drawing.Size(100, 20);
            _cpuUse.TabIndex = 1;
            // 

            // CpuTemp
            // 
            _cpuTemp.Location = new System.Drawing.Point(220, 60);
            _cpuTemp.Name = "textBox1";
            _cpuTemp.Size = new System.Drawing.Size(100, 20);
            _cpuTemp.TabIndex = 1;

            // 
            // GpuUse
            // 
            _gpuUse.Location = new System.Drawing.Point(220, 90);
            _gpuUse.Name = "PCUser";
            _gpuUse.Size = new System.Drawing.Size(100, 20);
            _gpuUse.TabIndex = 1;
            // 

            // GpuTemp
            // 
            _gpuTemp.Location = new System.Drawing.Point(220, 120);
            _gpuTemp.Name = "textBox1";
            _gpuTemp.Size = new System.Drawing.Size(100, 20);
            _gpuTemp.TabIndex = 1;


            // DispUse
            // 
            _dispUse.Location = new System.Drawing.Point(220, 150);
            _dispUse.Name = "textBox1";
            _dispUse.Size = new System.Drawing.Size(100, 20);
            _dispUse.TabIndex = 1;


            // HddUse
            // 
            _hddUse.Location = new System.Drawing.Point(220, 180);
            _hddUse.Name = "textBox1";
            _hddUse.Size = new System.Drawing.Size(100, 20);
            _hddUse.TabIndex = 1;

            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(400, 300);
            Controls.Add(_cpuUse);
            Controls.Add(_cpuTemp);
            Controls.Add(_gpuUse);
            Controls.Add(_gpuTemp);
            Controls.Add(_dispUse);
            Controls.Add(_hddUse);
            Controls.Add(_refreshList);
            Controls.Add(_devicesListBox);
            Controls.Add(_lcdList);
            Controls.Add(_lcdMsg);
            Controls.Add(_deviceListMsg);
            Controls.Add(_hardwareInfo);
            System.AppDomain.CurrentDomain.ProcessExit += new System.EventHandler(CurrentDomain_ProcessExit);  
            Name = "DivoomPcTool";
            Text = "DivoomPcTool";
            _selectedLcdId = 0;
            _deviceIpAddr = "";

            _updateVisitor = new UpdateVisitor();
            _computer = new Computer();
            _computer.IsStorageEnabled = true;
            _computer.Open();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();//创建定时器
            timer.Tick += new System.EventHandler(DivoomSendHttpInfo);//事件处理
            timer.Enabled = true;//设置启用定时器
            timer.Interval = 2000;//执行时间
            timer.Start();//开启定时器
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

    }
}

