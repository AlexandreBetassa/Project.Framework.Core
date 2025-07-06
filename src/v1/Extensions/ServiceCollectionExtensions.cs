using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Store.Framework.Core.v1.Bases.Models.CrossCutting;
using Store.Framework.Core.v1.Enums;
using System.Text;
namespace Store.Framework.Core.v1.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void InjectAuthenticationSwagger(this IServiceCollection services, string title, string version, string description)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = version, Description = description });

                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization Header - Utilizado com Bearer Authentication.\r\n\r\n" +
                        "Digite seu token no campo abaixo.\r\n\r\n" +
                        "Exemplo (informar apenas o token): '12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                 });
            });
        }

        public static TSettings AddConfigurations<TSettings>(this IServiceCollection services, WebApplicationBuilder builder)
            where TSettings : AppsettingsConfigurations
        {
            var configurations = builder.Configuration.GetSection(typeof(TSettings).Name);
            services.Configure<TSettings>(nameof(TSettings), configurations);
            services.AddTransient(sp => sp.GetRequiredService<IOptions<TSettings>>().Value);

            return configurations!.Get<TSettings>()!;
        }

        public static void InjectRedis(this IServiceCollection services, AppsettingsConfigurations appsettings) =>
            services.AddStackExchangeRedisCache(options => options.Configuration = appsettings.RedisConfiguration.Server);

        public static void ConfigureAuthentication(this IServiceCollection services, AppsettingsConfigurations appsettings)
        {
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtOptions =>
            {
                jwtOptions.RequireHttpsMetadata = false;
                jwtOptions.SaveToken = false;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appsettings.JwtConfiguration!.SecretJwtKey)),
                    ValidIssuer = appsettings.JwtConfiguration.Issuer,
                    ValidAudience = appsettings.JwtConfiguration.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true
                };
            });

            services.AddAuthorizationBuilder()
              .AddPolicy(nameof(AccessPoliciesEnum.Write),
                policy => policy.RequireRole(appsettings.JwtConfiguration!.WriteRoles))
             .AddPolicy(nameof(AccessPoliciesEnum.Read),
                policy => policy.RequireRole(appsettings.JwtConfiguration!.ReadRoles));
        }

        public static void InjectContext<TContext>(this IServiceCollection services, AppsettingsConfigurations appsettings)
            where TContext : DbContext =>
            services.AddDbContext<TContext>(options => options.UseSqlServer(appsettings!.Database));
    }
}
