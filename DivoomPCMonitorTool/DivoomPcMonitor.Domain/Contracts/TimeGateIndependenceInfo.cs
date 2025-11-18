namespace DivoomPcMonitor.Domain.Contracts
{
    public record TimeGateIndependenceInfo
    {
        public int LcdIndependence { get; init; }
        public int ChannelType { get; init; }
        public int ClockId { get; init; }
    }
}
