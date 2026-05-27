using System.Windows;

namespace CybersecurityBotWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set application-level error handling
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}\n\nThe application will continue running.",
                          "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}