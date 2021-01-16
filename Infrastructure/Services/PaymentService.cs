using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
            _configuration = configuration;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var basket = await GetAndVerifyCustomerBasket(basketId);

            if(basket == null) return null;
            
            var shippingPrice = await GetDeliveryPrice(basket.DeliveryMethodId);

            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long) basket.Items.Sum(item => item.Quantity * (item.Price * 100))
                        + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>{"card"}
                };

                paymentIntent = await paymentIntentService.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                     Amount = (long) basket.Items.Sum(item => item.Quantity * (item.Price * 100))
                        + (long)(shippingPrice * 100)
                };

                await paymentIntentService.UpdateAsync(basket.PaymentIntentId,options);
            }

            await _basketRepository.UpdateBasketAsync(basket);

            return basket;            
        }

        private async Task<decimal> GetDeliveryPrice(int ? deliveryMethodId)
        {
            var shippingPrice = 0m;

            if(deliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                    .GetByIdAsync((int)deliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }

            return shippingPrice;
        }

        private async Task<CustomerBasket> GetAndVerifyCustomerBasket(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);           

            foreach(var item in basket?.Items)
            {
                var productItem = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.Id);
                
                if(item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            return basket;

        }

        public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
           var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
           var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

           if(order == null) return null;

           order.Status = OrderStatus.PaymentRecieved;
           _unitOfWork.Repository<Order>().Update(order);

           await _unitOfWork.Complete();

           return order;
        }

        public async Task<Core.Entities.OrderAggregate.Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if(order == null) return null;

            order.Status = OrderStatus.PaymentFailed;
           _unitOfWork.Repository<Order>().Update(order);

           await _unitOfWork.Complete();

           return order;
        }
    }
}