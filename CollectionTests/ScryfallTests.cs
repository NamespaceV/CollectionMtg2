using CollectionMtg2.CQS.Scryfall;
using FluentAssertions;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace CollectionTests
{
    [Ignore("Trigger only manually, do not call Scryfall API without reason.")]
    public class ScryfallTests
    {
        [Test]
        public async Task GetWarOfTheSpark()
        {
            var client = new ScryfallApiClient(new HttpClient());
            var cards = await client.GetCardsFromSetAsync("WAR");
            cards.Should().HaveCount(265);
            cards.Should().Contain(c => c.name == "Karn, the Great Creator");
            cards.Should().Contain(c => c.name == "Oath of Kaya");
            cards.Should().Contain(c => c.name == "Mobilized District");
            cards.Should().Contain(c => c.name == "Forest");
        }
    }
}