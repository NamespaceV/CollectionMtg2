namespace CollectionMtg2
{
    using CollectionMtg2.ViewModel;
    using System.Net.Http;
    using Unity;

    class Configuration
    {
        public static HttpClient httpClient = new HttpClient();

        public static UnityContainer GetContainer() {
            var container = new UnityContainer();
            container.RegisterInstance(httpClient);
            container.RegisterType<MainWindowViewModel>();
            return container;
        }
    }
}
