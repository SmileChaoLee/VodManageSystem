using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var dbContext = services.GetRequiredService<KtvSystemDBContext>();
                    if (dbContext.Song.Any())
                    {
                        logger.LogInformation("Program.cs --> Song has records.");
                    }
                    else
                    {
                        throw new Exception("Song Table does not have any records.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogInformation(ex, "Program.cs --> DbContext error.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://127.0.0.1:5000");
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
                        options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
