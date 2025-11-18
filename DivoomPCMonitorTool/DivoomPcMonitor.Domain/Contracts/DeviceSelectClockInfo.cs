namespace DivoomPcMonitor.Domain.Contracts
{
    public class DeviceSelectClockInfo
    {
        public int LcdIndependence { get; set; }
        public int DeviceId { get; set; }
        public int LcdIndex { get; set; }
        public int ClockId { get; set; }
        public string Command { get; set; }
    }
}
