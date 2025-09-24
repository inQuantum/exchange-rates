using System.Globalization;
using ExchangeRates.CNB.Options;
using ExchangeRates.Contracts.FluentHelpers;
using ExchangeRates.Contracts.Models;
using FluentResults;

namespace ExchangeRates.CNB;

public static class CnbParser
{
    private static readonly char[] Separator = new[] { '\r', '\n' };

    public static Result<ExchangeRateResult> Parse(string content, Currency baseCurrency, CnbParserOptions opts)
    {
        var lines = content.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        
        if (lines.Length <= opts.HeaderLines)
            return Result.Fail<ExchangeRateResult>($"File had only {lines.Length} lines, expected at least {opts.HeaderLines + 1}.");

        var list = new List<ExchangeRate>(lines.Length - opts.HeaderLines);
        var reasons = new List<IReason>();

        for (var i = opts.HeaderLines; i < lines.Length; i++)
        {
            var parts = lines[i].Split(opts.Separator);

            if (parts.Length <= opts.RateColumn)
            {
                reasons.Add(new Warning($"Line {i} skipped: not enough columns."));
                continue;
            }

            if (!int.TryParse(parts[opts.AmountColumn], NumberStyles.Integer, CultureInfo.InvariantCulture, out var amount) || amount <= 0)
            {
                reasons.Add(new Warning($"Line {i} skipped: invalid amount '{parts[opts.AmountColumn]}'."));
                continue;
            }

            if (!decimal.TryParse(parts[opts.RateColumn], NumberStyles.Number, CultureInfo.InvariantCulture, out var rateCzk) || rateCzk <= 0)
            {
                reasons.Add(new Warning($"Line {i} skipped: invalid rate '{parts[opts.RateColumn]}'."));
                continue;
            }

            var code = parts[opts.CodeColumn].Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(code))
            {
                reasons.Add(new Warning($"Line {i} skipped: empty currency code."));
                continue;
            } 

            var perUnit = rateCzk / amount;

            list.Add(new ExchangeRate
            {
                SourceCurrency = baseCurrency,
                TargetCurrency = new Currency(code),
                Value = perUnit
            });
        }
        
        return Result.Ok(new ExchangeRateResult
        {
            ExchangeRates = [.. list],
            ValidOn = DateTime.Now // TODO: Parse valid date from source, cus exist Weekends and Holidays.
        }).WithReasons(reasons);
    }
}
