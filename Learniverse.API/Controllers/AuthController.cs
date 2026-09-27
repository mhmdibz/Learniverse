using FluentValidation;
using Learniverse.API.Contracts.Requests.Identity;
using Learniverse.Application.Features.Identity.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ApplicationValidationException =
    Learniverse.Application.Exceptions.ValidationException;
using Learniverse.API.Contracts.Responses.Identity;
using Learniverse.Application.Features.Identity.Commands.Login;
namespace Learniverse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
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

        Response.Headers.CacheControl = "no-store";

        return Ok(
            new LoginResponse(
                result.AccessToken,
                result.ExpiresAtUtc));
    }
}