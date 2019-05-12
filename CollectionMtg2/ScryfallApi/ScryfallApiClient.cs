namespace CollectionMtg2.ScryfallApi
{
    using CollectionMtg2.DomainModel;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class ScryfallApiClient
    {
        private readonly HttpClient _httpClient;

        public ScryfallApiClient(HttpClient client)
        {
            _httpClient = client;
        }

        class ScryfallImageUriSet
        {
            public string png;
        }

        class ScryfallCard
        {
            public string name;
            public int collector_number;
            public string type_line;
            public ScryfallImageUriSet image_uris;

            public bool IsBasicLand()
            {
                return type_line.StartsWith("Basic Land —");
            }

            public Card ToModelCard()
            {
                return new Card()
                {
                    CardName = name,
                    DisplayImage = image_uris.png
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
            var jsonString = await _httpClient.GetStringAsync(uri);
            var response = JsonConvert.DeserializeObject<ScryfallCardResponce>(jsonString);
            allCards.AddRange(response.data);
            while (response.has_more)
            {
                await Task.Delay(100);
                jsonString = await _httpClient.GetStringAsync(response.next_page);
                response = JsonConvert.DeserializeObject<ScryfallCardResponce>(jsonString);
                allCards.AddRange(response.data);
            }
            return allCards
                .Where(c => !c.IsBasicLand() && c.collector_number <= maxCardNo)
                .Select(c => c.ToModelCard())
                .ToList();
        }

        internal async Task LinkImages(CardCollection collection)
        {
            //foreach (var c in collection.cardPositions)
            //{
            //    c.CardType.DisplayImage = @"F:\ProjectMtg2\images\404.png";
            //}


            var allData = new List<ScryfallCard>();

            var uniqueNames = collection.cardPositions.Select(cp => cp.CardType.CardName).Distinct();
            var batchStart = 0;
            while (batchStart < uniqueNames.Count())
            {
                var batchNames = uniqueNames.Skip(batchStart).Take(75).ToList();
                batchStart += 75;
                await AskForData(batchNames, allData);
            }

            foreach (var c in collection.cardPositions)
            {
                var image = allData
                    .FirstOrDefault(scryData => scryData.name == c.CardType.CardName)
                    ?.image_uris.png ?? @"F:\ProjectMtg2\images\404.png";
                c.CardType.DisplayImage = image;
            }

            //return Task.CompletedTask;

            //var imageUri = response.data.FirstOrDefault()?.image_uris?.png ?? ;

        }

        private async Task AskForData(List<string> cardNames, List<ScryfallCard> allData)
        {
            if (cardNames.Count > 75)
            {
                throw new System.Exception($"Card limit exceeded, {cardNames.Count} is more than 75");
            }
            string uri = @"https://api.scryfall.com/cards/collection";
            var data = new
            {
                identifiers = new List<object>()
            };
            foreach (var cardName in cardNames)
            {
                data.identifiers.Add(new { name = cardName });
            }

            var myContent = JsonConvert.SerializeObject(data);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var postResponce = await _httpClient.PostAsync(uri, byteContent);
            var jsonString = await postResponce.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<ScryfallCardResponce>(jsonString);
            allData.AddRange(response.data);
            while (response.has_more)
            {
                await Task.Delay(100);
                jsonString = await _httpClient.GetStringAsync(response.next_page);
                response = JsonConvert.DeserializeObject<ScryfallCardResponce>(jsonString);
                allData.AddRange(response.data);
            }
        }
    }
}
