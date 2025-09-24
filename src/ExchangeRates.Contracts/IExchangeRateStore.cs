using FluentResults;
using ExchangeRates.Contracts.Models;
    
namespace ExchangeRates.Contracts;

public interface IExchangeRateStore
{
    /// <summary>
    /// Retrieve all exchange rates from store.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns></returns>
    Task<Result<ExchangeRateResult>> GetRatesAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Retrieve exchange rates from store with applied filter.
    /// </summary>
    /// <param name="filter">Filter to apply.</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns></returns>
    Task<Result<ExchangeRateResult>> GetRatesAsync(Func<ExchangeRate, bool> filter, CancellationToken ct = default);
}

