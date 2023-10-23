namespace Phys.Shared.Utils
{
    public class ProgressWithInterval<T> : Progress<T>
    {
        private static DateTime lastReportedAt = DateTime.MinValue.ToUniversalTime();

        public TimeSpan MinReportInterval { get; }

        public ProgressWithInterval(Action<T> handler, TimeSpan minReportInterval) : base(handler)
        {
            MinReportInterval = minReportInterval;
        }

        protected override void OnReport(T value)
        {
            var now = DateTime.UtcNow;
            if (now - lastReportedAt > MinReportInterval)
            {
                base.OnReport(value);
                lastReportedAt = now;
            }
        }
    }
}
