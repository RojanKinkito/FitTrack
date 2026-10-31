using Microsoft.AspNetCore.Identity;

namespace FitTrack.Data;

public static class IdentitySeeder
{
    private static readonly string[] Roles = ["Admin", "Trainer"];

    public static async Task SeedAsync(IServiceProvider services, IWebHostEnvironment environment)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Could not create the {role} role.");
                }
            }
        }

        // Optional: set these on the host (e.g. Render) so an admin login survives restarts
        // of a disposable database.
        var adminEmail = Environment.GetEnvironmentVariable("FITTRACK_ADMIN_EMAIL");
        var adminPassword = Environment.GetEnvironmentVariable("FITTRACK_ADMIN_PASSWORD");

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(admin, adminPassword);
            if (!userResult.Succeeded)
            {
                var errors = string.Join("; ", userResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not create the development admin account: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException("Could not assign the Admin role to the development account.");
            }
        }
    }
}
