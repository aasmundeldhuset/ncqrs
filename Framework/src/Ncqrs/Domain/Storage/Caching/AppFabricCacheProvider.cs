// From palantir: src/palantir-common/Cache/AppFabricCacheProvider.cs

using System;
using Microsoft.ApplicationServer.Caching;

namespace Ncqrs.Domain.Storage.Caching
{
    public class AppFabricCacheProvider : ICacheProvider
    {
        private static DataCacheFactory _factory;
        private static DataCache _cache;

        private static DataCache GetCache()
        {
            if (_cache != null) return _cache;

            var configuration = new DataCacheFactoryConfiguration();
            _factory = new DataCacheFactory(configuration);
            _cache = _factory.GetCache("default");

            return _cache;
        }

        public void Add(string key, object value)
        {
            var cache = GetCache();
            cache.Put(key, value, new TimeSpan(0, 24, 0));
        }

        public void Add(string key, object value, TimeSpan timeout)
        {
            var cache = GetCache();
            cache.Put(key, value, timeout);
        }

        public object Get(string key)
        {
            var cache = GetCache();
            return cache.Get(key);
        }

        public object this[string key]
        {
            get { return Get(key); }
            set
            {
                var cache = GetCache();
                cache.Put(key, value, new TimeSpan(0, 24, 0));
            }
        }

        public bool Remove(string key)
        {
            var cache = GetCache();
            return cache.Remove(key);
        }
    }
}
