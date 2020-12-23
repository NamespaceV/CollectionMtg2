using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CollectionMtg2.CQS.Core
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync<T>(T command);
    }

    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<T>(T command)
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<T>>();
            await handler.HandleAsync(command);
        }
    }
}
