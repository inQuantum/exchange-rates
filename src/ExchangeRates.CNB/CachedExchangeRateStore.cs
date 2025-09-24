using ExchangeRates.Contracts.Models;
using ExchangeRates.Contracts;
using FluentResults;
using Microsoft.Extensions.Caching.Memory;

namespace ExchangeRates.CNB;

public class CachedExchangeRateStore : IExchangeRateStore
{
    #region Constructors

    /// <summary>
    /// Cached exchange rate store constructor.
    /// </summary>
    /// <param name="source">Source where retrieve data.</param>
    /// <param name="cache">Memory cache</param>
    /// <param name="expiryCalculator">Expiry calculator policy.</param>
    public CachedExchangeRateStore(
        IExchangeRateSource source,
        IMemoryCache cache,
        Func<DateOnly, DateTimeOffset> expiryCalculator)
    {
        _source = source;
        _cache = cache;
        _expiryCalculator = expiryCalculator;
        _keyPrefix = $"rates:{source.Name}";
    }

    #endregion Constructors

    #region Fields

    private readonly IExchangeRateSource _source;
    private readonly IMemoryCache _cache;
    private readonly Func<DateOnly, DateTimeOffset> _expiryCalculator;
    private readonly string _keyPrefix;

    #endregion Fields

    #region Methods

    #region Public Methods

    public Task<Result<ExchangeRateResult>> GetRatesAsync(CancellationToken ct = default) => GetRatesAsync(_ => true, ct);

    public async Task<Result<ExchangeRateResult>> GetRatesAsync(Func<ExchangeRate, bool> filter, CancellationToken ct = default)
    {
        // TODO: Think about: Store in cache previous data. And fallback to prev time if failed to fetch.
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        if (_cache.TryGetValue(_keyPrefix, out ExchangeRateResult? cached) && cached is not null)
            return Result.Ok(new ExchangeRateResult
            {
                ValidOn = cached.ValidOn,
                ExchangeRates = [.. cached.ExchangeRates.Where(filter)]
            });

        var result = await _source.FetchFromSourceAsync(date, ct);
        var effective = result.Value.ValidOn;

        if (!result.IsSuccess) 
            return result;
        
        _cache.Set(_keyPrefix, result.Value, new MemoryCacheEntryOptions 
        { 
            AbsoluteExpiration = _expiryCalculator(DateOnly.FromDateTime(effective)) 
        });

        return Result.Ok(new ExchangeRateResult
        {
            ValidOn = result.Value.ValidOn,
            ExchangeRates = [.. result.Value.ExchangeRates.Where(filter)]
        });
    }

    #endregion Public Methods

    #endregion Methods
}