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
}



