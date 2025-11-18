using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DivoomPcMonitorTool.UI.Views;

namespace DivoomPcMonitorTool.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";

        public IAsyncRelayCommand SettingsCommand { get; }
        public IRelayCommand ExitCommand { get; }

        public MainWindowViewModel()
        {
            SettingsCommand = new AsyncRelayCommand(OpenSettingsAsync);
            ExitCommand = new RelayCommand(ExitApp);
        }

        private async Task OpenSettingsAsync()
        {
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var settings = new SettingsWindow();

            if (lifetime?.MainWindow is Window owner)
            {
                // Show as modal dialog when running as classic desktop app
                await settings.ShowDialog<object>(owner);
            }
            else
            {
                // Fallback: just show non-modal
                settings.Show();
            }
        }

        private void ExitApp()
        {
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            lifetime?.Shutdown();
        }
    }
}
