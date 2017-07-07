using AuthService.Repository.Users;
using AuthService.Repository.Users.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthService
{
    public partial class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add DI support
            SetupDI(services);
            
            // Add framework services.
            services.AddMvc();
            
            // Add https features
            services.Configure<MvcOptions>(options =>
                options.Filters.Add(new RequireHttpsAttribute()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            var options = new RewriteOptions()
                .AddRedirectToHttps();
            app.UseRewriter(options);
            
            ConfigureAuth(app);

            app.UseMvc();
        }

        /// <summary>
        /// Setup dependency injection for the services
        /// </summary>
        /// <param name="services"></param>
        private void SetupDI(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
        }
    }
}