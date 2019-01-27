namespace CollectionMtg2
{
    using CollectionMtg2.ViewModel;
    using System.Windows;
    using Unity;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Configuration.GetContainer().Resolve<MainWindowViewModel>();
        }

    }
}
