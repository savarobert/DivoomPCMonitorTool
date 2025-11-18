namespace DivoomPcMonitor.Domain.Contracts
{
    public record DevicePostList
    {
        public string Command { get; init; } = string.Empty;
        public DevicePostItem[] ScreenList { get; init; } = Array.Empty<DevicePostItem>();
    }
}
