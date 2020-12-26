using System;
using System.Linq.Expressions;
using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
    public class OrdersWithItemsAndOrderingSpecificaiton : BaseSpecification<Order>
    {
        public OrdersWithItemsAndOrderingSpecificaiton(string email) 
            : base(o => o.BuyerEmail == email)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
            AddOrderByDescending(o => o.OrderDate);

        }

        public OrdersWithItemsAndOrderingSpecificaiton(int id, string buyerEmail) 
            : base(o => o.Id == id && o.BuyerEmail == buyerEmail)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
        }
    }
}