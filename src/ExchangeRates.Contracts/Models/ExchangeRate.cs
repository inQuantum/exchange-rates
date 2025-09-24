namespace ExchangeRates.Contracts.Models;

/// <summary>
/// Represents an exchange rate between source and target currency.
/// </summary>
public record ExchangeRate
{
    #region Properties

    public required Currency SourceCurrency { get; init; }

    public required Currency TargetCurrency { get; init; }

    public decimal Value { get; init; }
    
    #endregion Properties

    #region Methods

    #region Public Methods

    public override string ToString() => $"{SourceCurrency}/{TargetCurrency} = {Value}";

    #endregion Public Methods

    #endregion Methods
}