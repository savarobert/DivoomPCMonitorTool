using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DivoomPcMonitor.Infrastructure
{
    public static class HttpClientProvider
    {
        public static IServiceProvider? ServiceProvider { get; set; }

        public static HttpClient CreateClient(string? name = null)
        {
            if (ServiceProvider != null)
            {
                var factory = ServiceProvider.GetService<IHttpClientFactory>();
                if (factory != null)
                {
                    return name is null ? factory.CreateClient() : factory.CreateClient(name);
                }
            }

            return new HttpClient();
        }
    }
}
