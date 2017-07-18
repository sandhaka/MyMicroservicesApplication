using System;
using System.Reflection;
using Amazon.SimpleNotificationService;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orders.Domain.AggregatesModel.OrderAggregate;
using Orders.Infrastructure;
using Orders.Infrastructure.Repositories;

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
            
            services.AddDbContext<OrdersContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    opts =>
                    {
                        opts.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    });
            });
            
            // Add framework services.
            services.AddMvc();

            // Dependency injection
            services.AddTransient<IOrderRepository, OrderRepository>();    /* Orders respository */
            //services.AddSingleton<IEventBus, >();    /* Interface for the event bus */ 
            
            // Amazon setup
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSimpleNotificationService>();

            services.AddOptions();

            // MediatR config
            services.AddMediatR(typeof(Startup));
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
    }
}