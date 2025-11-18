using System;
using System.Collections.Generic;
using System.Text;

namespace DivoomPcMonitor.Domain.Contracts
{
    public record SensorValues
    {
        public string CpuTemp_value = "--";

        public string CpuUse_value = "--";
        public string GpuTemp_value = "--";
        public string GpuUse_value = "--";
        public string RamUse_value = "--";
        public string HardDiskUse_value = "--";
    }
}
