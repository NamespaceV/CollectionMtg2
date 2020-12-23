namespace CollectionMtg2.CQS.Scryfall
{
    public class ScryfallCard
    {
        public string name;
        public int collector_number;
        public string type_line;
        public ScryfallImageUriSet image_uris;

        public class ScryfallImageUriSet
        {
            public string png;
        }

        public bool IsBasicLand()
        {
            return type_line.StartsWith("Basic Land —");
        }
    }
}
