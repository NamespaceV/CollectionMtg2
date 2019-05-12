namespace CollectionMtg2.DomainModel
{
    using System.Collections.Generic;
    using System.Linq;

    public class CardCollection
    {
        public class Position
        {
            public Card CardType;
            public int CardCount;
            public string DisplayName => CardCount + " " + CardType.CardName;
            public string DisplayImage => CardType.DisplayImage;
        }
        public List<Position> cardPositions = new List<Position>();

        public void AddCard(string name, int count)
        {
            var position = new Position
            {
                CardType = new Card() {
                    CardName = name
                },
                CardCount = count
            };

            cardPositions.Add(position);
        }

        public int GetCount(string cardName)
        {
            return cardPositions
                .Where(cp => cp.CardType.CardName == cardName)
                .Sum(cp => cp.CardCount);
        }
    }
}
