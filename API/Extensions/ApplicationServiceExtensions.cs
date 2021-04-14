using System.Linq;
using API.Errors;
using API.ThirdPartyAuth;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using API.MailService;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IGoogleAuth, GoogleAuth>();
            services.Configure<MailSettings>(config.GetSection("MailSettings"));
            services.AddScoped<IMailService, API.MailService.MailService>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count() > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);

                };
            });

            return services;
        }
    }
}