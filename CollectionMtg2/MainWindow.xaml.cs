namespace CollectionMtg2
{
    using CollectionMtg2.ViewModel;
    using System.Windows;

    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            DataContext = mainWindowViewModel;
        }

    }
}
