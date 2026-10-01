using Learniverse.Application.Common.Constants;
using Learniverse.Infrastructure.Authentication;
using Learniverse.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Authentication;

public static class BootstrapAdminSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        IOptions<BootstrapAdminOptions> options)
    {
        var adminUsers = await userManager.GetUsersInRoleAsync(Roles.Admin);

        if (adminUsers.Count > 0)
            return;

        var bootstrapAdmin = new ApplicationUser
        {
            UserName = options.Value.Email,
            Email = options.Value.Email,
            EmailConfirmed = true
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
            "Admin");

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                roleResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Failed to assign Admin role: {errors}");
        }
    }
}