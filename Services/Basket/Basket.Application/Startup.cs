using System;
using System.Linq;
using System.Net;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Basket.Application.Filters;
using EventBus;
using EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var ips = Dns.GetHostAddressesAsync(redisConnectionString).Result;
                return ConnectionMultiplexer.Connect(ips.First().ToString());
            });
            
            services.AddTransient<ISubscriptionsManager, SuscriptionManager>(); /* Subscription manager used by the EventBus */
            services.AddSingleton<IEventBus, EventBusAwsSns.EventBus>(); /* Adding EventBus as a singletone service */
           
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
        }
        
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            // TODO
        }
    }
}