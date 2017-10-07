using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthService.Security
{
    /// <summary>
    /// Abstract class Token Middleware - Setting up the middleware
    /// </summary>
    public abstract class TokenMiddleware : ITokenMiddleware
    {
        /// <summary>
        /// Delegate to the next middleware
        /// </summary>
        protected readonly RequestDelegate Next;
        /// <summary>
        /// Token options
        /// </summary>
        protected readonly TokenOptions Options;
        /// <summary>
        /// Json serialization settings
        /// </summary>
        protected readonly JsonSerializerSettings SerializerSettings;

        /// <summary>
        /// Base ctor
        /// </summary>
        /// <param name="next">next middleware delegate</param>
        /// <param name="options">Token options</param>
        protected TokenMiddleware(RequestDelegate next, IOptions<TokenOptions> options)
        {
            Next = next;
            Options = options.Value;
            SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        
        /// <summary>
        /// Analyze a request - Return a new access token or delegate the request to the next middleware
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Return the next task</returns>
        public virtual Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(Options.Path, StringComparison.Ordinal))
            {
                return Next(context);
            }
           
            if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return context.Response.WriteAsync("Bad request.");
            }
            
            return GenerateTokenAsync(context);
        }
        
        /// <summary>
        /// Implementation specific
        /// </summary>
        /// <param name="httpContext">Http context</param>
        /// <returns>Task</returns>
        protected abstract Task GenerateTokenAsync(HttpContext httpContext); 
    }
}