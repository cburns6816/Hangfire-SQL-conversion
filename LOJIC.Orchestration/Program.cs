using System;
using LOJIC.Orchestration.Hosts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LOJIC.Orchestration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => {
                    config.AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true);
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<WebHost>())
                .UseWindowsService();
    }
}