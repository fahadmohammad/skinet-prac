using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace API.Extensions 
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title="Skinet API",
                    Version="v1"
                });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Auth Bearer Scheme",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer",securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement{{securitySchema,
                    new []{"Bearer"}}};

                c.AddSecurityRequirement(securityRequirement);

            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json","SkiNet API v1");
            });

            return app;
        }
    }
}