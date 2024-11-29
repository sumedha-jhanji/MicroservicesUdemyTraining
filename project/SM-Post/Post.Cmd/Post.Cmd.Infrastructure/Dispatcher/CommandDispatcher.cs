using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatcher
{
    public class CommandDispatcher : ICommandDispatcher// Concrete Mediatr
    {
        private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new(); // Type = type of command handler
        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if(_handlers.ContainsKey(typeof(T))){
                throw new IndexOutOfRangeException("You cannot register the same command handler twice!");
            }
            _handlers.Add(typeof(T), x => handler((T)x));// x is base command and T is a concrete command object type
        }

        // will actual dispatch the command object to registerd command handler.
        public async Task SendAsync(BaseCommand command)
        {
            if (_handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task> handler)){
                await handler(command);
            }
            else
            {
                throw new ArgumentNullException(nameof(handler),"No command handler was registered");
            }
        }
    }
}