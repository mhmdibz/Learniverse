using Microsoft.AspNetCore.Identity;

namespace Learniverse.Persistence.Identity;

public static class IdentitySeeder
{
    private static readonly string[] Roles =
    {
        "Student",
        "Instructor",
        "Admin"
    };

    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles)
        {
            if (await roleManager.RoleExistsAsync(role))
                continue;

            var result = await roleManager.CreateAsync(
                new IdentityRole(role));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create role '{role}': {errors}");
            }
        }
    }
}