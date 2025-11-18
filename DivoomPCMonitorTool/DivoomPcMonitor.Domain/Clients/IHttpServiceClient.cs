namespace DivoomPcMonitor.Domain.Clients
{
    public interface IHttpServiceClient
    {
        Task<string> GetAsync(string url);
        Task<string> PostJsonAsync(string url, string json);
        Task<string> PostFormAsync(string url, string formData);
    }
}
