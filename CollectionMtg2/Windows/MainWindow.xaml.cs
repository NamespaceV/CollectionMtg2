namespace CollectionMtg2.Windows
{
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
