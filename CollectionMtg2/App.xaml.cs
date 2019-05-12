
namespace CollectionMtg2
{
    using System.Windows;
    using Microsoft.Extensions.DependencyInjection;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var sp = Configuration.GetServiceProvider();
            var mw = sp.GetRequiredService<MainWindow>();
            mw.Show();
        }
    }
}
