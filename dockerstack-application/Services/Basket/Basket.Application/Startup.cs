using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Basket.Application.Filters;
using Basket.Application.Infrastructure.Repositories;
using Basket.Application.IntegrationEvents;
using Basket.Application.Services;
using EventBus;
using EventBus.Abstractions;
using EventBusAwsSns.Shared.IntegrationEvents;
using IntegrationEventsContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Basket.Application
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
            // Add cors and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials() );
            });
            
            // Setup Token validation
            var prvtKeyPassphrase = File.ReadAllText("certificate/dev.boltjwt.passphrase");
            var publicKey = new X509Certificate2("certificate/dev.boltjwt.pfx", prvtKeyPassphrase).GetRSAPublicKey();
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Add the access_token as a claim, as we may actually need it
                        var accessToken = context.SecurityToken as JwtSecurityToken;
                        if (accessToken != null)
                        {
                            ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                            if (identity != null)
                            {
                                identity.AddClaim(new Claim("access_token", accessToken.RawData));
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
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
            services.AddMvc(options =>
            {
                // TODO: add global exception handling
            });

            // Take Redis connection string from environment varible by default
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
            // Otherwise take from the local configuration
            if (string.IsNullOrEmpty(redisConnectionString))
                redisConnectionString = Configuration.GetConnectionString("Redis");
            
            services.AddSingleton(sp =>
            {
                var ips = Dns.GetHostAddressesAsync(redisConnectionString).Result;
                return ConnectionMultiplexer.Connect(ips.First().ToString());
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIntegrationEventsRespository, IntegrationEventsRespository>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IBasketRepository, BasketRepository>();
            services.AddTransient<ISubscriptionsManager, SuscriptionManager>(); /* Subscription manager used by the EventBus */
            services.AddSingleton<IEventBus, EventBusAwsSns.EventBus>(); /* Adding EventBus as a singletone service */

            services.AddTransient<DeleteBasketOnOrderStartedIntegrationEventHandler>();
           
            // Amazon services setup
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions()); /* Setup credentails and others options */
            services.AddAWSService<IAmazonSimpleNotificationService>(); /* Amazon SNS */
            services.AddAWSService<IAmazonSQS>(); /* Amazon SQS */
            services.AddSingleton<IConfiguration>(Configuration); /* Make project configuration available */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseAuthentication();

            app.UseCors("CorsPolicy");
            
            ConfigureEventBus(app);
            
            // Log actions
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ResponseLoggingMiddleware>();
            
            app.UseMvc();
        }
        
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            
            eventBus.Init();

            eventBus.Subscribe<OrderStartedIntegrationEvent, DeleteBasketOnOrderStartedIntegrationEventHandler>(() =>
                app.ApplicationServices.GetRequiredService<DeleteBasketOnOrderStartedIntegrationEventHandler>());
        }
    }
}