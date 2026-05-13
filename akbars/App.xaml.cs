using System.Windows;
using akbars.Services;

namespace akbars
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppServices.Initialize();
            base.OnStartup(e);
        }
    }
}
