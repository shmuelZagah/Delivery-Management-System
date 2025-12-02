using BO;
using DalApi;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlImplementation
{
    internal class OrderImplementation : BIApi.IOrder
    {

 
        /// <summary>
        /// adding order to the data-layer
        /// </summary>

        public void addOrder(int requesterId, BO.Order order)
        {
            if (!CourierManager.EnsureIsManager(requesterId.ToString()))
                throw new AccessViolationException();

            OrderManager.AddOrder(order);
        }

        public void CancelOrder(int requesterId, int orderId)
        {
            if (!CourierManager.EnsureIsManager(requesterId.ToString()))
                throw new AccessViolationException();

           var order = OrderManager.GetOrder(orderId);

            // case 1 - if the order had finished of cancelled
           if(order!.OrderStatus == OrderStatus.Cancelled ||
                order!.OrderStatus == OrderStatus.Completed)
                
                throw new BO.BlInvalidOperationStateException($"cannot cancel order {orderId}");

            var doDelivery = DeliveryManager.GetDeliveryByOrderId(orderId);

            //case 2 - if the order had created then try to update as cancelled

            if (order!.OrderStatus == OrderStatus.Created)
            {

                DO.Delivery delivery = new DO.Delivery(
                   Id: doDelivery.Id,
                   OrderId: orderId,
                   CourierId: 0,  
                   StartTime: AdminManager.Now,
                   DistanceKm: 0,
                   DeliveryType: DO.ShipmentType.Bicycle,
                   DeliveryEndType: DO.DeliveryEndType.Cancelled,
                   EndTime: AdminManager.Now
                   );

                try
                {
                    DeliveryManager.createDelivery(delivery);
                }
                catch (DO.DalDoesNotExistException ex)
                {
                    throw new BlDoesNotExistException("cant update" , ex);
                }
            }

            //case 3 - if the order is in progress
            if(order!.OrderStatus == OrderStatus.InProgress)
            {
                DO.Delivery delivery = new DO.Delivery(
             Id: doDelivery.Id,
             OrderId: orderId,
             CourierId: doDelivery.CourierId,
             StartTime: doDelivery.StartTime,
             DistanceKm: doDelivery.DistanceKm,
             DeliveryType: DO.ShipmentType.Bicycle,
             DeliveryEndType: DO.DeliveryEndType.Cancelled,
             EndTime: AdminManager.Now
             );

                try
                {
                    DeliveryManager.UpdateDelivery(delivery);
                }
                catch (DO.DalDoesNotExistException ex)
                {
                    throw new BlDoesNotExistException("cant update", ex);
                }
            }


        }

        public void ChooseOrderForHandling(int requesterId, int courierId, int orderId)
        {
            if (!CourierManager.EnsureIsManager(requesterId.ToString()))
                throw new AccessViolationException();
        }

        public void CompleteOrderHandling(int requesterId, int courierId, int deliveryId)
        {
            if (!CourierManager.EnsureIsManager(requesterId.ToString()))
                throw new AccessViolationException();

            var deliveryToUpdate = DeliveryManager.GetDelivery(deliveryId) ;
            
        }

        /// <summary>
        /// delete order by id
        /// </summary>
        public void deleteOrder(int requesterId, int orderId)
        {
            if (!CourierManager.EnsureIsManager(requesterId.ToString()))
                throw new AccessViolationException();

            try
            {
                OrderManager.DeleteOrder(orderId);
            }

            catch (DO.DalDoesNotExistException ex){

                throw new BO.BlDoesNotExistException($"cant delete order with {orderId} id", ex);
            }
        }


        public IEnumerable<ClosedDeliveryInList> GetClosedOrders(int requesterId, int courierId, BO.OrderType? filterBy, ClosedDeliveryField? sortBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OpenOrderInList> getOpenOrders(int requesterId, int courierId, BO.OrderType? filterBy, OpenDeliveryField? sortBy)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// return order detail by id 
        /// </summary>
        public BO.Order? GetOrderDetails(int requesterId, int orderId)
        {
            if (!CourierManager.EnsureIsManager(requesterId.ToString()))
            {
                throw new AccessViolationException();
            }

            var order = OrderManager.GetOrder(orderId);

            return order;
        }

        /// <summary>
        /// Accepts id of who is asking and return arr the represents sum by status
        /// </summary>

        public int[][] GetOrdersAmountSummary(int requesterId)
        {
            if (!CourierManager.EnsureIsManager(requesterId.ToString()))
                throw new AccessViolationException();

            var orders = OrderManager.GetAllOrders();

            // GroupBy two enums
            var grouped =
                orders.GroupBy(o => new { o.OrderStatus, o.ScheduleStatus })
                      .Select(g => new
                      {
                          OrderStatus = g.Key.OrderStatus,
                          ScheduleStatus = g.Key.ScheduleStatus,
                          Count = g.Count()
                      })
                      .ToList();

            int orderStatusCount = Enum.GetValues<OrderStatus>().Length;
            int scheduleStatusCount = Enum.GetValues<ScheduleStatus>().Length;

            // create the matrix
            var matrix =
                Enumerable.Range(0, orderStatusCount)
                          .Select(orderIndex =>
                              Enumerable.Range(0, scheduleStatusCount)
                                        .Select(scheduleIndex =>
                                            grouped.FirstOrDefault(g =>
                                                (int)g.OrderStatus == orderIndex &&
                                                (int)g.ScheduleStatus == scheduleIndex
                                            )?.Count ?? 0
                                        )
                                        .ToArray()
                          )
                          .ToArray();

            return matrix;
        }

        /// <summary>
        /// Retrieves a filtered and sorted list of orders based on optional criteria:
        /// order status, schedule status, and a dynamic selector (ID, phone, date, etc.).
        /// Only managers are authorized to access this method.
        /// </summary>
        public IEnumerable<BO.OrderInList> GetOrdersList(
        int requesterId,
        OrderStatus? statusFilter,      // Nullable filter for OrderStatus
        object? selector,               // Optional filter by dynamic type (ID, phone, date, etc.)
        ScheduleStatus? scheduleFilter) // Nullable filter for ScheduleStatus
        {
            // 1. Ensure the requester is a manager
            if (!CourierManager.EnsureIsManager(requesterId.ToString()))
                throw new AccessViolationException("Access denied: manager privileges required.");

            // 2. Retrieve all orders (logical layer objects)
            var orders = OrderManager.GetAllOrders();

            // 3. Filter by OrderStatus if provided
            if (statusFilter is not null)
                orders = orders.Where(o => o.OrderStatus == statusFilter);

            // 4. Filter by ScheduleStatus if provided
            if (scheduleFilter is not null)
                orders = orders.Where(o => o.ScheduleStatus == scheduleFilter);

            // 5. Filter by selector (dynamic typed object)
            //    - Applies additional filtering only if selector is not null
            if (selector is not null)
            {
                orders =
                    selector switch
                    {
                        // Filter by order ID
                        int id => orders.Where(o => o.Id == id),

                        // Filter by customer phone number
                        string phone => orders.Where(o => o.CustomerPhone == phone),

                        // Filter by order date
                        DateTime date => orders.Where(o => o.OrderCreated.Date == date.Date),

                        // Unknown selector type → no filtering applied
                        _ => orders
                    };
            }

            // 6. Project results into BO.OrderInList objects
            var result =
                orders
                .Select(order => new BO.OrderInList
                {
                    OrderId = order.Id,
                    OrderType = order.orderType,
                    AirDistance = order.AirDistance,
                    OrderStatus = order.OrderStatus,
                    ScheduleStatus = order.ScheduleStatus,
                    //TimeLeftToDeadline = ??
                    //TimeLeftToFinish = ??
                    //CouriersCount = ??

                })

                // 7. Sort the final list by OrderStatus (as required)
                .OrderBy(o => o.OrderStatus);

            return result;
        }


        /// <summary>
        /// update order by id
        /// </summary>
        public void UpdateOrder(int requesterId, BO.Order order)
        {
            if (!CourierManager.EnsureIsManager(requesterId.ToString()))
                throw new AccessViolationException();

            OrderManager.UpdateOrder(order);
     
        }
    }
}
