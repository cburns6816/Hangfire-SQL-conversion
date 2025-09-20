using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LOJIC.Orchestration.Data.Extensions
{
    public static class ConfigureDbContext
    {
        public static void AddCustomDbContext<T>(this IServiceCollection services, IConfiguration configuration) where T : DbContext
        {
            var connectionStringParameter = typeof(T).Name;
            var connString = Environment.GetEnvironmentVariable(connectionStringParameter);
            if (String.IsNullOrEmpty(connString))
                connString = configuration.GetConnectionString(connectionStringParameter);
            services.AddDbContext<T>(o => o.UseSqlServer(connString));
        }
    }
}
