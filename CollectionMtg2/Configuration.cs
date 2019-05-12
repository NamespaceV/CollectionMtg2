namespace CollectionMtg2
{
    using CollectionMtg2.CollectionDiff;
    using CollectionMtg2.Deckbox;
    using CollectionMtg2.ScryfallApi;
    using CollectionMtg2.ViewModel;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;

    class Configuration
    {
        public static HttpClient httpClient = new HttpClient();

        public static IServiceProvider GetServiceProvider() {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(httpClient);
            serviceCollection.AddScoped<CollectionComparer>();
            serviceCollection.AddScoped<ScryfallApiClient>();
            serviceCollection.AddScoped<DeckboxExportParser>();
            serviceCollection.AddScoped<MainWindowViewModel>();
            serviceCollection.AddScoped<MainWindow>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
