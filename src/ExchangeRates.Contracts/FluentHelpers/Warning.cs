using FluentResults;

namespace ExchangeRates.Contracts.FluentHelpers;

public sealed class Warning : IReason
{
    #region Constructors

    /// <summary>
    /// Warning reason constructor
    /// </summary>
    /// <param name="message">Message of the reason.</param>
    /// <param name="metadata">Additional metadata.</param>
    public Warning(string message, Dictionary<string, object>? metadata = null)
    {
        Message = message;
        Metadata = metadata ?? [];
    }

    #endregion Constructors

    #region Properties

    public string Message { get; }
    
    public Dictionary<string, object> Metadata { get; }

    #endregion Properties
}