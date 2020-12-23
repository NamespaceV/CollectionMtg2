using System.Threading.Tasks;

namespace CollectionMtg2.CQS.Core
{
    internal interface IQueryHandler<T, R>
    {
        Task<R> HandleAsync(T query);
    }
}