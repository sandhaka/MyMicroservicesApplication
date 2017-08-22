using System;
using System.Reflection;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Catalog.Application.Filters;
using Catalog.Application.Infrastructure;
using Catalog.Application.Infrastructure.Repositories;
using Catalog.Application.IntegrationEvents;
using EventBus;
using EventBus.Abstractions;
using EventBusAwsSns.Shared.IntegrationEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Catalog.Application
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
            // Otherwise take from the local configuration
            if (string.IsNullOrEmpty(connectionString))
                connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CatalogContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    opts =>
                    {
                        opts.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    });
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
            
            // Add services
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionHandlingFilter));
            });
            
            services.AddTransient<ICatalogRepository, CatalogRepository>(); /* Catalog repository */
            services.AddTransient<ISubscriptionsManager, SuscriptionManager>(); /* Subscription manager used by the EventBus */
            services.AddSingleton<IEventBus, EventBusAwsSns.EventBus>(); /* Adding EventBus as a singletone service */
           
            // Amazon services setup
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions()); /* Setup credentails and others options */
            services.AddAWSService<IAmazonSimpleNotificationService>(); /* Amazon SNS */
            services.AddAWSService<IAmazonSQS>(); /* Amazon SQS */
            services.AddSingleton<IConfiguration>(Configuration); /* Make project configuration available */
            
            // Add event handlers
            services.AddTransient<CatalogUpdateOnOrderStartedIntegrationEventHandler>();
            
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
            
            ConfigureEventBus(app);

            // Log actions
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ResponseLoggingMiddleware>();

            ConfigureAuth(app);

            app.UseMvc();

            new CatalogContextSeed().SeedAsync(app, loggerFactory).Wait();
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderStartedIntegrationEvent, CatalogUpdateOnOrderStartedIntegrationEventHandler>(() =>
                app.ApplicationServices.GetRequiredService<CatalogUpdateOnOrderStartedIntegrationEventHandler>());
        }
    }
}