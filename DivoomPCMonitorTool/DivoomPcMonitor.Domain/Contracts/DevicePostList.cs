namespace DivoomPcMonitor.Domain.Contracts
{
    public class DevicePostList
    {
        public string Command { get; set; }
        public DevicePostItem[] ScreenList { get; set; }
    }
}
