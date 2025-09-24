using System.Collections.Immutable;

namespace ExchangeRates.Contracts.Models;

public record ExchangeRateResult
{
    public DateTime ValidOn { get; init; }
    
    public ImmutableArray<ExchangeRate> ExchangeRates { get; init; } = [];
}