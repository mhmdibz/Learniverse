using FluentValidation;
using Learniverse.API.Contracts.Requests.Identity;
using Learniverse.API.Contracts.Responses.Identity;
using Learniverse.Application.Features.Identity.Commands.ForgotPassword;
using Learniverse.Application.Features.Identity.Commands.ConfirmEmail;
using Learniverse.Application.Features.Identity.Commands.Login;
using Learniverse.Application.Features.Identity.Commands.Register;
using Learniverse.Application.Features.Identity.Commands.ResendEmailConfirmation;
using Learniverse.Application.Features.Identity.Commands.ResetPassword;
using Learniverse.Application.Features.Identity.Commands.RefreshToken;
using Learniverse.Application.Features.Identity.Commands.Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ApplicationValidationException =
    Learniverse.Application.Exceptions.ValidationException;
namespace Learniverse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName =
        "__Host-LearniverseRefreshToken";

    private readonly ISender _sender;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;

    public AuthController(
        ISender sender,
        IValidator<RegisterRequest> registerRequestValidator,
    IValidator<LoginRequest> loginRequestValidator)
    {
        _sender = sender;
        _registerRequestValidator = registerRequestValidator;
        _loginRequestValidator = loginRequestValidator;

    }

    [HttpPost("register")]
    [EnableRateLimiting("register")]
    public async Task<IActionResult> Register(
    RegisterRequest request,
    CancellationToken cancellationToken)
    {
        var validationResult =
            await _registerRequestValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ApplicationValidationException(validationResult.Errors);
        }

        var userId = await _sender.Send(
            new RegisterCommand(
                request.FullName,
                request.UserName,
                request.Email,
                request.Password),
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new RegisterResponse(userId));
    }
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(
    LoginRequest request,
    CancellationToken cancellationToken)
    {
        var validationResult =
            await _loginRequestValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
            throw new ApplicationValidationException(
                validationResult.Errors);

        var result = await _sender.Send(
        new LoginCommand(
            request.Identifier,
            request.Password),
        cancellationToken);

        Response.Cookies.Append(
            RefreshTokenCookieName,
            result.RefreshToken,
            CreateRefreshTokenCookieOptions(
                result.RefreshTokenExpiresAtUtc));

        Response.Headers.CacheControl = "no-store";

        return Ok(
            new LoginResponse(
                result.AccessToken,
                result.ExpiresAtUtc));
    }

    private static CookieOptions CreateRefreshTokenCookieOptions(
        DateTimeOffset expiresAtUtc)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = expiresAtUtc,
            IsEssential = true
        };
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("refresh")]
    public async Task<IActionResult> Refresh(
        CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";

        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            Response.Cookies.Delete(
                RefreshTokenCookieName,
                CreateRefreshTokenCookieOptions(DateTimeOffset.UnixEpoch));

            return Unauthorized();
        }

        var session = await _sender.Send(
            new RefreshTokenCommand(refreshToken),
            cancellationToken);

        if (session is null)
        {
            Response.Cookies.Delete(
                RefreshTokenCookieName,
                CreateRefreshTokenCookieOptions(DateTimeOffset.UnixEpoch));

            return Unauthorized();
        }

        Response.Cookies.Append(
            RefreshTokenCookieName,
            session.RefreshToken,
            CreateRefreshTokenCookieOptions(
                session.RefreshTokenExpiresAtUtc));

        return Ok(new LoginResponse(
            session.AccessToken,
            session.AccessTokenExpiresAtUtc));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [EnableRateLimiting("logout")]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new LogoutCommand(Request.Cookies[RefreshTokenCookieName]),
            cancellationToken);

        Response.Cookies.Delete(
            RefreshTokenCookieName,
            CreateRefreshTokenCookieOptions(DateTimeOffset.UnixEpoch));
        Response.Headers.CacheControl = "no-store";

        return NoContent();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EnableRateLimiting("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ForgotPasswordCommand(request.Email),
            cancellationToken);

        Response.Headers.CacheControl = "no-store";

        return Ok(new AuthMessageResponse(
            "If an account with that email exists, a reset link has been sent."));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EnableRateLimiting("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ResetPasswordCommand(
                request.Email,
                request.Token,
                request.NewPassword),
            cancellationToken);

        Response.Headers.CacheControl = "no-store";

        return Ok(new AuthMessageResponse(
            "Password has been reset successfully."));
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [EnableRateLimiting("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ConfirmEmailCommand(request.Email, request.Token),
            cancellationToken);

        Response.Headers.CacheControl = "no-store";

        return Ok(new AuthMessageResponse(
            "Email has been confirmed successfully."));
    }

    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    [EnableRateLimiting("resend-confirmation")]
    public async Task<IActionResult> ResendEmailConfirmation(
        ResendEmailConfirmationRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ResendEmailConfirmationCommand(request.Email),
            cancellationToken);

        Response.Headers.CacheControl = "no-store";

        return Ok(new AuthMessageResponse(
            "If an account with that email needs confirmation, a confirmation link has been sent."));
    }
}
