using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace Store.Framework.Core.Bases.v1.Injections
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection InjectValidators(this IServiceCollection services, Assembly assembly)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
