using System.Security.AccessControl;
using Hangfire;
using LOJIC.Hangfire.Extensions;
using LOJIC.Hangfire.Services;
using LOJIC.Hangfire.Services.Heartbeat;
using LOJIC.Hangfire.Services.Notifications;
using LOJIC.Orchestration.Data;
using LOJIC.Orchestration.Data.Extensions;
using LOJIC.Orchestration.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LOJIC.Orchestration.Hosts
{
    public class WebHost
    {
        public IConfiguration Configuration { get; }
        public WebHost(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfireConfiguration(Configuration);
            services.AddWorkerPoolConfiguration(Configuration);
            services.AddCustomDbContext<OrchestrationMetadataContext>(Configuration);
            
            //setup Dependency Injection for all Heartbeat services
            services.AddDerivedServices<BaseHeartbeatService>(ServiceLifetime.Transient);
            services.AddTransient<EmailService>();
            services.AddHostedService<ServiceHost>();
            services.AddHealthChecks();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OrchestrationMetadataContext dataContext)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            dataContext.Database.Migrate();
            app.UseHttpsRedirection();
            app.UseHangfireDashboard();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHangfireDashboard();
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapGet("/debug-config", ctx =>
                {
                    var config = (Configuration as IConfigurationRoot).GetDebugView();
                    return ctx.Response.WriteAsync(config);
                });
            });
        }
    }
}
