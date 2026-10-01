using Learniverse.Application.Common.Constants;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Infrastructure.Authentication;
using Learniverse.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Authentication;

public static class BootstrapAdminSeeder
{
    public static async Task SeedAsync(
     UserManager<ApplicationUser> userManager,
     IOptions<BootstrapAdminOptions> options,
     IUnitOfWork unitOfWork,
     CancellationToken cancellationToken)
    {
        var adminUsers = await userManager.GetUsersInRoleAsync(Roles.Admin);

        if (adminUsers.Count > 0)
            return;

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var bootstrapAdmin = new ApplicationUser
            {
                UserName = options.Value.Email,
                Email = options.Value.Email,
                EmailConfirmed = true,
                FullName = "Administrator"
            };

            var createResult = await userManager.CreateAsync(
                bootstrapAdmin,
                options.Value.Password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create bootstrap admin: {errors}");
            }

            var roleResult = await userManager.AddToRoleAsync(
                bootstrapAdmin,
                Roles.Admin);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to assign Admin role: {errors}");
            }

            await unitOfWork.CommitTransactionAsync(
                cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(
                cancellationToken);

            throw;
        }
    }
}