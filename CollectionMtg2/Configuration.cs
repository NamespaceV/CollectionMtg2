namespace CollectionMtg2
{
    using CollectionMtg2.ViewModel;
    using Unity;

    class Configuration
    {
        public static UnityContainer GetContainer() {
            var container = new UnityContainer();
            container.RegisterType<MainWindowViewModel>();
            return container;
        }
    }
}
