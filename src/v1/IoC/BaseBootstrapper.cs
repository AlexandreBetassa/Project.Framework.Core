using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Framework.Core.v1.Bases.Injections;
using Store.Framework.Core.v1.Bases.Models.CrossCutting;
using Store.Framework.Core.v1.Extensions;
using System.Reflection;

namespace Store.Framework.Core.v1.IoC
{
    public abstract class BaseBootstrapper(WebApplicationBuilder builder)
    {
        protected IServiceCollection Services = builder.Services;
        protected static AppsettingsConfigurations Appsettings;
        private readonly string _mediatorLicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzgzMjk2MDAwIiwiaWF0IjoiMTc1MTgyNzQ3MiIsImFjY291bnRfaWQiOiIwMTk3ZTEwYWRjNzU3YWNhYWIzMWMyNmQzODE4ZTBlYyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxanpnZ3ZyNmY2em1nejExZTYzMWMwNGVlIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.hQZ7sapBzGr78oJ63lUJc3hQeoHLT2fqZs88C3sRCHp8OiM-qzysw7bYhr4GNQIIeVUJGUmDShZfWMdSWzsisU773csUIIhvoo7nSWMoof1efnM0KL1SnSGY_RudVruOUEJydQ8jVq97Lw6RpIbUNvSS2BP1yL-ZBL8lZD4OAHeU44thhhxsY0LVyljUDmn3JjCZjPYenJ4sz44Xfa1HCVykAJzbGi3gE2L81wQdVfO_NJuARNhpKksHRlgBKIbuuz1ruLM1GXruCAZWiR8hZAbfgkpISdVMOcwypUGpnaCYn-PtTFBZdPkpKRaUAlTLyPv5X1ckwY5cnZlZ4KQXDA";

        public abstract void InjectDependencies();

        public static TBootstrapper CreateBootstrapper<TBootstrapper, TAppSettings>(WebApplicationBuilder builder)
            where TBootstrapper : BaseBootstrapper
            where TAppSettings : AppsettingsConfigurations
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var bootstrapper = typeof(TBootstrapper)!
                .GetConstructor(new[] { typeof(WebApplicationBuilder) })!
                .Invoke(new object[] { builder }) as TBootstrapper;

            Appsettings = bootstrapper!.Services.AddConfigurations<TAppSettings>(builder);

            if (Appsettings.JwtConfiguration is not null)
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
                cfg.LicenseKey = _mediatorLicenseKey;
                cfg.RegisterServicesFromAssembly(type.Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
        }

        protected void InjectAutoMapperFromAssembly(Assembly assembly)
        {
            Services.AddAutoMapper(opt =>
            {
                opt.LicenseKey = _mediatorLicenseKey;
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