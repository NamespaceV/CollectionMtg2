namespace CollectionMtg2.ScryfallApi
{
    using CollectionMtg2.DomainModel;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    class ScryfallApiClient
    {
        public ScryfallApiClient()
        {
        }

        class ScryfallCard
        {
            public string name;
            public int collector_number;
            public string type_line;

            public bool IsBasicLand()
            {
                return type_line.StartsWith("Basic Land —");
            }

            public Card ToModelCard()
            {
                return new Card()
                {
                    CardName = name
                };
            }
        }

        class ScryfallCardResponce
        {
            public bool has_more;
            public string next_page;
            public List<ScryfallCard> data;
        }

        public async Task<ICollection<Card>> GetCardsFromSet(string setShortName, int maxCardNo)
        {
            string uri = @"https://api.scryfall.com/cards/search";
            uri += $"?q=e%3A{setShortName}";
            var allCards = new List<ScryfallCard>();
            using (var client = new HttpClient())
            {
                var jsonString = await client.GetStringAsync(uri);
                var response = JsonConvert.DeserializeObject<ScryfallCardResponce>(jsonString);
                allCards.AddRange(response.data);
                while (response.has_more)
                {
                    await Task.Delay(100);
                    jsonString = await client.GetStringAsync(response.next_page);
                    response = JsonConvert.DeserializeObject<ScryfallCardResponce>(jsonString);
                    allCards.AddRange(response.data);
                }
            }
            return allCards
                .Where(c => !c.IsBasicLand() && c.collector_number <= maxCardNo)
                .Select(c => c.ToModelCard())
                .ToList();
        }

    }
}
