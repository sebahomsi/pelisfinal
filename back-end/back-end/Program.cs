using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeliculasAPI.Entidades;

namespace PeliculasAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                  
                    await context.Database.MigrateAsync();

                    AddGeneros(context);

                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager);

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                    throw;
                }
            }

            await host.RunAsync();
        }

        private static void AddGeneros(ApplicationDbContext context)
        {
            if (context.Generos.Any())
            {
                return;
            }

            context.AddRange(new List<Genero>
            {
                new Genero() { Nombre = "Terror" },
                new Genero() { Nombre = "Comedia" },
                new Genero() { Nombre = "Drama" },
                new Genero() { Nombre = "Ciencia Ficcion" },
                new Genero() { Nombre = "Aventura" },
            });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<IdentityUser> userManager)
        {
            var user = new IdentityUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com"
            };

            if (userManager.Users.All(u => u.UserName != user.UserName))
            {
                await userManager.CreateAsync(user, "admin123");
            }
        }
    }
}
