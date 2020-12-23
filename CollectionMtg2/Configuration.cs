namespace CollectionMtg2
{
    using CollectionMtg2.Windows;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;

    class Configuration
    {
        public static HttpClient httpClient = new HttpClient();

        public static IServiceProvider GetServiceProvider() {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(httpClient);
            serviceCollection.AddScoped<MainWindowViewModel>();
            serviceCollection.AddScoped<MainWindow>();
            serviceCollection.Scan(s => s.FromCallingAssembly().AddClasses().AsImplementedInterfaces());
            return serviceCollection.BuildServiceProvider();
        }
    }
}
