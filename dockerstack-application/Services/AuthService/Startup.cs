using AuthService.Repository.Users;
using AuthService.DbModels;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using AuthService.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
            
            services.AddEntityFrameworkSqlServer().AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    opts =>
                    {
                        opts.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        opts.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });
            
            services.AddScoped<IUserRepository, UserRepository>();

            var certName = Environment.GetEnvironmentVariable("CERT_NAME");
            var certPwdName = Environment.GetEnvironmentVariable("CERT_PWD_NAME");

            var certPath = File.Exists(certName)
                ? certName
                : "/run/secrets/cert";

            var certPwdPath = File.Exists(certPwdName)
                ? certPwdName
                : "/run/secrets/cert_pwd";

            // Setup Token validation
            var prvtKeyPassphrase = File.ReadAllText(certPwdPath);
            var publicKey = new X509Certificate2(certPath, prvtKeyPassphrase).GetRSAPublicKey();
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(publicKey),
                    ValidateIssuer = true,
                    ValidIssuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                    ValidateAudience = true,
                    ValidAudience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            
            // Add framework services.
            services.AddMvc(opts =>
            {
                // TODO
            });

            // Add cors and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials() );
            });
            
            // Add https features
            services.Configure<MvcOptions>(options =>
                options.Filters.Add(new RequireHttpsAttribute()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider services)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Trace);
            
            app.UseCors("CorsPolicy");

            var options = new RewriteOptions()
                .AddRedirectToHttps();
            app.UseRewriter(options);
            
            // Log actions
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ResponseLoggingMiddleware>();
            
            ConfigureTokenProviderMiddleware(app, services);
            
            app.UseMvc();
        }
    }
}