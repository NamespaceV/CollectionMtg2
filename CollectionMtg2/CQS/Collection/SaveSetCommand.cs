using CollectionMtg2.CQS.Core;
using CollectionMtg2.DomainModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CollectionMtg2.CQS.Collection
{
    public class SaveSetCommand
    {
        public string Path { get; set; }
        public ICollection<Card> CardsToSave { get; set; }
    }

    public class SaveSetCommandHandler : ICommandHandler<SaveSetCommand>
    {
        public async Task HandleAsync(SaveSetCommand command)
        {
            using (var writer = new StreamWriter(File.Open(command.Path, FileMode.OpenOrCreate)))
            {
                foreach (var card in command.CardsToSave)
                {
                    await writer.WriteLineAsync(card.CardName);
                }
            }
        }
    }
}
