using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Storage.Caching;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage.SQL
{
    public class CacheBasedSnapshotStore : ISnapshotStore
    {
        private readonly ICacheProvider _cache;

        public CacheBasedSnapshotStore(ICacheProvider cache)
        {
            _cache = cache;
        }

        public void SaveShapshot(ISnapshot source)
        {
            _cache[source.EventSourceId.ToString()] = source; //TODO: !!! SYNCHRONIZATION ISSUES HERE? (And should we suffix the key with the version?) !!!
        }

        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            return (ISnapshot) _cache[eventSourceId.ToString()];
        }
    }
}
