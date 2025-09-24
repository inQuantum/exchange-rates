using ExchangeRates.CNB.Options;
using ExchangeRates.CNB.Sources.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExchangeRates.CNB.Sources;

/// <summary>
/// Represents a Cnb Monthly Source to retrieve exchange rates.
/// </summary>
public sealed class CnbMonthlySource : CnbSourceBase
{    
    #region Constructors

    /// <summary>
    /// Cnb monthly source constructor.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    /// <param name="logger">Logger</param>
    /// <param name="cnbParserOptions">Cnb parser options to apply.</param>
    public CnbMonthlySource(
        IHttpClientFactory httpClientFactory,
        ILogger<CnbMonthlySource> logger,
        IOptions<CnbParserOptions> cnbParserOptions)
        : base(httpClientFactory, logger, cnbParserOptions)
    { }

    #endregion Constructors

    #region Properties
    
    public const string SourceName = "CNB.Monthly";

    public override string Name => SourceName;
    
    #endregion Properties

    #region Methods

    #region Protected Methods

    protected override string BuildUrl(DateOnly date)
    {
        var prevMonth = date.AddMonths(-1);
        return $"{Constants.MONTHLY_BASE_URL}?year={prevMonth.Year}&month={prevMonth.Month}";
    }
    
    #endregion Protected Methods

    #endregion Methods
}