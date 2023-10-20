using NodaTime;

namespace Phys.Shared.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime UnixSecondsToDateTime(long unixSeconds)
        {
            return Instant.FromUnixTimeSeconds(unixSeconds).ToDateTimeUtc();
        }
    }
}
