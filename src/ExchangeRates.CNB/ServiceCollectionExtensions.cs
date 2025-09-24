using System.Net;
using ExchangeRates.CNB.Sources;
using ExchangeRates.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace ExchangeRates.CNB;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCnbExchangeRateProvider(this IServiceCollection services)
    {
        services.AddMemoryCache();

        // TODO: Move Client Configuration to Options/Configuration to not hardcode it there. 
        services.AddHttpClient("CNB", client =>
        {
            client.BaseAddress = new Uri("https://www.cnb.cz/");
            client.Timeout = TimeSpan.FromSeconds(10);
        })
            .AddResilienceHandler("CNB", builder =>
        {
            builder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(200),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = args =>
                    ValueTask.FromResult(
                        args.Outcome.Result is { StatusCode: >= HttpStatusCode.InternalServerError } ||
                        args.Outcome.Result?.StatusCode == (HttpStatusCode)429 ||
                        args.Outcome.Exception is HttpRequestException)
            });

            builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(30)
            });
        });
        
        services.AddTransient<CnbDailySource>();
        services.AddTransient<CnbMonthlySource>();

        
        services.AddKeyedSingleton<IExchangeRateStore>(CnbDailySource.SourceName, (sp, _) =>
        {
            var source = sp.GetRequiredService<CnbDailySource>();
            return new CachedExchangeRateStore(
                source,
                sp.GetRequiredService<IMemoryCache>(),
                ExpirationCalculators.Daily
            );
        });
        
        services.AddKeyedSingleton<IExchangeRateStore>(CnbMonthlySource.SourceName, (sp, _) =>
        {
            var source = sp.GetRequiredService<CnbMonthlySource>();
            return new CachedExchangeRateStore(
                source,
                sp.GetRequiredService<IMemoryCache>(),
                ExpirationCalculators.Monthly
            );
        });
        
        
        services.AddSingleton<IExchangeRateProvider, CnbExchangeRateProvider>();

        return services;
    }
}