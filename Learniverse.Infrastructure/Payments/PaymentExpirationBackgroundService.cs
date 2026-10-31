using Learniverse.Application.Features.Payments.Commands.ExpirePendingPayments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Learniverse.Infrastructure.Payments;

public sealed class PaymentExpirationBackgroundService : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentExpirationBackgroundService> _logger;

    public PaymentExpirationBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentExpirationBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(CheckInterval);

        do
        {
            await ExpirePendingPaymentsAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ExpirePendingPaymentsAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var expiredCount = await sender.Send(
                new ExpirePendingPaymentsCommand(),
                cancellationToken);

            if (expiredCount > 0)
            {
                _logger.LogInformation(
                    "Expired {ExpiredPaymentCount} pending payments.",
                    expiredCount);
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An error occurred while expiring pending payments.");
        }
    }
}
