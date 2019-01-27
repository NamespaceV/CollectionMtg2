namespace CollectionMtg2.ViewModel
{
    using CollectionMtg2.CollectionDiff;
    using CollectionMtg2.Commands;
    using CollectionMtg2.DeckboxApi;
    using CollectionMtg2.ScryfallApi;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;

    class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _outputText;
        public string OutputText { get => _outputText; set { _outputText = value; OnPropertyChanged(); } }

        public string SetName { get; set; }
        public string MaxCardInSet { get; set; }

        public string CollectionPath { get; set; }
        public string CollectionSetFilter { get; set; }

        public string CardListPath { get; set; }

        public bool WantPlaysets { get; set; }

        public ICommand LoadCollectionCommand { get; }
        public ICommand LoadSetCommand { get; }
        public ICommand CompareCommand { get; }

        private readonly DeckboxApiClient _deckboxParser;
        private readonly ScryfallApiClient _scryfallApiClient;
        private readonly CollectionComparer _collectionComparer;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(
            DeckboxApiClient deckboxParser,
            ScryfallApiClient scryfallApiClient,
            CollectionComparer collectionComparer)
        {
            _deckboxParser = deckboxParser;
            _scryfallApiClient = scryfallApiClient;
            _collectionComparer = collectionComparer;

            LoadCollectionCommand = new BaseCommand(LoadCollection);
            LoadSetCommand = new BaseCommand(LoadSet);
            CompareCommand = new BaseCommand(CompareCollectionWithSet);

            SetName = "GRN";
            MaxCardInSet = "9999";
            CollectionPath = @"F:\ProjectMtg2\collectionBackups\Inventory_1421686397875289_2019.January.27.csv";
            CollectionSetFilter = "Guilds of Ravnica";
            CardListPath = @"F:\ProjectMtg2\setLists\grn.txt";

        }

        private async Task LoadSet()
        {
            OutputText = "Getting Set...";
            var cards = await _scryfallApiClient.GetCardsFromSet(SetName, int.Parse(MaxCardInSet));
            OutputText = "";
            foreach (var card in cards)
            {
                OutputText += card.CardName + Environment.NewLine;
            }

        }

        private async Task LoadCollection()
        {
            OutputText = "Loading collection...";
            var collction = await _deckboxParser.ReadCollectionCsv(CollectionPath, CollectionSetFilter);
            OutputText = "";
            foreach (var cardPosition in collction.cardPositions)
            {
                OutputText += cardPosition.CardCount + " " + cardPosition.CardType.CardName + Environment.NewLine;
            }

        }

        private async Task CompareCollectionWithSet()
        {
            OutputText = "Comparing...";
            var collection = await _deckboxParser.ReadCollectionCsv(CollectionPath, CollectionSetFilter);
            var missing = await _collectionComparer.GetMissingCards(collection, CardListPath, WantPlaysets);
            OutputText = "";
            foreach (var cardPosition in missing.cardPositions)
            {
                OutputText += cardPosition.CardCount + " " + cardPosition.CardType.CardName + Environment.NewLine;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
