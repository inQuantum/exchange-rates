namespace ExchangeRates.CNB.Options;

public record CnbParserOptions
{
    public int HeaderLines { get; init; } = 2;

    public int AmountColumn { get; init; } = 2;
    
    public int CodeColumn { get; init; } = 3;
    
    public int RateColumn { get; init; } = 4;
    
    public char Separator { get; init; } = '|';
}