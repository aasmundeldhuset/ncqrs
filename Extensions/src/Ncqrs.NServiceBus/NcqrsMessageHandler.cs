using System;
using System.Threading;
using Ncqrs.Commanding.ServiceModel;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
    /// <summary>
    /// NServiceBus message handler for messages transporting Ncqrs commands.
    /// </summary>
    public class NcqrsMessageHandler : IHandleMessages<CommandMessage>
    {
        private log4net.ILog _log = log4net.LogManager.GetLogger(typeof (NcqrsMessageHandler));

        /// <summary>
        /// Command service which is injected by NServiceBus infrastructure.
        /// </summary>
        public ICommandService CommandService { get; set; }

        public void Handle(CommandMessage message)
        {
            _log.Warn("=== Message with type " + message.Payload.GetType() + " (cmd.id. " + message.Payload.CommandIdentifier + ") picked up by thread " + Thread.CurrentThread.Name + " ===");
            CommandService.Execute(message.Payload);
        }
    }
}