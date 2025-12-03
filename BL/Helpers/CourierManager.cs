using BO;
using DalApi;
using DO;
using System.Linq;
using System.Numerics;

namespace Helpers;

internal static class CourierManager
{
    private static IDal s_dal = Factory.Get;

    #region Getters
    internal static BO.Courier? GetCourier(int id)
    {
        DO.Courier? doCourier = null;

        doCourier = s_dal.Courier.Read(id) ??
            throw new BO.BlDoesNotExistException ($"Courier with ID={id} does Not exist");

        // Calculate Orders Delivered In Time and After Time
        BO.Courier temp = new BO.Courier()
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
            OrdersDeliveredInTime = DeliveriesInTime(id),
            OrdersDeliveredAfterTime = DeliveredLate(id),
            InProgress = GetOrderInProgress(id),
        };

        return temp;

    }

    internal static IEnumerable<BO.CourierInList> GetAllCouriers(Func<BO.CourierInList, bool>? predicate = null)
    {
        IEnumerable<DO.Courier> dalCouriers = s_dal.Courier.ReadAll();

        var query = from doCourier in dalCouriers 

                    let courierDeliveries = s_dal.Delivery.ReadAll(d => d.CourierId == doCourier.Id)

                    let successCount = courierDeliveries.Count(d => d.EndTime != null)
                    let lateCount = courierDeliveries.Count(d => d.EndTime != null)


                    let activeDelivery = courierDeliveries.FirstOrDefault(d => d.EndTime == null)

                    select new BO.CourierInList
                    {
                        Id = doCourier.Id,
                        FullName = doCourier.Name,

                        IsActive = doCourier.IsActive,
                        ShipmentType = (BO.ShipmentType)doCourier.PreferredShipmentType,
                        StartTime = doCourier.EmploymentStartTime,


                        DeliveredOnTimeCount = successCount,
                        DeliveredLateCount = lateCount,
                        CurrentOrderId = activeDelivery?.Id
                    };

        if (predicate != null)
        {
            return query.Where(predicate);
        }

        return query;
    }


    internal static BO.UserType GetUserType(string id)
    {
        var courier = s_dal.Courier.Read(p => p.Name == id);

        if (courier == null)
        {
            throw new BlDoesNotExistException($"User with Name={id} does not exist");
        }

        return (s_dal.Config.ManagerId == courier.Id) ? BO.UserType.Manager : BO.UserType.Courier;
    }

    #endregion

    #region Setters and Modifiers
    internal static void AddCourier(BO.Courier courier)
    {
   
        try { 
        s_dal.Courier.Create(new DO.Courier()
        {
            Id = courier.Id,
            Name = courier.Name,
            Phone = courier.Phone,
            Email = courier.Email,
            Password = courier.Password,
            IsActive = courier.IsActive,
            PersonalMaxDistance = courier.MaxDistance,
            PreferredShipmentType = (DO.ShipmentType)courier.ShipmentType,
            EmploymentStartTime = courier.StartTime,
        });
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Courier with ID={courier.Id} already exists", ex);
        }
    }

    internal static void DeleteCourier(int id)
    {
        try
        {
            var courier = Helpers.CourierManager.GetCourier(id);
            var hasHistory = s_dal.Delivery.ReadAll(p=> p.CourierId == id);
            if (courier!.InProgress != null)
                throw new BO.BlInvalidOperationStateException("Cannot delete a courier with an order in progress");
            else if(hasHistory.Any())
                throw new BO.BlInvalidOperationStateException("Cannot delete a courier with delivery history");

                s_dal.Courier.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Courier with ID={id} does not exist", ex);
        }
    }

    internal static void UpdateCourier(BO.Courier boCourier)
    {

        // Ensure Validity of Input
        ValidationOfCarrier(boCourier);

        // Check if courier in the middle of a delivery when trying to deactivate
        var activeDeliveriesCheck =
            (from delivery in s_dal.Delivery.ReadAll()
             where delivery.CourierId == boCourier.Id && delivery.EndTime == null
             let isCourierBeingDeactivated = boCourier.IsActive == false
             where isCourierBeingDeactivated
             select delivery).ToList();

        if (activeDeliveriesCheck.Any())
        {
            throw new BO.BlInvalidOperationStateException($"Cannot deactivate courier {boCourier.Id} while they have active deliveries in progress.");
        }

        var courier = s_dal.Courier.Read(boCourier.Id);
   
        // Convert BO to DO
        DO.Courier doCourier = new DO.Courier
        {
            Id = courier.Id == boCourier.Id ? boCourier.Id : throw new BO.BlInvalidInputException("Cannot change id"),
            Name = boCourier.Name,
            Phone = boCourier.Phone,
            Email = boCourier.Email,
            Password = boCourier.Password,
            IsActive = boCourier.IsActive,
            PersonalMaxDistance = boCourier.MaxDistance,
            PreferredShipmentType = (DO.ShipmentType)boCourier.ShipmentType,
            EmploymentStartTime = courier.EmploymentStartTime == boCourier.StartTime? boCourier.StartTime
            : throw new BO.BlInvalidInputException("Cannot change start time"),
        };

        // Update in DAL
        try
        {
            s_dal.Courier.Update(doCourier);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Courier with ID={boCourier.Id} does not exist", ex);
        }
    }
    #endregion

    #region Validation Methods

    internal static bool EnsureIsManager(string id)
    {
        if (GetUserType(id) != BO.UserType.Manager)
            throw new BLUnauthorizedAccessException("Unauthorized access attempt detected");
        return true;
    }

    /// <summary>
    /// Input Validity Check
    /// </summary>
    /// <returns>true if all valid</returns>
    internal static bool ValidationOfCarrier(BO.Courier courier)
    {
    if(
        Helpers.Tools.IdValidtion(courier.Id)
        || courier.Phone.Length != 10 || courier.Phone[0] != 0
        || Helpers.Tools.IsEmailValidManual(courier.Email)
    ) throw new BO.BlInvalidInputException("Invalid Courier Input");

        return true;
    }

    #endregion

    #region private
    private static BO.OrderInProgress? GetOrderInProgress(int courierId)
    {
        // 1. Find the courier's active delivery (not yet completed)
        DO.Delivery? activeDoDelivery = s_dal.Delivery.ReadAll()
            .FirstOrDefault(d => d.CourierId == courierId && d.EndTime == null);

        // If no active delivery exists - return null
        if (activeDoDelivery == null)
            return null;

        // 2. Get the full order details for the delivery's OrderId
        DO.Order? orderDo = s_dal.Order.Read(activeDoDelivery.OrderId);

        // Safety check for edge cases (should not happen in a valid system)
        if (orderDo == null)
            return null;

        // Variables needed for calculations
        DO.Courier courier = s_dal.Courier.Read(courierId);
        var timeGap = orderDo.OrderCreationTime.Add(s_dal.Config.maxSupplayTime) - AdminManager.Now;

        double lat = (s_dal.Config.Latitude == null)? 0 : (double)s_dal.Config.Latitude;
        double longit = (s_dal.Config.Longitude == null) ? 0 : (double)s_dal.Config.Longitude;

        var realDistance = Tools.GetTravelDistance(
            lat,
            longit,
            orderDo.Latitude,
            orderDo.Longitude,
            courier!.PreferredShipmentType
        );
        var scheduleStatus = timeGap.TotalHours switch
        {
            var h when h < 0 => BO.ScheduleStatus.Late,
            var h when h > s_dal.Config.RiskRange.TotalHours => BO.ScheduleStatus.OnTime,
            _ => BO.ScheduleStatus.InRisk
        };

        // 3. Build the complete business object
        return new BO.OrderInProgress
        {
            DeliveryId = activeDoDelivery.Id,
            OrderId = orderDo.Id,
            OrderType = (BO.OrderType)orderDo.OrderType,
            CustomerDescription = orderDo.Description,
            CustomerAddress = orderDo.FullAddress,

            AirDistance = Tools.CalculateAirDistance(
                orderDo.Latitude,
                orderDo.Longitude
            ),

            RealDistance = activeDoDelivery.DistanceKm,

            CustomerName = orderDo.CustomerName,
            CustomerPhone = orderDo.CustomerPhone,

            OrderCreated = orderDo.OrderCreationTime,
            DeliveryStart = activeDoDelivery.StartTime,

            // Logical fields that need calculation or retrieval from configuration (here just copied for example)
            ExpectedArrivalTime = activeDoDelivery.StartTime.Add(
                TimeSpan.FromHours(realDistance / Tools.SpeedKmByType(courier.PreferredShipmentType))
            ),
            LastArrivalTime = activeDoDelivery.StartTime.Add(s_dal.Config.maxSupplayTime),

            // Logical status fields
            OrderStatus = BO.OrderStatus.InProgress, // Because this is an active delivery
            ScheduleStatus = scheduleStatus,

            TimeGap = timeGap,
        };
       
    }

    private static int DeliveriesInTime(int id)
    {
        return (from item in s_dal.Delivery.ReadAll(p => p.CourierId == id)
                let order = Helpers.OrderManager.GetOrder(item.OrderId)
                where order != null && order.ScheduleStatus == BO.ScheduleStatus.OnTime
                select order).Count();
    }

    private static int DeliveredLate(int id)
    {
        return (from item in s_dal.Delivery.ReadAll(
        p => p.CourierId == id)
                let order = Helpers.OrderManager.GetOrder(item.OrderId)
                where order != null && order.ScheduleStatus == BO.ScheduleStatus.Late
                select item).Count();
    }

    #endregion
}









