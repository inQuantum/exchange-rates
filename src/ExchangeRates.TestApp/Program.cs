using ExchangeRates.CNB;
using ExchangeRates.Contracts;
using ExchangeRates.Contracts.Models;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();

services.AddCnbExchangeRateProvider();

var provider = services.BuildServiceProvider();
var cnb = provider.GetRequiredService<IExchangeRateProvider>();

var currencies = new[]
{
    new Currency("USD"),
    new Currency("EUR"),
    new Currency("CZK"),
    new Currency("JPY"),
    new Currency("KES"),
    new Currency("RUB"),
    new Currency("THB"),
    new Currency("TRY"),
    new Currency("XYZ")
};

try
{
    while (true)
    {
        Console.WriteLine("Calculate?");
        var input = Console.ReadLine();
        
        if(string.IsNullOrWhiteSpace(input))
            break;
        
        var rates = (await cnb.GetExchangeRatesAsync(currencies)).ToList();

        Console.WriteLine($"Successfully retrieved {rates.Count} exchange rates:");
        foreach (var rate in rates)
        {
            Console.WriteLine(rate.ToString());
        }
    }
}
catch (Exception e)
{
    Console.WriteLine($"Could not retrieve exchange rates: '{e.Message}'.");
}

