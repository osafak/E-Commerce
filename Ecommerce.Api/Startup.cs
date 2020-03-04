using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Config;
using Data.Entity;
using Ecommerce.Api.Middleware;
using FluentValidation.AspNetCore;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Pipeline;
using Service.Query.Basket;
using SimpleInjector;

namespace Ecommerce.Api
{
    public class Startup
    {
        public static readonly Container Container = new Container { };
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var assemblies = GetAssemblies().ToArray();
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(assemblies));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Ecommerce API V1", Description = "Ecommerce API V1" });
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddMediatR(assemblies);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ApiPipelineBehavior<,>));
            services.AddScoped(typeof(IRequestPreProcessor<>), typeof(ApiRequestPreProcessor<>));
            services.AddScoped(typeof(IRequestPostProcessor<,>), typeof(ApiRequestPostProcessor<,>));
            var config = Configuration.GetSection("ConnectionString").Get<DbConfig>();
            services.AddSingleton(config);
            services.AddScoped(typeof(Entities), typeof(Entities));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMiddleware<ExceptionMiddleware>();
        
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API V1");
            });
            Mapping.Mapping.Configure();
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(BasketSaveQuery).GetTypeInfo().Assembly;
            yield return typeof(Startup).GetTypeInfo().Assembly;
        }
    }
}
