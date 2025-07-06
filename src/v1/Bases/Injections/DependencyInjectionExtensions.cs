using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace Store.Framework.Core.v1.Bases.Injections
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
