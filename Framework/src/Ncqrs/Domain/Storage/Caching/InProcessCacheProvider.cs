// From palantir: src/palantir-common/Cache/InProcessCacheProvider.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs.Domain.Storage.Caching
{
    public class InProcessCacheProvider : ICacheProvider
    {
        private static long hits;

        private static InProcessCacheProvider _instance = new InProcessCacheProvider();
        private static Dictionary<string, InProcessCacheEntry> _cache;

        private InProcessCacheProvider()
        {
            _cache = new Dictionary<string, InProcessCacheEntry>();
        }

        public static InProcessCacheProvider Instance()
        {
            return _instance;
        }

        private static Dictionary<string, InProcessCacheEntry> GetCache()
        {
            return _cache ?? (_cache = new Dictionary<string, InProcessCacheEntry>());
        }

        public void Add(string key, object value)
        {
            var cache = GetCache();
            cache.Add(key, new InProcessCacheEntry(value));
        }

        public void Add(string key, object value, TimeSpan timeout)
        {
            var cache = GetCache();
            cache.Add(key, new InProcessCacheEntry(value, timeout));
        }

        public object Get(string key)
        {
            var cache = GetCache();
            hits++;
            RemoveExpiredCacheObjects();
            if (!cache.ContainsKey(key)) return null;

            var entry = cache[key];
            if (entry.Timeout.Year < 2999)
                entry.Timeout = DateTime.Now.AddMinutes(24); // Sliding Door Time To Live for Cache object

            return entry.Value;
        }

        public object this[string key]
        {
            get { return Get(key); }
            set
            {
                var cache = GetCache();
                cache[key] = new InProcessCacheEntry(value);
            }
        }

        public bool Remove(string key)
        {
            var cache = GetCache();
            return cache.Remove(key);
        }

        private void RemoveExpiredCacheObjects()
        {
            if (hits / 10000 > 0)
                _cache
                    .Where(entry => entry.Value.Timeout < DateTime.Now).ToList()
                    .ForEach(entry => _cache.Remove(entry.Key));
        }
    }
}
