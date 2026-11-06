using System.Net;
using Learniverse.Application.Interfaces.Email;
using Learniverse.Application.Interfaces.Identity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learniverse.Application.Features.Identity.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IPasswordResetLinkBuilder _linkBuilder;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IIdentityService identityService,
        IPasswordResetLinkBuilder linkBuilder,
        IEmailSender emailSender,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _identityService = identityService;
        _linkBuilder = linkBuilder;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var token = await _identityService.GeneratePasswordResetTokenAsync(
            request.Email,
            cancellationToken);

        if (token is null)
        {
            return Unit.Value;
        }

        var resetLink = _linkBuilder.Build(request.Email, token);
        var safeLink = WebUtility.HtmlEncode(resetLink);
        var message = new EmailMessage(
            EmailPurpose.PasswordReset,
            request.Email,
            "Reset your Learniverse password",
            $"Use this link to reset your password: {resetLink}",
            $"<p>Use the link below to reset your Learniverse password.</p><p><a href=\"{safeLink}\">Reset password</a></p>");

        try
        {
            await _emailSender.SendAsync(message, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
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
