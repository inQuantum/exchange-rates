using ExchangeRates.Contracts.Models;
using FluentResults;

namespace ExchangeRates.Contracts;

public interface IExchangeRateSource
{
    #region Properties

    string Name { get; }
    
    Currency BaseCurrency { get; }

    #endregion Properties

    #region Methods
    
    /// <summary>
    /// Fetch exchange rates from source.
    /// </summary>
    /// <param name="date">Optional date to fetch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns></returns>
    Task<Result<ExchangeRateResult>> FetchFromSourceAsync(DateOnly? date = null, CancellationToken ct = default);

    #endregion Methods
}