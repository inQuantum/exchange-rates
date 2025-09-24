namespace ExchangeRates.CNB;

public static class ExpirationCalculators
{
    public static DateTimeOffset Daily(DateOnly date)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague");

        // TODO: Add proper jitter, to not hardcode in 14:35 everyday.
        var expiryLocal = date.AddDays(1).ToDateTime(new TimeOnly(14, 35));
        return TimeZoneInfo.ConvertTimeToUtc(expiryLocal, tz);
    }
    
    public static DateTimeOffset Monthly(DateOnly date)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague");

        var firstNextMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1);
        return TimeZoneInfo.ConvertTimeToUtc(firstNextMonth, tz);
    }
}