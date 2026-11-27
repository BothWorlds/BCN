using BCN.Client.Services;
using BCN.Server.Repository;
using BCN.Server.Services;
using BCN.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Oqtane.Infrastructure;
using Oqtane.Modules;
using Radzen;
using System;
using System.IO;

namespace BCN.Startup
{
    public class ServerStartup : IServerStartup
    {
        private IWebHostEnvironment _env;

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _env = env;
            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseStaticFiles();
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            if (_env?.IsDevelopment() ?? false)
            {
                AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
                {
                    Console.WriteLine("Reporting exception: " + e.Exception.Message);
                };
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddMemoryCache();
            services.AddControllers();
            services.AddRadzenComponents();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IUsersService, ServerUsersService>();
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<INotificationService, BCN.Server.Services.NotificationService>();
            services.AddDbContextFactory<MainContext>(opt => { }, ServiceLifetime.Transient);
            
            services
                .AddDataProtection()
                .SetApplicationName("BCN");
        }
    }
}
