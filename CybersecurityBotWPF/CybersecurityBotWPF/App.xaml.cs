using System.Windows;

namespace CybersecurityBotWPF
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize database
            var dbHelper = new DatabaseHelper();
            dbHelper.InitializeDatabase();
        }
    }
}