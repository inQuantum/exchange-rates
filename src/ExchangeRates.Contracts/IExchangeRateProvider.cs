using ExchangeRates.Contracts.Models;

namespace ExchangeRates.Contracts;

public interface IExchangeRateProvider
{
    #region Methods

    #region Public Methods

    /// <summary>
    /// Retrieve exchange rates for target currencies.
    /// </summary>
    /// <param name="currencies">Target currencies to take rates.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Array of requested exchange rates.</returns>
    Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(IEnumerable<Currency> currencies, CancellationToken ct = default);

    #endregion Public Methods
    
    #endregion Methods
}