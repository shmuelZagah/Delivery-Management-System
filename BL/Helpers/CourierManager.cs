using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class CourierManager
{
    private static IDal s_dal = Factory.Get;

    internal static BO.Courier? GetCourier(int id)
    {
        DO.Courier? doCourier = null;

        doCourier = s_dal.Courier.Read(id) ??
            throw new Exception /*BO.BlDoesNotExistException*/ ($"Courier with ID={id} does Not exist");



        return new BO.Courier()
        {
            Id = id,
            Name = doCourier.Name,
            Phone = doCourier.Phone,
            Email = doCourier.Email,
            Password = doCourier.Password,
            IsActive = doCourier.IsActive,
            MaxDistance = doCourier.PersonalMaxDistance,
            ShipmentType = (BO.ShipmentType)doCourier.PreferredShipmentType,
            StartTime = doCourier.EmploymentStartTime,
        };

    }

    internal static IEnumerable<BO.Courier> GetAllCouriers(Func<BO.Courier, bool>? predicate = null)
    {
        var dalCouriers = s_dal.Courier.ReadAll();
        var boCouriers = from doCourier in dalCouriers
                         select new BO.Courier()
                         {
                             Id = doCourier.Id,
                             Name = doCourier.Name,
                             Phone = doCourier.Phone,
                             Email = doCourier.Email,
                             Password = doCourier.Password,
                             IsActive = doCourier.IsActive,
                             MaxDistance = doCourier.PersonalMaxDistance,
                             ShipmentType = (BO.ShipmentType)doCourier.PreferredShipmentType,
                             StartTime = doCourier.EmploymentStartTime,
                         };

        return predicate == null ? boCouriers : boCouriers.Where(predicate);
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

                ExpectedArrivalTime = CalculateExpected(doOrder), // אם יש לוגיקה
                LastArrivalTime = GetLastArrival(doOrder.Id),      // אם יש DAL

                OrderStatus = GetOrderStatus(doOrder.Id),          // קריאה מה-DAL
                ScheduleStatus = GetScheduleStatus(doOrder.Id),

                TimeLeftToDeadline = CalculateTimeLeft(doOrder.Id),

                CouriersForOrder = GetCouriersForOrder(doOrder.Id)
            };

        return predicate is null ? boOrders : boOrders.Where(predicate);
    }


    internal static BO.UserType GetUserType(string name)
    {
        var courier = s_dal.Courier.Read(p => p.Name == name);

        if (courier == null)
        {
            throw new BlDoesNotExistException($"User with Name={name} does not exist");
        }

        return (s_dal.Config.ManagerId == courier.Id) ? BO.UserType.MANAGER : BO.UserType.COURIER;
    }
}





