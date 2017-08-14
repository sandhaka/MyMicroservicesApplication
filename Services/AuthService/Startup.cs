using AuthService.Repository.Users;
using AuthService.DbModels;
using System;
using System.Reflection;
using AuthService.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
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
            // Take connection string from environment varible by default
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            // Otherwise take from the local configuration (service testing)
            if (string.IsNullOrEmpty(connectionString))
                connectionString = Configuration.GetConnectionString("DefaultConnection");
            
            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    opts =>
                    {
                        opts.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    });
            });
            
            services.AddTransient<IUserRepository, UserRepository>();

            // Add cors and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials() );
            });
            
            // Add framework services.
            services.AddMvc(opts =>
            {
                // TODO
            });
            
            // Add https features
            services.Configure<MvcOptions>(options =>
                options.Filters.Add(new RequireHttpsAttribute()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Trace);
            
            var options = new RewriteOptions()
                .AddRedirectToHttps();
            app.UseRewriter(options);
            
            app.UseCors("CorsPolicy");
            
            // Log actions
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ResponseLoggingMiddleware>();
            
            ConfigureAuth(app);
            
            app.UseMvc();
            
            new UserDbContextSeed().SeedAsync(app, loggerFactory).Wait();
        }
    }
}