namespace DivoomPcMonitor.Domain.Contracts
{
    public record DevicePostItem
    {
        public int LcdId { get; init; }

        public string[] DispData { get; init; } = Array.Empty<string>();
    }
}
