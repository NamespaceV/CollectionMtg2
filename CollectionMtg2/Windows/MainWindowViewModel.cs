namespace CollectionMtg2.Windows
{
    using CollectionMtg2.CQS.Collection;
    using CollectionMtg2.CQS.Core;
    using CollectionMtg2.CQS.Deckbox;
    using CollectionMtg2.CQS.Scryfall;
    using CollectionMtg2.CQS.Scryfall.Helper;
    using CollectionMtg2.DomainModel;
    using CollectionMtg2.WPF;
    using Microsoft.Win32;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using static CollectionMtg2.DomainModel.CardCollection;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _windowTitle;
        public string WindowTitle { get => _windowTitle; set { _windowTitle = value; OnPropertyChanged(); } }

        private string _outputText;
        public string OutputText { get => _outputText; set { _outputText = value; OnPropertyChanged(); } }

        public ObservableCollection<Position> CardsList { get; set; } = new ObservableCollection<Position>();
        private Position _selectedCard;
        public Position SelectedCard { get => _selectedCard; set { _selectedCard = value; OnPropertyChanged(); } }

        public string SetName { get; set; }
        public string MaxCardInSet { get; set; }

        private const string _programName = "Mtg Connellection Tool";

        private string _collectionPath;
        public string CollectionPath
        {
            get
            {
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

        private string _cardListPath;
        public string CardListPath { get => _cardListPath; set { _cardListPath = value;  OnPropertyChanged(); } }

        public bool WantPlaysets { get; set; }

        public string DisplayedImagePath { get; set; }

        public ICommand OpenCollectionCommand { get; }
        public ICommand LoadCollectionCommand { get; }

        public ICommand OpenSetCommand { get; }
        public ICommand LoadSetCommand { get; }
        public ICommand SaveSetCommand { get; }
        public ICommand CompareCommand { get; }

        public ICommand CopyToClipboardCommand { get; }

        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IScryfallApiClient _scryfallApiClient;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(
            IQueryDispatcher queryDispatcher,
            IScryfallApiClient scryfallApiClient)
        {
            _queryDispatcher = queryDispatcher;
            _scryfallApiClient = scryfallApiClient;

            OpenCollectionCommand = new BaseCommand(OpenCollection);
            LoadCollectionCommand = new BaseCommand(LoadCollection);
            OpenSetCommand = new BaseCommand(OpenSet);
            LoadSetCommand = new BaseCommand(LoadSet);
            SaveSetCommand = new BaseCommand(SaveSet);
            CompareCommand = new BaseCommand(CompareCollectionWithSet);

            CopyToClipboardCommand = new BaseCommand(CopyToClipboard);

            WindowTitle = _programName;
            SetName = "RNA";
            MaxCardInSet = "9999";
            CollectionPath = Properties.Settings.Default.CardCollectionFileName;
            CollectionSetFilter = "Ravnica Allegiance";
            CardListPath = @"F:\Projects\ProjectMtg2\setLists\rna.txt";
            DisplayedImagePath = @"https://img.scryfall.com/cards/large/en/gtc/193.jpg?1517813031";
        }

        private Task CopyToClipboard()
        {
            var text = string.Join("\n", CardsList.Select(c => c.CardCount + " " + c.CardType.CardName));
            Clipboard.SetText(text);
            return Task.CompletedTask;
        }

        private Task OpenSet()
        {
            var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Path.GetDirectoryName(CardListPath)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CardListPath = openFileDialog.FileName;
            }
            return Task.CompletedTask;
        }

        private async Task LoadSet()
        {
            WindowTitle += " - Loading Set...";
            var cards = await _queryDispatcher.DispatchAsync<GetCardsFromSetQuery, ICollection<Card>>(
                new GetCardsFromSetQuery() {
                    SetNameAs3Letter = SetName,
                    MaxCardNo = int.Parse(MaxCardInSet)
                });
            OutputText = "";
            CardsList.Clear();
            foreach (var card in cards)
            {
                var pos = new Position() { CardType = card, CardCount = 0 };
                CardsList.Add(pos);
            }
            WindowTitle = _programName;
        }

        private async Task SaveSet()
        {
            WindowTitle += " - Saving Set...";
            var cards = await _queryDispatcher.DispatchAsync<GetCardsFromSetQuery, ICollection<Card>>(
                new GetCardsFromSetQuery() {
                    SetNameAs3Letter = SetName,
                    MaxCardNo = int.Parse(MaxCardInSet)
                });
            OutputText = "";
            CardsList.Clear();
            using (var writer = new StreamWriter(File.Open(CardListPath, FileMode.OpenOrCreate))) {
                foreach (var card in cards)
                {
                    var pos = new Position() { CardType = card, CardCount = 0 };
                    CardsList.Add(pos);
                    await writer.WriteLineAsync(card.CardName);
                }
            }
            WindowTitle = _programName;
        }

        private Task OpenCollection()
        {
            var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Path.GetDirectoryName(CollectionPath)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CollectionPath = openFileDialog.FileName;
            }
            return Task.CompletedTask;
        }

        private async Task LoadCollection()
        {
            WindowTitle += " - Loading collection...";
            var collection = await _queryDispatcher.DispatchAsync
                <ReadCollectionCsvQuery, CardCollection> ( new ReadCollectionCsvQuery()
            {
                CollectionFilePath = CollectionPath,
                SetNameFilter = CollectionSetFilter
            });

            var metadata = await _scryfallApiClient.GetCardsFromCollectionAsync(collection);
            foreach (var c in collection.cardPositions)
            {
                var cardData = metadata.FindMatching(c.CardType);
                var imageUrl = cardData?.image_uris?.png ?? @"F:\Projects\ProjectMtg2\images\404.png";
                c.CardType.DisplayImage = imageUrl;
            }

            OutputText = "";
            CardsList.Clear();
            foreach (var cardPosition in collection.cardPositions)
            {
                CardsList.Add(cardPosition);
            }
            WindowTitle = _programName;
        }

        private async Task CompareCollectionWithSet()
        {
            WindowTitle += " - Comparing...";
            var collection = await _queryDispatcher.DispatchAsync
               <ReadCollectionCsvQuery, CardCollection>(new ReadCollectionCsvQuery()
               {
                   CollectionFilePath = CollectionPath,
                   SetNameFilter = CollectionSetFilter
               });
            var missingCards = await _queryDispatcher.DispatchAsync
              <GetMissingCardsQuery, CardCollection>(new GetMissingCardsQuery()
              {
                  CollectedCards = collection,
                  WantedCardsListPath = CardListPath,
                  WantPlaysets = WantPlaysets
              });
            var metadata = await _scryfallApiClient.GetCardsFromCollectionAsync(missingCards);
            foreach (var c in missingCards.cardPositions)
            {
                var cardData = metadata.FindMatching(c.CardType);
                var imageUrl = cardData?.image_uris?.png ?? @"F:\Projects\ProjectMtg2\images\404.png";
                c.CardType.DisplayImage = imageUrl;
            }

            OutputText = "";
            CardsList.Clear();
            foreach (var cardPosition in missingCards.cardPositions)
            {
                CardsList.Add(cardPosition);
            }
            WindowTitle = _programName;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
