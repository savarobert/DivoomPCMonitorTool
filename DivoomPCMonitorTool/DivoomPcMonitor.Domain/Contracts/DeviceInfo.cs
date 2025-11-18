using System;
using System.Collections.Generic;
using System.Text;

namespace DivoomPcMonitor.Domain.Contracts
{
    public class DeviceInfo
    {
        public int DeviceId { get; set; }

        public int Hardware { get; set; }

        public string DeviceName { get; set; }
        public string DevicePrivateIP { get; set; }
        public string DeviceMac { get; set; }
    }
}
