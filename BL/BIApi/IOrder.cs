using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApi
{
    public interface IOrder
    {

        //must use linq in here

        IEnumerable<int> GetOrdersAmountSummary(int requesterId);

        IEnumerable<DO.Order> GetOrdersList(int requesterId, OrderField? filterBy, object? obj, OrderField? sortBy);

        BO.Order? GetOrderDetails(int requesterId, int orderId);

        void UpdateOrder(int requesterId, BO.Order order);

        void CancelOrder(int requesterId, int orderId);

        void deleteOrder(int requesterId, int orderId);

        void addOrder(int requesterId, BO.Order order);

        void CompleteOrderHandling(int requesterId, int courierId, int deliveryId);

        void ChooseOrderForHandling(int requesterId, int courierId, int orderId);

        IEnumerable<BO.ClosedDeliveryInList> GetClosedOrders(int requesterId, int courierId, OrderType? filterBy, ClosedDeliveryField? sortBy);

        IEnumerable<BO.OpenOrderInList> getOpenOrders(int requesterId, int courierId, OrderType? filterBy, OpenDeliveryField? sortBy);






    }
}
