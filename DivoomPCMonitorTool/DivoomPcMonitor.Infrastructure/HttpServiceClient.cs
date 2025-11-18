using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DivoomPcMonitor.Domain.Clients;

namespace DivoomPcMonitor.Infrastructure
{
    public class HttpServiceClient : IHttpServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetAsync(string url)
        {
            using var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<string> PostJsonAsync(string url, string json)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> PostFormAsync(string url, string formData)
        {
            using var client = _httpClientFactory.CreateClient();
            using var content = new StringContent(formData, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync(url, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
