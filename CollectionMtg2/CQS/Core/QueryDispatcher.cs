using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CollectionMtg2.CQS.Core
{
    public interface IQueryDispatcher
    {
        Task<R> DispatchAsync<T, R>(T query);
    }

    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<R> DispatchAsync<T, R>(T query)
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<T, R>>();
            return await handler.HandleAsync(query);
        }
    }
}
