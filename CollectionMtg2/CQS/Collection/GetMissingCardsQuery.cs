namespace CollectionMtg2.CQS.Collection
{
    using CollectionMtg2.CQS.Core;
    using CollectionMtg2.DomainModel;
    using System.IO;
    using System.Threading.Tasks;

    public class GetMissingCardsQuery
    {
        public CardCollection CollectedCards { get; set; }
        public string WantedCardsListPath { get; set; }
        public bool WantPlaysets { get; set; }
    }

    public class GetMissingCardsQueryHandler : IQueryHandler<GetMissingCardsQuery, CardCollection>
    {
        public Task<CardCollection> HandleAsync(GetMissingCardsQuery query)
        {
            var missing = new CardCollection();
            var goalCount = query.WantPlaysets ? 4 : 1;
            using (TextReader fileReader = File.OpenText(query.WantedCardsListPath))
            {
                while (fileReader.Peek() >= 0)
                {
                    var cardName = fileReader.ReadLine();
                    if (string.IsNullOrWhiteSpace(cardName))
                    {
                        continue;
                    }
                    int has = query.CollectedCards.GetCount(cardName);
                    if (has < goalCount)
                    {
                        missing.AddCard(cardName, goalCount - has);
                    }
                }
            }
            return Task.FromResult(missing);
        }
    }
}
