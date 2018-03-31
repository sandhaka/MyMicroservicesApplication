using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using EventBus;
using EventBus.Abstractions;
using FluentValidation;
using FluentValidation.AspNetCore;
using IntegrationEventsContext;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Orders.Application.Commands;
using Orders.Application.Filters;
using Orders.Application.Services;
using Orders.Application.Validation;
using Orders.Domain.AggregatesModel.BuyerAggregate;
using Orders.Domain.AggregatesModel.OrderAggregate;
using Orders.Infrastructure;
using Orders.Infrastructure.Repositories;
using StackExchange.Redis;

namespace Orders.Application
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
            
            services.AddEntityFrameworkSqlServer().AddDbContext<OrdersContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    opts =>
                    {
                        opts.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        opts.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);                        
                    });
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
            
            // Setup MVC 
            services.AddMvc( options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionHandlingFilter));
            })
                .AddFluentValidation(); /* Using Fluent validation */

            // Add cors and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials() );
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

            // Adding services to DI container
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIntegrationEventsRespository, IntegrationEventsRespository>();
            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddScoped<IBuyerRepository, BuyerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();    /* Orders respository */
            services.AddTransient<ISubscriptionsManager, SuscriptionManager>(); /* Subscription manager used by the EventBus */
            services.AddSingleton<IEventBus, EventBusAwsSns.EventBus>(); /* Adding EventBus as a singletone service */
           
            // Amazon services setup
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions()); /* Setup credentails and others options */
            services.AddAWSService<IAmazonSimpleNotificationService>(); /* Amazon SNS */
            services.AddAWSService<IAmazonSQS>(); /* Amazon SQS */
            
            services.AddSingleton<IConfiguration>(Configuration); /* Make project configuration available */
            
            // Register all integration event handlers for this microservice
            RegisterIntegrationEventHandlers(services);
            
            services.AddOptions();

            // MediatR config
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            
            // Command validation:
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipeline<,>)); /* Validation pipline (MediatR) */
            services.AddTransient<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>(); /* Adding fluent validator to DI container */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Trace);
            
            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            var options = new RewriteOptions()
                .AddRedirectToHttps();
            app.UseRewriter(options);
            
            ConfigureEventBus(app);

            // Log actions
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ResponseLoggingMiddleware>();

            app.UseMvc();
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            // Init the event bus
            app.ApplicationServices.GetRequiredService<IEventBus>().Init();
        }

        private void RegisterIntegrationEventHandlers(IServiceCollection services)
        {
            
        }
    }
}