using System.Net;
using Learniverse.Application.Interfaces.Email;
using Learniverse.Application.Interfaces.Identity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learniverse.Application.Features.Identity.Commands.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationCommandHandler
    : IRequestHandler<ResendEmailConfirmationCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailConfirmationLinkBuilder _linkBuilder;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ResendEmailConfirmationCommandHandler> _logger;

    public ResendEmailConfirmationCommandHandler(
        IIdentityService identityService,
        IEmailConfirmationLinkBuilder linkBuilder,
        IEmailSender emailSender,
        ILogger<ResendEmailConfirmationCommandHandler> logger)
    {
        _identityService = identityService;
        _linkBuilder = linkBuilder;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        ResendEmailConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        var token = await _identityService
            .GenerateEmailConfirmationTokenAsync(
                request.Email,
                cancellationToken);

        if (token is null)
        {
            return Unit.Value;
        }

        var confirmationLink = _linkBuilder.Build(request.Email, token);
        var safeLink = WebUtility.HtmlEncode(confirmationLink);
        var message = new EmailMessage(
            EmailPurpose.EmailConfirmation,
            request.Email,
            "Confirm your Learniverse email",
            $"Use this link to confirm your email: {confirmationLink}",
            $"<p>Confirm your Learniverse email by clicking the link below.</p>" +
            $"<p><a href=\"{safeLink}\">Confirm email</a></p>");

        try
        {
            await _emailSender.SendAsync(message, cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                "Failed to send {EmailPurpose} email. Failure type: {FailureType}.",
                message.Purpose,
                exception.GetType().Name);
        }

        return Unit.Value;
    }
}
