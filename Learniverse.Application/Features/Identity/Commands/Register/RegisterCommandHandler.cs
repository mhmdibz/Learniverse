using System.Net;
using FluentValidation.Results;
using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Email;
using Learniverse.Application.Interfaces.Identity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learniverse.Application.Features.Identity.Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, string>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailConfirmationLinkBuilder _linkBuilder;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IIdentityService identityService,
        IEmailConfirmationLinkBuilder linkBuilder,
        IEmailSender emailSender,
        ILogger<RegisterCommandHandler> logger)
    {
        _identityService = identityService;
        _linkBuilder = linkBuilder;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<string> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterStudentAsync(
            request.FullName,
            request.UserName,
            request.Email,
            request.Password,
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new ValidationException(
                result.Errors.Select(error =>
                    new ValidationFailure("Identity", error)));
        }

        var token = await _identityService
            .GenerateEmailConfirmationTokenAsync(
                request.Email,
                cancellationToken);

        if (token is not null)
        {
            var confirmationLink = _linkBuilder.Build(
                request.Email,
                token);

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
        }

        return result.UserId!;
    }
}
