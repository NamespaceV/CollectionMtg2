namespace CollectionMtg2.Deckbox
{
    using CollectionMtg2.DomainModel;
    using System.IO;
    using System.Threading.Tasks;
    using CsvHelper;
    using CsvHelper.Configuration;

    public class DeckboxExportParser
    {
        public sealed class DeckboxCardRowMap : ClassMap<DeckboxCardRow>
        {
            public DeckboxCardRowMap()
            {
                Map(m => m.Count).Name("Count");
                Map(m => m.TradelistCount).Name("Tradelist Count");
                Map(m => m.Name).Name("Name");
                Map(m => m.Edition).Name("Edition");
                Map(m => m.CardNumber).Name("Card Number");
                Map(m => m.Condition).Name("Condition");
                Map(m => m.Language).Name("Language");
                Map(m => m.Foil).Name("Foil");
                Map(m => m.Signed).Name("Signed");
                Map(m => m.ArtistProof).Name("Artist Proof");
                Map(m => m.AlteredArt).Name("Altered Art");
                Map(m => m.Misprint).Name("Misprint");
                Map(m => m.Promo).Name("Promo");
                Map(m => m.Textless).Name("Textless");
                Map(m => m.MyPrice).Name("My Price");
            }
        }
        public class DeckboxCardRow
        {
            public int Count;
            public string TradelistCount;
            public string Name;
            public string Edition;
            public string CardNumber;
            public string Condition;
            public string Language;
            public string Foil;
            public string Signed;
            public string ArtistProof;
            public string AlteredArt;
            public string Misprint;
            public string Promo;
            public string Textless;
            public string MyPrice;
        }

        public async Task<CardCollection> ReadCollectionCsv(string filePath, string setFilter)
        {
            var cardCollection = new CardCollection();
            using (TextReader fileReader = File.OpenText(filePath))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.RegisterClassMap<DeckboxCardRowMap>();
                csv.Configuration.Delimiter = ",";

                await csv.ReadAsync();
                csv.ReadHeader();
                while (await csv.ReadAsync())
                {
                    var record = csv.GetRecord<DeckboxCardRow>();
                    if (record.Edition != setFilter)
                    {
                        continue;
                    }
                    cardCollection.AddCard(record.Name, record.Count);
                }
            }
            return cardCollection;
        }
    }
}
