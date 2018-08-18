namespace CollectionMtg2
{
    using Unity;

    class Configuration
    {
        public static UnityContainer GetContainer() {
            var container = new UnityContainer();
            //container.RegisterType<>();
            return container;
        }
    }
}
