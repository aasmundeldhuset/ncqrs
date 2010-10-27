using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
    /// <summary>
    /// A <see cref="IEventBus"/> implementation using NServiceBus. Forwards all published
    /// events via NServiceBus. Does NOT support registering local event handlers using
    /// <see cref="RegisterHandler{TEvent}"/>. All events passed to <see cref="Publish(System.Collections.Generic.IEnumerable{Ncqrs.Eventing.IEvent})"/>
    /// method are send using of NServiceBus transport level message.
    /// </summary>
    public class NsbEventBus : IEventBus
    {
        private static ILog _log = LogManager.GetLogger(typeof (NsbEventBus));
        private const int QueueFullWaitInSeconds = 60;

        public void Publish(IEvent eventMessage)
        {
            while (true)
            {
                try
                {
                    Bus.Publish(CreateEventMessage(eventMessage));
                    return;
                }
                catch (MessageQueueException e)
                {
                    if (e.MessageQueueErrorCode != MessageQueueErrorCode.InsufficientResources)
                        throw;
                    _log.Error("Queue full; waiting for " + QueueFullWaitInSeconds + " seconds");
                    Thread.Sleep(QueueFullWaitInSeconds * 1000);
                    _log.Error("(" + QueueFullWaitInSeconds + " seconds has passed since queue was full; continuing)");
                }
            }
        }

        public void Publish(IEnumerable<IEvent> eventMessages)
        {
            while (true)
            {
                try
                {
                    Bus.Publish(eventMessages.Select(CreateEventMessage).ToArray());
                    return;
                }
                catch (MessageQueueException e)
                {
                    if (e.MessageQueueErrorCode != MessageQueueErrorCode.InsufficientResources)
                        throw;
                    _log.Error("Queue full; waiting for " + QueueFullWaitInSeconds + " seconds");
                    Thread.Sleep(QueueFullWaitInSeconds * 1000);
                    _log.Error("(" + QueueFullWaitInSeconds + " seconds has passed since queue was full; continuing)");
                }
            }
        }

        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            throw new NotSupportedException("Registering local event handlers with NsbEventBus is not supported yet.");
        }

        private static IBus Bus
        {
            get { return NcqrsEnvironment.Get<IBus>(); }
        }

        private static IMessage CreateEventMessage(IEvent payload)
        {
            Type factoryType =
               typeof(EventMessageFactory<>).MakeGenericType(payload.GetType());
            var factory =
               (IEventMessageFactory)Activator.CreateInstance(factoryType);
            return factory.CreateEventMessage(payload);
        }

        public interface IEventMessageFactory
        {
            IMessage CreateEventMessage(IEvent payload);
        }

        private class EventMessageFactory<T> : IEventMessageFactory where T : IEvent
        {
            IMessage IEventMessageFactory.CreateEventMessage(IEvent payload)
            {
                return new EventMessage<T>
                          {
                              Payload = (T)payload
                          };
            }
        }
    }
}
