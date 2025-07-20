using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Framework.Core.v1.Bases.Injections;
using Project.Framework.Core.v1.Bases.Models.CrossCutting;
using Project.Framework.Core.v1.Extensions;
using System.Reflection;

namespace Project.Framework.Core.v1.IoC
{
    public abstract class BaseBootstrapper<TAppSettings>(WebApplicationBuilder builder)
        where TAppSettings : AppsettingsConfigurations
    {
        protected IServiceCollection Services = builder.Services;
        protected static TAppSettings? Appsettings;

        public abstract void InjectDependencies();

        public static TBootstrapper CreateBootstrapper<TBootstrapper>(WebApplicationBuilder builder)
            where TBootstrapper : BaseBootstrapper<TAppSettings>
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var bootstrapper = typeof(TBootstrapper)!
                .GetConstructor(new[] { typeof(WebApplicationBuilder) })!
                .Invoke(new object[] { builder }) as TBootstrapper;

            var configurations = builder.Configuration.GetSection(typeof(TAppSettings).Name);
            bootstrapper!.Services.Configure<TAppSettings>(configurations);

            Appsettings = configurations.Get<TAppSettings>();

            if (Appsettings!.JwtConfiguration is not null)
            {
                bootstrapper.InjectAuthenticationSwagger();
                bootstrapper.ConfigureAuthentication();
            }

            bootstrapper.InjectHttpContextAccessor();

            return bootstrapper!;
        }

        protected void InjectMediatorFromAssembly(Type type)
        {
            Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(type.Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
        }

        protected void InjectAutoMapperFromAssembly(Assembly assembly)
        {
            Services.AddAutoMapper(opt =>
            {
                opt.AddMaps(assembly);
            });
        }

        protected void InjectRedis() => Services.InjectRedis(Appsettings);

        protected void InjectContext<TContext>() where TContext : DbContext => Services.InjectContext<TContext>(Appsettings);

        #region Private Methods

        private void ConfigureAuthentication() => Services.ConfigureAuthentication(Appsettings);

        private void InjectAuthenticationSwagger() => Services.InjectAuthenticationSwagger(Appsettings.SwaggerSettings);

        private void InjectHttpContextAccessor() => Services.AddHttpContextAccessor();

        #endregion Private Methods
    }
}