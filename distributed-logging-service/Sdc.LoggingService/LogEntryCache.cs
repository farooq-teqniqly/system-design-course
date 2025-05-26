using Microsoft.Extensions.Options;

namespace Sdc.LoggingService
{
    public sealed class LogEntryCache
    {
        private readonly LinkedList<LogCacheEntry> _logEntries = new();
        private readonly LogEntryCacheOptions _options;

        public LogEntryCache(IOptions<LogEntryCacheOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);
            _options = options.Value;
        }

        public void AddEntry(LogCacheEntry cacheEntry)
        {
            if (_logEntries.Count == _options.MaxSize)
            {
                _logEntries.RemoveLast();
            }

            _logEntries.AddFirst(cacheEntry);
        }

        public List<LogCacheEntry> GetEntries(int limit) => _logEntries.Take(limit).ToList();
    }
}
