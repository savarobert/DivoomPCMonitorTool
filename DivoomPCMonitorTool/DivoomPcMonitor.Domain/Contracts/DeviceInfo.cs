using System;
using System.Collections.Generic;
using System.Text;

namespace DivoomPcMonitor.Domain.Contracts
{
    public record DeviceInfo
    {
        public int DeviceId { get; init; }

        public int Hardware { get; init; }

        public string DeviceName { get; init; } = string.Empty;
        public string DevicePrivateIP { get; init; } = string.Empty;
        public string DeviceMac { get; init; } = string.Empty;
    }
}
