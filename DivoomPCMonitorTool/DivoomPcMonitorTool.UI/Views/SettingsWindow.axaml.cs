using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace DivoomPcMonitorTool.UI.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            var ok = this.FindControl<Button>("OkButton");
            if (ok != null)
            {
                ok.Click += Ok_Click;
            }

            var cancel = this.FindControl<Button>("CancelButton");
            if (cancel != null)
            {
                cancel.Click += (s, e) => Close(null);
            }
        }

        private void Ok_Click(object? sender, RoutedEventArgs e)
        {
            Close(null);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}