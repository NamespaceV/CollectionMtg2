namespace CollectionMtg2.CQS.Scryfall
{
    using CollectionMtg2.CQS.Core;
    using CollectionMtg2.CQS.Scryfall.Helper;
    using CollectionMtg2.DomainModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetCardsFromSetQuery
    {
        public string SetNameAs3Letter;
            
        public int? MaxCardNo;
    }

    public class GetCardsFromSetQueryHandler : IQueryHandler<GetCardsFromSetQuery, ICollection<Card>>
    {
        private readonly IScryfallApiClient _scryfallApiClient;

        public GetCardsFromSetQueryHandler( IScryfallApiClient scryfallApiClient)
        {
            _scryfallApiClient = scryfallApiClient;
        }

        public async Task<ICollection<Card>> HandleAsync(GetCardsFromSetQuery query)
        {
            var allCards = await _scryfallApiClient.GetCardsFromSetAsync(query.SetNameAs3Letter);
            return allCards
                .Where(c => !c.IsBasicLand() && c.collector_number <= query.MaxCardNo)
                .Select(c => c.ToModelCard())
                .ToList();
        }
    }
}
