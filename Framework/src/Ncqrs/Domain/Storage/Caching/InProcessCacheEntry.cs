// From palantir: src/palantir-common/Cache/InProcessCacheEntry.cs

using System;

namespace Ncqrs.Domain.Storage.Caching
{
    public class InProcessCacheEntry
    {
        public InProcessCacheEntry()
        {
            Timeout = DateTime.Now.AddMinutes(24);
        }

        public InProcessCacheEntry(object value)
            : this()
        {
            if (value == null) throw new ArgumentException("Cache entries cannot be null", "value");
            Value = value;
        }

        public InProcessCacheEntry(object value, TimeSpan timeOut)
            : this(value)
        {
            Timeout = DateTime.Now.Add(timeOut.TotalDays < 36500 ? timeOut : new TimeSpan(36500, 0, 0, 0));
        }

        public DateTime Timeout { get; set; }
        public object Value { get; private set; }
    }
}
