using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public static class StartupSeeding
{
    private static readonly (string Email, string Password, string Role)[] DemoUsers =
    {
        ("2251012106oanh@ou.edu.vn", "Passw0rd!", "Student"),
        ("2251012106oanh@ou.edu.vn",  "Passw0rd!", "Parent")
    };

    private static readonly string[] Roles = new[] { "Student", "Parent" };

    public static async Task EnsureSeedDataAsync(this IServiceProvider services, IWebHostEnvironment env)
    {
        // Only run in Development to avoid leaking demo accounts into prod
        if (!env.IsDevelopment()) return;

        using var scope = services.CreateScope();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure roles
        foreach (var r in Roles)
            if (!await roleMgr.RoleExistsAsync(r))
                await roleMgr.CreateAsync(new IdentityRole(r));

        // Ensure demo users
        foreach (var (email, pwd, role) in DemoUsers)
        {
            var u = await userMgr.FindByEmailAsync(email);
            if (u is null)
            {
                u = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var cr = await userMgr.CreateAsync(u, pwd);
                if (!cr.Succeeded)
                {
                    // log errors (replace with ILogger if you have it)
                    Console.WriteLine($"Failed to create {email}: {string.Join(",", cr.Errors.Select(e => e.Description))}");
                    continue;
                }
            }

            if (!await userMgr.IsInRoleAsync(u, role))
                await userMgr.AddToRoleAsync(u, role);
        }
    }
}
