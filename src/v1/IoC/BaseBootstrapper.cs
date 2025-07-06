using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Framework.Core.v1.Bases.Models.CrossCutting;
using Store.Framework.Core.v1.Extensions;
using System.Reflection;

namespace Store.Framework.Core.v1.IoC
{
    public abstract class BaseBootstrapper
    {
        protected IServiceCollection Services;
        protected static AppsettingsConfigurations Appsettings;

        protected BaseBootstrapper(WebApplicationBuilder builder)
        {
            Services = builder.Services;
        }

        public static TBootstrapper CreateBootstrapper<TBootstrapper, TAppSettings>(WebApplicationBuilder builder)
            where TBootstrapper : BaseBootstrapper
            where TAppSettings : AppsettingsConfigurations
        {
            var bootstrapper = typeof(TBootstrapper)
                .GetConstructor(new[] { typeof(WebApplicationBuilder) })?
                .Invoke(new object[] { builder }) as TBootstrapper;

            Appsettings = bootstrapper!.Services.AddConfigurations<TAppSettings>(builder);

            return bootstrapper!;
        }

        protected void InjectAuthenticationSwagger(string title, string version = "", string description = "") =>
            Services.InjectAuthenticationSwagger(title: title, version: version, description: description);

        protected void InjectMediatorFromAssembly(Type type) =>
            Services.AddMediatR(new MediatRServiceConfiguration().RegisterServicesFromAssemblyContaining(type));

        protected void InjectAutoMapperFromAssembly(Assembly assembly) =>
            Services.AddAutoMapper(opt => opt.AddMaps(assembly));

        protected void InjectHttpContextAccessor() => Services.AddHttpContextAccessor();

        protected void ConfigureAuthentication() => Services.ConfigureAuthentication(Appsettings);

        protected void InjectRedis() => Services.InjectRedis(Appsettings);

        protected void InjectContext<TContext>() where TContext : DbContext => Services.InjectContext<TContext>(Appsettings);
    }
}