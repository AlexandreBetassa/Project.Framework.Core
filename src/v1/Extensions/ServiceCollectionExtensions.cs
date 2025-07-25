﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project.Framework.Core.v1.Bases.Models.CrossCutting;
using Project.Framework.Core.v1.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
namespace Project.Framework.Core.v1.Extensions
{
    public static class ServiceCollectionExtensions
    {
        internal static void InjectAuthenticationSwagger(this IServiceCollection services, SwaggerSettings swaggerSettings)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = swaggerSettings.Title,
                    Version = swaggerSettings.Version,
                    Description = swaggerSettings.Description
                });

                AddSwaggerDocumentation(c);

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

        private static void AddSwaggerDocumentation(SwaggerGenOptions c)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var xmlFiles = Directory.GetFiles(baseDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (var xmlFile in xmlFiles)
            {
                c.IncludeXmlComments(xmlFile);
            }
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
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(appsettings!.Database));
    }
}
