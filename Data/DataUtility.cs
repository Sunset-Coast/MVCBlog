using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Reflection.Metadata;
using TechnicalBlog.Models;

namespace TechnicalBlog.Data
{
    public static class DataUtility
    {
        private const string _adminRole = "Administrator";
        private const string _modRole = "Moderator";
        private const string _adminEmail = "hellokitty321@Mailinator.com";
        private const string _modEmail = "moderator@Malinator.com";

        public static string GetConnectionString(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");


            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAysnc(IServiceProvider svcProvider)
        {
            //Obtaining the necessary services based on the IServiceProvider parameter
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();

            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();

            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Align the database by checking Migrations
            await dbContextSvc.Database.MigrateAsync();

            // Seed Roles

            await SeedRolesAsync(roleManagerSvc);


            // Seed Default Users

            await SeedUsersAsync(dbContextSvc, configurationSvc, userManagerSvc);

        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if(!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));

            }
            if (!await roleManager.RoleExistsAsync(_modRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_modRole));

            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            //confirm not in the database
            if (!context.Users.Any(u => u.Email == _adminEmail))
            {
                BlogUser adminUser = new()
                {
                    Email = _adminEmail,
                    UserName = _adminEmail,
                    FirstName = "Jacqueline",
                    LastName = "Thomas",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, configuration["AdminPassword"] ?? Environment.GetEnvironmentVariable("AdminPassword"));
                await userManager.AddToRoleAsync(adminUser, _adminRole);

            }
            if (!context.Users.Any(u => u.Email == _modEmail))
            {
                BlogUser modUser = new()
                {
                    Email = _modEmail,
                    UserName = _modEmail,
                    FirstName = "Blog",
                    LastName = "Moderator",
                    EmailConfirmed = true

                };

                await userManager.CreateAsync(modUser, configuration["ModeratorPassword"] ?? Environment.GetEnvironmentVariable("ModeratorPassword"));
                await userManager.AddToRoleAsync(modUser, _modRole);

            }
        }


    }

}
