using ExchangeRates.CNB.Options;
using ExchangeRates.CNB.Sources.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExchangeRates.CNB.Sources;

/// <summary>
/// Represents a Cnb Daily Source to retrieve exchange rates.
/// </summary>
public sealed class CnbDailySource : CnbSourceBase
{
    #region Constructors

    /// <summary>
    /// Cnb daily source constructor.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    /// <param name="logger">Logger</param>
    /// <param name="cnbParserOptions">Cnb parser options to apply.</param>
    public CnbDailySource(
        IHttpClientFactory httpClientFactory,
        ILogger<CnbDailySource> logger,
        IOptions<CnbParserOptions> cnbParserOptions)
        : base(httpClientFactory, logger, cnbParserOptions)
    { }

    #endregion Constructors

    #region Properties

    public const string SourceName = "CNB.Daily";

    public override string Name => SourceName;

    #endregion Properties

    #region Methods

    #region Protected Methods

    protected override string BuildUrl(DateOnly date) => $"{Constants.DAILY_BASE_URL}?date={date:dd.MM.yyyy}";
    
    #endregion Protected Methods

    #endregion Methods
}
