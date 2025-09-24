using ExchangeRates.CNB.Options;
using ExchangeRates.Contracts;
using ExchangeRates.Contracts.FluentHelpers;
using ExchangeRates.Contracts.Models;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExchangeRates.CNB.Sources.Common;

public abstract class CnbSourceBase : IExchangeRateSource
{
    #region Constructors

    /// <summary>
    /// Cnb base source constructor.
    /// </summary>
    /// <param name="httpClientFactory">Http Client Factory.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="cnbParserOptions">Cnb parser options.</param>
    protected CnbSourceBase(IHttpClientFactory httpClientFactory, ILogger logger, IOptions<CnbParserOptions> cnbParserOptions)
    {
        _httpClient = httpClientFactory.CreateClient("CNB");
        _logger = logger;
        _cnbParserOptions = cnbParserOptions.Value;
    }

    #endregion Constructors

    #region Fields

    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly CnbParserOptions _cnbParserOptions;

    #endregion Fields

    #region Properties

    public abstract string Name { get; }
    
    public Currency BaseCurrency => new("CZK");
    
    protected abstract string BuildUrl(DateOnly date);
    
    #endregion Properties

    #region Methods

    #region Public Methods
    
    /// <inheritdoc/>
    public async Task<Result<ExchangeRateResult>> FetchFromSourceAsync(DateOnly? date = null, CancellationToken ct = default)
    {
        if (!date.HasValue)
        {
            _logger.LogError("Failed to fetch data for source: {Source}; Date should be provided", Name);
            return Result.Fail<ExchangeRateResult>("Date should be provided.");
        }
        
        try
        {
            // TODO: Think about correctness of GetStringAsync, probably change to any better way, to handle different cases.
            var content = await _httpClient.GetStringAsync(BuildUrl(date.Value), ct);
            var exchangeRatesResult = CnbParser.Parse(content, BaseCurrency, _cnbParserOptions);

            if (!exchangeRatesResult.IsSuccess)
            {
                _logger.LogError("Parsing failed for source: {Source}; With errors: {Errors}", Name, string.Join("; ", exchangeRatesResult.Errors));
                return Result.Fail<ExchangeRateResult>("Couldn't parse source data.");
            }

            if (exchangeRatesResult.Reasons.Any(x => x is Warning))
                _logger.LogWarning("Parsing is partially invalid for source {Source}. Warnings: {Warning}", Name, string.Join(";", exchangeRatesResult.Reasons.Where(x => x is Warning).Select(x => x.Message)));
            
            return exchangeRatesResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during source({Source}) data fetching: '{Message}'", Name, ex.Message);
            return Result.Fail<ExchangeRateResult>("Error during source data fetching.");
        }
    }

    #endregion Public Methods

    #endregion Methods
}