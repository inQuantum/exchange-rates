namespace ExchangeRates.Contracts.Models;

/// <summary>
/// Represents currency in exchange rate.
/// </summary>
public sealed class Currency : IEquatable<Currency>
{
    #region Constructors

    public Currency(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Code = code.ToUpperInvariant();
    }

    #endregion Constructors
    
    #region Properties

    /// <summary>
    /// Three-letter ISO 4217 code of the currency.
    /// </summary>
    public string Code { get; }

    #endregion Properties

    #region Methods

    #region Public Static Methods

    public static bool operator ==(Currency? left, Currency? right) => Equals(left, right);
    
    public static bool operator !=(Currency? left, Currency? right) => !Equals(left, right);

    #endregion Public Static Methods
    
    #region Public Methods

    public bool Equals(Currency? other) => other is not null && Code == other.Code;
    
    public override string ToString() => Code;
    
    public override bool Equals(object? obj) => Equals(obj as Currency);

    public override int GetHashCode() => Code.GetHashCode();

    #endregion Public Methods
    
    #endregion Methods
}