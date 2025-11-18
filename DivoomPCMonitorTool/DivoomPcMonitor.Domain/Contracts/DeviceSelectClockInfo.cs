namespace DivoomPcMonitor.Domain.Contracts
{
    public record DeviceSelectClockInfo
    {
        public int LcdIndependence { get; init; }
        public int DeviceId { get; init; }
        public int LcdIndex { get; init; }
        public int ClockId { get; init; }
        public string Command { get; init; } = string.Empty;
    }
}
