using BO;
using DalApi;
using DO;

namespace Helpers;

internal class OrderManager
{
    private static IDal s_dal = Factory.Get;


    internal static BO.Order? GetOrder(int id)
    {
        DO.Order doOrder;

        try
        {
            doOrder = s_dal.Order.Read(id)
                ?? throw new BO.BlDoesNotExistException($"Order with ID={id} does not exist");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Order with ID={id} does not exist", ex);
        }

        return new BO.Order()
        {
            Id = doOrder.Id,
            OrderType = (BO.OrderType)doOrder.OrderType,
            Description = doOrder.Description,
            Address = doOrder.FullAddress,
            Latitude = doOrder.Latitude,
            Longitude = doOrder.Longitude,
            CustomerName = doOrder.CustomerName,
            CustomerPhone = doOrder.CustomerPhone,
            Notes = doOrder.Notes,
            OrderCreated = doOrder.OrderCreationTime
        };
    }

    internal static IEnumerable<BO.Order> GetAllOrders(Func<BO.Order, bool>? predicate = null)
    {
        var dalOrders = s_dal.Order.ReadAll();

        var boOrders =
            from doOrder in dalOrders
            select new BO.Order()
            {
                Id = doOrder.Id,
                OrderType = (BO.OrderType)doOrder.OrderType,
                Description = doOrder.Description,
                Address = doOrder.FullAddress,
                Latitude = doOrder.Latitude,
                Longitude = doOrder.Longitude,
                //AirDistance = doOrder.
                CustomerName = doOrder.CustomerName,
                CustomerPhone = doOrder.CustomerPhone,
                Notes = doOrder.Notes,
                OrderCreated = doOrder.OrderCreationTime,

                // הערכים האלו כנראה מגיעים מטבלה אחרת או מחישוב:

                orderType = (BO.OrderType)doOrder.OrderType,           // אם יש DAL

                //    ExpectedArrivalTime = CalculateExpected(doOrder), // אם יש לוגיקה
                //    LastArrivalTime = GetLastArrival(doOrder.Id),      // אם יש DAL

                //    OrderStatus = GetOrderStatus(doOrder.Id),          // קריאה מה-DAL
                //    ScheduleStatus = GetScheduleStatus(doOrder.Id),

                //    TimeLeftToDeadline = CalculateTimeLeft(doOrder.Id),

                //    CouriersForOrder = GetCouriersForOrder(doOrder.Id)
            };

        return predicate is null ? boOrders : boOrders.Where(predicate);
    }

    internal static void UpdateOrder(BO.Order order)
    {
        DO.Order toUpdate = new DO.Order()
        {
            Id = order.Id,
            OrderType = (DO.OrderType)order.orderType,
            Description = order.Description,
            FullAddress = order.Address,
            Latitude = order.Latitude,
            Longitude = order.Longitude,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            OrderCreationTime = order.OrderCreated.Date
        };

        s_dal.Order.Update(toUpdate);
    }

    internal static void DeleteOrder(int orderId)
    {
        s_dal.Order.Delete(orderId);
    }

    /// <summary>
    /// adding order to the data list
    /// </summary>
    internal static void AddOrder(BO.Order order)
    {


        // BO → DO
        DO.Order doOrder = new DO.Order(
            Id: order.Id,
            OrderType: (DO.OrderType)order.OrderType,
            Description: order.Description,
            FullAddress: order.Address,
            Latitude: order.Latitude,
            Longitude: order.Longitude,
            CustomerName: order.CustomerName,
            CustomerPhone: order.CustomerPhone,
            Notes: order.Notes,
            OrderCreationTime: order.OrderCreated
        );


        s_dal.Order.Create(doOrder);

    }

    #region Time & statuses

    /// <summary>
    /// Calculates the status of an order (Open, In Progress, Delivered, etc.)
    /// based on its delivery history.
    /// </summary>
    internal static BO.OrderStatus GetOrderStatus(int orderId)
    {
        // 1. Retrieve all deliveries related to this order
        var deliveries = s_dal.Delivery.ReadAll(d => d.OrderId == orderId);

        // 2. Check if there is an active delivery (In Progress)
        if (deliveries.Any(d => d.EndTime == null))
        {
            return BO.OrderStatus.InProgress;
        }

        // 3. Retrieve the most recent completed delivery (if any)
        var lastDelivery = deliveries
            .Where(d => d.EndTime != null)
            .OrderByDescending(d => d.EndTime)
            .FirstOrDefault();

        // 4. If there are no deliveries at all – the order is considered open
        if (lastDelivery == null)
        {
            return BO.OrderStatus.Open;
        }

        // 5. Determine the status based on the result of the last delivery
        return lastDelivery.DeliveryEndType switch
        {
            DO.DeliveryEndType.Provided => BO.OrderStatus.Completed,
            DO.DeliveryEndType.ClientRefusedAccept => BO.OrderStatus.ClientRefusedAccept,
            DO.DeliveryEndType.Canceled => BO.OrderStatus.Canceled,
            DO.DeliveryEndType.ClientNotFound => BO.OrderStatus.Open,

            // Default case
            _ => BO.OrderStatus.Open
        };
    }

    /// <summary>
    /// Calculates the schedule status of an order (OnTime, InRisk, Late)
    /// based on the order's creation time, configuration settings, and system clock.
    /// </summary>
    internal static BO.ScheduleStatus GetOrderScheduleStatus(int orderId)
    {
        // 1. Retrieve the order (to know when it was created)
        DO.Order? order = s_dal.Order.Read(orderId);
        if (order == null) throw new BO.BlDoesNotExistException($"Order {orderId} not found");

        // 2. Calculate the deadline:
        // Creation time + maximum delivery time from the configuration
        DateTime deadline = order.OrderCreationTime.Add(s_dal.Config.maxSupplayTime);

        // 3. Check whether the order has already been delivered
        var deliveries = s_dal.Delivery.ReadAll(d => d.OrderId == orderId);
        var lastSuccess = deliveries
            .Where(d => d.EndTime != null && d.DeliveryEndType == DO.DeliveryEndType.Provided)
            .OrderByDescending(d => d.EndTime)
            .FirstOrDefault();

        // Case A: The order has already been delivered
        if (lastSuccess != null)
        {
            return lastSuccess.EndTime > deadline
                ? BO.ScheduleStatus.Late
                : BO.ScheduleStatus.OnTime;
        }

        // Case B: The order is still active (open or in progress)
        TimeSpan timeLeft = deadline - AdminManager.Now;

        if (timeLeft.TotalMilliseconds < 0)
        {
            return BO.ScheduleStatus.Late; // Deadline passed
        }

        // Compare with the risk threshold from the configuration
        if (timeLeft < s_dal.Config.RiskRange)
        {
            return BO.ScheduleStatus.InRisk; // Time is running out
        }

        return BO.ScheduleStatus.OnTime; // Everything is on schedule
    }
    #endregion

}

  



