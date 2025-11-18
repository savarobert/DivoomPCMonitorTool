using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DivoomPcMonitor.Infrastructure;

namespace DivoomPCMonitorTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddHttpClient();
                    // Register the WinForms main form for constructor injection
                    services.AddSingleton<MainForm>();
                })
                .UseConsoleLifetime();

            var host = builder.Build();

            // Resolve MainForm from the DI container so dependencies are injected
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = host.Services.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }
    }
}
