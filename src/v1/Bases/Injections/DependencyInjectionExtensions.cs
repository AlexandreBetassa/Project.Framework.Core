using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Store.Framework.Core.v1.Bases.Injections
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection InjectValidators(this IServiceCollection services, Type type)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssembly(type.Assembly);

            return services;
        }
    }
}
