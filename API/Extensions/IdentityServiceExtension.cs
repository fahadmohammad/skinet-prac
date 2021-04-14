using System.Text;
using Core.Entities.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration config)
        {
            var builder = services.AddIdentityCore<AppUser>();
            builder = new Microsoft.AspNetCore.Identity.IdentityBuilder(builder.UserType,
                        typeof(IdentityRole), services);
            builder.AddRoles<IdentityRole>();
            builder.AddEntityFrameworkStores<AppIdentityDbContext>();
            builder.AddSignInManager<SignInManager<AppUser>>();
            builder.AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
                            options.TokenLifespan = System.TimeSpan.FromHours(2));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddGoogle(options =>
                {
                    var googleAuth = config.GetSection("Authentication:Google");

                    options.ClientId = googleAuth["ClientId"];
                    options.ClientSecret = googleAuth["ClientSecret"];
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                            (config["Token:Key"])),
                        ValidateIssuer = true,
                        ValidIssuer = config["Token:Issuer"],
                        ValidateAudience = false
                    };
                });

            return services;
        }

        private class TimeSpan
        {
        }
    }
}