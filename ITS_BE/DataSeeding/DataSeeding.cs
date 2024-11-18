using ITS_BE.Data;
using ITS_BE.Enumerations;
using ITS_BE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITS_BE.DataSeeding
{
    public class DataSeeding
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();

            if (context != null)
            {
                try
                {
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                    }
                    await InitialRoles(scope.ServiceProvider, context);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private static async Task InitialRoles(IServiceProvider serviceProvider, MyDbContext context)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            foreach (string role in Enum.GetNames(typeof(RolesEnum)))
            {
                if (!context.Roles.Any(r => r.Name == role))
                {
                    await roleManager.CreateAsync(new Role
                    {
                        Name = role,
                        NormalizedName = role.ToUpper(),
                    });
                }
            }
            await InitialUsers(serviceProvider, context);
        }

        private static async Task InitialUsers(IServiceProvider serviceProvider, MyDbContext context)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var adminEmail = "hoab2005755@student.ctu.edu.vn";
            var admin = new User
            {
                FullName = "Nhựt Hòa",
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                UserName = adminEmail,
                NormalizedUserName = adminEmail.ToUpper(),
                PhoneNumber = "0944990152",
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var employeeEmail = "lethinhachan18@gmail.com";
            var employee = new User
            {
                FullName = "Chân Chân",
                Email = employeeEmail,
                NormalizedEmail = employeeEmail.ToUpper(),
                UserName = employeeEmail,
                NormalizedUserName = employeeEmail.ToUpper(),
                PhoneNumber = "0901089182",
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var stockerEmail = "minhnhat012340@gmail.com";
            var stocker = new User
            {
                FullName = "Minh Nhật",
                Email = stockerEmail,
                NormalizedEmail = stockerEmail.ToUpper(),
                UserName = stockerEmail,
                NormalizedUserName = stockerEmail.ToUpper(),
                PhoneNumber = "0358103707",
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var statistEmail = "lengoc14082002@gmail.com";
            var statist = new User
            {
                FullName = "Hồng Ngọc",
                Email = statistEmail,
                NormalizedEmail = statistEmail.ToUpper(),
                UserName = statistEmail,
                NormalizedUserName = statistEmail.ToUpper(),
                PhoneNumber = "0946633248",
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            if (!context.Users.Any(u => u.UserName == admin.UserName))
            {
                var result = await userManager.CreateAsync(admin, "Hoa123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, RolesEnum.Admin.ToString());
                }
            }
            if (!context.Users.Any(u => u.UserName == employee.UserName))
            {
                var result = await userManager.CreateAsync(employee, "Hoa123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employee, RolesEnum.Employee.ToString());
                }
            }
            if (!context.Users.Any(u => u.UserName == stocker.UserName))
            {
                var result = await userManager.CreateAsync(stocker, "Hoa123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(stocker, RolesEnum.Stocker.ToString());
                }
            }
            if (!context.Users.Any(u => u.UserName == statist.UserName))
            {
                var result = await userManager.CreateAsync(statist, "Hoa123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(statist, RolesEnum.Statist.ToString());
                }
            }

        }

    }
}
