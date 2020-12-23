using CollectionMtg2.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace CollectionMtg2.CQS.Scryfall.Helper
{
    public static class ScryfallCardExtensions
    {
        public static ScryfallCard FindMatching(
            this ICollection<ScryfallCard> cardMetadataSet,
            Card cardToMatch)
        {
                
            return cardMetadataSet.FirstOrDefault(scryData => scryData.name == cardToMatch.CardName);
        }

        public static Card ToModelCard(this ScryfallCard cardMetadata)
        {
            return new Card()
            {
                CardName = cardMetadata.name,
                DisplayImage = cardMetadata?.image_uris?.png
            };
        }
    }
}
