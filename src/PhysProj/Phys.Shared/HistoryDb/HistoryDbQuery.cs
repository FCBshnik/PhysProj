using NodaTime;

namespace Phys.HistoryDb
{
    public record HistoryDbQuery
    {
        public Interval Interval { get; }

        public int Skip { get; }

        public int Limit { get; }

        public HistoryDbQuery(Interval interval, int skip, int limit)
        {
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip));
            if (limit < 0 || limit > 1000)
                throw new ArgumentOutOfRangeException(nameof(limit));

            Interval = interval;
            Skip = skip;
            Limit = limit;
        }
    }
}
