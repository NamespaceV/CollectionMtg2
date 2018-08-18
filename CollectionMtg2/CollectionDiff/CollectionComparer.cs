namespace CollectionMtg2.CollectionDiff
{
    using CollectionMtg2.DomainModel;
    using System.IO;
    using System.Threading.Tasks;

    class CollectionComparer
    {
        public async Task<CardCollection> GetMissingCards(CardCollection collection, string setListPath, bool playsets)
        {
            var missing = new CardCollection();
            var goalCount = playsets ? 4 : 1; 
            using (TextReader fileReader = File.OpenText(setListPath))
            {
                while(fileReader.Peek() >= 0)
                {
                    var cardName = fileReader.ReadLine();
                    if (string.IsNullOrWhiteSpace(cardName))
                    {
                        continue;
                    }
                    int has = collection.GetCount(cardName);
                    if (has < goalCount)
                    {
                        missing.AddCard(cardName, goalCount - has);
                    }
                }
            }
            return missing;
        }
    }
}
