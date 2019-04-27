namespace CollectionMtg2.ViewModel
{
    using CollectionMtg2.CollectionDiff;
    using CollectionMtg2.Commands;
    using CollectionMtg2.Deckbox;
    using CollectionMtg2.DomainModel;
    using CollectionMtg2.ScryfallApi;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using static CollectionMtg2.DomainModel.CardCollection;

    class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _outputText;
        public string OutputText { get => _outputText; set { _outputText = value; OnPropertyChanged(); } }

        public ObservableCollection<Position> CardsList { get; set; } = new ObservableCollection<Position>();
        private Position _selectedCard;
        public Position SelectedCard { get => _selectedCard; set { _selectedCard = value; OnPropertyChanged(); } }

        public string SetName { get; set; }
        public string MaxCardInSet { get; set; }

        private string _collectionPath;
        public string CollectionPath {
            get {
                return _collectionPath;
            }
            set
            {
                _collectionPath = value;
                Properties.Settings.Default.CardCollectionFileName = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        public string CollectionSetFilter { get; set; }

        public string CardListPath { get; set; }

        public bool WantPlaysets { get; set; }

        public string DisplayedImagePath { get; set; }

        public ICommand LoadCollectionCommand { get; }
        public ICommand LoadSetCommand { get; }
        public ICommand SaveSetCommand { get; }
        public ICommand CompareCommand { get; }

        private readonly DeckboxExportParser _deckboxParser;
        private readonly ScryfallApiClient _scryfallApiClient;
        private readonly CollectionComparer _collectionComparer;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(
            DeckboxExportParser deckboxParser,
            ScryfallApiClient scryfallApiClient,
            CollectionComparer collectionComparer)
        {
            _deckboxParser = deckboxParser;
            _scryfallApiClient = scryfallApiClient;
            _collectionComparer = collectionComparer;

            LoadCollectionCommand = new BaseCommand(LoadCollection);
            LoadSetCommand = new BaseCommand(LoadSet);
            SaveSetCommand = new BaseCommand(SaveSet);
            CompareCommand = new BaseCommand(CompareCollectionWithSet);

            SetName = "RNA";
            MaxCardInSet = "9999";
            CollectionPath = Properties.Settings.Default.CardCollectionFileName;
            CollectionSetFilter = "Ravnica Allegiance";
            CardListPath = @"F:\ProjectMtg2\setLists\rna.txt";
            DisplayedImagePath = @"https://img.scryfall.com/cards/large/en/gtc/193.jpg?1517813031";
        }

        private async Task LoadSet()
        {
            OutputText = "Getting Set...";
            var cards = await _scryfallApiClient.GetCardsFromSet(SetName, int.Parse(MaxCardInSet));
            OutputText = "";
            CardsList.Clear();
            foreach (var card in cards)
            {
                var pos = new Position() { CardType = card, CardCount = 0 };
                CardsList.Add(pos);
            }
        }

        private async Task SaveSet()
        {
            OutputText = "Getting Set...";
            var cards = await _scryfallApiClient.GetCardsFromSet(SetName, int.Parse(MaxCardInSet));
            OutputText = "";
            CardsList.Clear();
            using (var writer = new StreamWriter(File.Open(CardListPath, FileMode.OpenOrCreate)))
            foreach (var card in cards)
            {
                var pos = new Position() { CardType = card, CardCount = 0 };
                CardsList.Add(pos);
                await writer.WriteLineAsync(card.CardName);
            }
        }


        private async Task LoadCollection()
        {
            OutputText = "Loading collection...";
            var collection = await _deckboxParser.ReadCollectionCsv(CollectionPath, CollectionSetFilter);
            await _scryfallApiClient.LinkImages(collection);
            //SelectedCard = new Position() { CardType = new Card() { CardName = "dasdas" }, CardCount = 5 };
            OutputText = "";
            CardsList.Clear();
            foreach (var cardPosition in collection.cardPositions)
            {
                CardsList.Add(cardPosition);
            }
        }

        private async Task CompareCollectionWithSet()
        {
            OutputText = "Comparing...";
            var collection = await _deckboxParser.ReadCollectionCsv(CollectionPath, CollectionSetFilter);
            var missing = await _collectionComparer.GetMissingCards(collection, CardListPath, WantPlaysets);
            await _scryfallApiClient.LinkImages(missing);
            OutputText = "";
            CardsList.Clear();
            foreach (var cardPosition in missing.cardPositions)
            {
                CardsList.Add(cardPosition);
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
