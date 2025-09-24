using ExchangeRates.CNB.Sources;
using ExchangeRates.Contracts;
using ExchangeRates.Contracts.Models;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExchangeRates.CNB;

/// <summary>
/// Represents a Czech National Bank exchange rate provider.
/// </summary>
public sealed class CnbExchangeRateProvider : IExchangeRateProvider
{
    #region Constructors

    /// <summary>
    /// Czech national bank exchange rate provider constructor.
    /// </summary>
    /// <param name="dailyStore">Cnb daily store.</param>
    /// <param name="monthlyStore">Cnb monthly store.</param>
    /// <param name="logger">Logger.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CnbExchangeRateProvider(
            [FromKeyedServices(CnbDailySource.SourceName)] IExchangeRateStore dailyStore,
            [FromKeyedServices(CnbMonthlySource.SourceName)] IExchangeRateStore monthlyStore,
            ILogger<CnbExchangeRateProvider> logger)
    {
        _dailyStore = dailyStore ?? throw new ArgumentNullException(nameof(dailyStore));
        _monthlyStore = monthlyStore ?? throw new ArgumentNullException(nameof(monthlyStore));
        _logger = logger;
    }

    #endregion Constructors

    #region Fields

    private readonly IExchangeRateStore _dailyStore;
    private readonly IExchangeRateStore _monthlyStore;
    private readonly ILogger<CnbExchangeRateProvider> _logger;

    #endregion Fields
   
    #region Methods

    #region Public Methods

    /// <inheritdoc/>
    public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(IEnumerable<Currency> currencies, CancellationToken ct = default)
    {
        var wanted = currencies.ToHashSet();

        if (wanted.Count == 0)
        {
            _logger.LogWarning("Requested empty list of currencies.");
            return [];
        }
        
        Func<ExchangeRate, bool> filter = r => wanted.Contains(r.TargetCurrency);

        var dailyTask = _dailyStore.GetRatesAsync(filter, ct);
        var monthlyTask = _monthlyStore.GetRatesAsync(filter, ct);

        await Task.WhenAll(dailyTask, monthlyTask);
        
        var map = new Dictionary<string, ExchangeRate>(StringComparer.OrdinalIgnoreCase);
        
        Merge(monthlyTask.Result, map);
        Merge(dailyTask.Result, map);
        
        return map.Values.ToArray();
    }

    #endregion Public Methods

    #region Private Methods

    private void Merge(Result<ExchangeRateResult> res, Dictionary<string, ExchangeRate> map)
    {
        if (!res.IsSuccess) 
            return;
        
        foreach (var r in res.Value.ExchangeRates) 
            map[r.TargetCurrency.Code] = r;
    }

    #endregion Private Methods

    #endregion Methods
}