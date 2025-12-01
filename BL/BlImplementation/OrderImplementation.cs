using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlImplementation
{
    internal class OrderImplementation : BIApi.IOrder
    {

 

        public void addOrder(int requesterId, BO.Order order)
        {
            throw new NotImplementedException();
        }

        public void CancelOrder(int requesterId, int orderId)
        {
            throw new NotImplementedException();
        }

        public void ChooseOrderForHandling(int requesterId, int courierId, int orderId)
        {
            throw new NotImplementedException();
        }

        public void CompleteOrderHandling(int requesterId, int courierId, int deliveryId)
        {
            throw new NotImplementedException();
        }

        public void deleteOrder(int requesterId, int orderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClosedDeliveryInList> GetClosedOrders(int requesterId, int courierId, BO.OrderType? filterBy, ClosedDeliveryField? sortBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OpenOrderInList> getOpenOrders(int requesterId, int courierId, BO.OrderType? filterBy, OpenDeliveryField? sortBy)
        {
            throw new NotImplementedException();
        }

        public BO.Order? GetOrderDetails(int requesterId, int orderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetOrdersAmountSummary(int requesterId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DO.Order> GetOrdersList(int requesterId, OrderField? filterBy, object? obj, OrderField? sortBy)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(int requesterId, BO.Order order)
        {
            throw new NotImplementedException();
        }
    }
}
