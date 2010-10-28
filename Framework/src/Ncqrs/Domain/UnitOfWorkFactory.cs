using System;
using Ncqrs.Domain.Storage;
using Ncqrs.Domain.Storage.Caching;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage.SQL;

namespace Ncqrs.Domain
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWorkContext CreateUnitOfWork()
        {
            if(UnitOfWork.Current != null) throw new InvalidOperationException("There is already a unit of work created for this context.");

            var store = NcqrsEnvironment.Get<IEventStore>();
            var bus = NcqrsEnvironment.Get<IEventBus>();

            var repository = new DomainRepository(store, bus, new CacheBasedSnapshotStore(new AppFabricCacheProvider()));
            return new UnitOfWork(repository);
        }
    }
}
