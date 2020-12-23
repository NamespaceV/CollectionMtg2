using System.Threading.Tasks;

namespace CollectionMtg2.CQS.Core
{
    internal interface ICommandHandler<T>
    {
        Task HandleAsync(T command);
    }
}