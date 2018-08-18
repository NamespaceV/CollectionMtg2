namespace CollectionMtg2
{
    using CollectionMtg2.CollectionDiff;
    using CollectionMtg2.DeckboxApi;
    using CollectionMtg2.ScryfallApi;
    using System;
    using System.Windows;
    using Unity;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var container = Configuration.GetContainer();
            var api = container.Resolve<ScryfallApiClient>();
            var cards = await api.GetCardsFromSet(SetTextBox.Text, int.Parse(NoLimitTextBox.Text));
            OutputTextBox.Clear();
            foreach (var card in cards)
            {
                OutputTextBox.Text += card.CardName + Environment.NewLine;
            }
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            var container = Configuration.GetContainer();
            var api = container.Resolve<DeckboxApiClient>();
            var collction = await api.ReadCollectionCsv(CollectionPathTextBox.Text, CollectionSetFilterTextBox.Text);
            OutputTextBox.Clear();
            foreach (var cardPosition in collction.cardPositions)
            {
                OutputTextBox.Text += cardPosition.CardCount+" "+cardPosition.CardType.CardName + Environment.NewLine;
            }

        }

        private async void Button3_Click(object sender, RoutedEventArgs e)
        {
            var container = Configuration.GetContainer();
            var api = container.Resolve<DeckboxApiClient>();
            var comparer = container.Resolve<CollectionComparer>();
            var collction = await api.ReadCollectionCsv(CollectionPathTextBox.Text, CollectionSetFilterTextBox.Text);
            var missing = await comparer.GetMissingCards(collction, ListPathTextBox.Text, PlaysetsCheckbox.IsChecked == true);

            OutputTextBox.Clear();
            foreach (var cardPosition in missing.cardPositions)
            {
                OutputTextBox.Text += cardPosition.CardCount + " " + cardPosition.CardType.CardName + Environment.NewLine;
            }
        }
    }
}
