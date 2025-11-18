namespace DivoomPcMonitor.Domain.Contracts
{
    public record DivoomDeviceList
    {
        public DeviceInfo[] DeviceList { get; init; } = Array.Empty<DeviceInfo>();
    }
}
