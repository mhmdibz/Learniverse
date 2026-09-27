using Learniverse.Application.Common.Constants;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Identity;
using Learniverse.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Learniverse.Infrastructure.Authentication;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    SignInManager<ApplicationUser> signInManager,
    IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterResult>
    RegisterStudentAsync(
            string fullName,
            string userName,
            string email,
            string password,
            CancellationToken cancellationToken)
    {
        if (!await _roleManager.RoleExistsAsync(Roles.Student))
        {
            return RegisterResult.Failure(
    new[] { "Student role does not exist." });
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var transactionCompleted = false;

        try
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                FullName = fullName
            };

            var createResult =
                await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                transactionCompleted = true;

                var errors = createResult.Errors
                    .Select(error => error.Description)
                    .ToArray();

                return RegisterResult.Failure(errors);
            }

            var roleResult =
                await _userManager.AddToRoleAsync(user, Roles.Student);

            if (!roleResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                transactionCompleted = true;

                var errors = roleResult.Errors
                    .Select(error => error.Description)
                    .ToArray();

                return RegisterResult.Failure(errors);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            transactionCompleted = true;

            return RegisterResult.Success(user.Id);
        }
        catch
        {
            if (!transactionCompleted)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            }

            throw;
        }
    }
    public async Task<AuthenticationResult> AuthenticateAsync(
     string identifier,
     string password,
     CancellationToken cancellationToken)
    {
        ApplicationUser? user;

        if (identifier.Contains('@'))
        {
            user = await _userManager.FindByEmailAsync(identifier);
        }
        else
        {
            user = await _userManager.FindByNameAsync(identifier);
        }

        if (user is null)
        {
            return AuthenticationResult.Failure(
                AuthenticationFailureReason.InvalidCredentials);
        }

        var signInResult =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true);

        if (signInResult.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var authenticatedUser = new AuthenticatedUser(
                user.Id,
                roles.ToArray());

            return AuthenticationResult.Success(authenticatedUser);
        }

        if (signInResult.IsLockedOut)
        {
            return AuthenticationResult.Failure(
                AuthenticationFailureReason.LockedOut);
        }

        if (signInResult.IsNotAllowed)
        {
            return AuthenticationResult.Failure(
                AuthenticationFailureReason.NotAllowed);
        }

        return AuthenticationResult.Failure(
            AuthenticationFailureReason.InvalidCredentials);
    }
}