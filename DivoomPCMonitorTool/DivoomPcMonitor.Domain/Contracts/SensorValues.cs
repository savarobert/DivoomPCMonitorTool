using System;
using System.Collections.Generic;
using System.Text;

namespace DivoomPcMonitor.Domain.Contracts
{
    public record SensorValues
    {
        public string CpuTempValue { get; set; } = "--";

        public string CpuUseValue { get; set; } = "--";
        public string GpuTempValue { get; set; } = "--";
        public string GpuUseValue { get; set; } = "--";
        public string RamUseValue { get; set; } = "--";
        public string HardDiskUseValue { get; set; } = "--";
    }
}
