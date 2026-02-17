using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Helpers;

/// <summary>
/// Full logical engine for Orders and Deliveries.
/// Contains ALL DAL access, logic, calculations, DO→BO mappings, LINQ queries.
/// </summary>
internal static class OrderManager
{
    private static readonly IDal s_dal = Factory.Get;
    internal static ObserverManager Observers = new();

    // ============================================================
    #region DAL ACCESS
    // ============================================================

    private static DO.Order LoadDoOrder(int orderId)
    {
        try
        {
            lock (AdminManager.BlMutex)
                return s_dal.Order.Read(orderId)
                       ?? throw new BlDoesNotExistException($"Order with ID={orderId} does not exist");
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Order with ID={orderId} does not exist", ex);
        }
    }

    private static IEnumerable<DO.Order> LoadAllOrders()
    {
        lock (AdminManager.BlMutex)
        {
            return s_dal.Order.ReadAll()?.Cast<DO.Order>() ?? Enumerable.Empty<DO.Order>();
        }
    }
    private static IEnumerable<DO.Delivery> LoadAllDeliveries()
    {
        lock (AdminManager.BlMutex)
        {
            return s_dal.Delivery.ReadAll()?.Cast<DO.Delivery>() ?? Enumerable.Empty<DO.Delivery>();
        }
    }
    private static IEnumerable<DO.Delivery> GetDeliveriesForOrder(int orderId)
    {
        lock (AdminManager.BlMutex)
        {
            return LoadAllDeliveries().Where(d => d.OrderId == orderId);

        }
    }

    private static DO.Delivery? GetLastDeliveryForOrder(int orderId)
        => GetDeliveriesForOrder(orderId).OrderByDescending(d => d.Id).FirstOrDefault();

    #endregion
    // ============================================================


    // ============================================================
    #region STATUS CALCULATIONS
    // ============================================================

    private static OrderStatus CalculateOrderStatus(int orderId)
    {

        var last = GetLastDeliveryForOrder(orderId);

        if (last == null)
            return OrderStatus.Open;

        if (last.DeliveryEndType is null)
            return OrderStatus.InProgress;

        return last.DeliveryEndType.Value switch
        {
            DO.DeliveryEndType.Provided => OrderStatus.Completed,
            DO.DeliveryEndType.ClientRefusedAccept => OrderStatus.Close,
            DO.DeliveryEndType.Canceled => OrderStatus.Canceled,

            DO.DeliveryEndType.ClientNotFound => OrderStatus.Open,
            DO.DeliveryEndType.Failed => OrderStatus.Open,

            _ => OrderStatus.Open
        };
    }

    private static bool CaculateIfIsClosed(OrderStatus status)
    {
        if (status == OrderStatus.Completed ||
            status == OrderStatus.Close ||
            status == OrderStatus.Canceled)
        {
            return true;
        }

        return false;
    }


    private static ScheduleStatus CalculateScheduleStatus(DO.Order doOrder)
    {

        lock (AdminManager.BlMutex)
        {
            var config = s_dal.Config;
            DateTime deadline = doOrder.OrderCreationTime + config.maxSupplayTime;
            TimeSpan riskRange = config.RiskRange;


            var last = GetLastDeliveryForOrder(doOrder.Id);

            var orderStatus = CalculateOrderStatus(doOrder.Id);



            if (orderStatus == OrderStatus.Canceled)
                return ScheduleStatus.Unknown;

            if (CaculateIfIsClosed(orderStatus))
            {
                if (last?.EndTime == null)
                    return ScheduleStatus.Unknown;

                return (last.EndTime.Value <= deadline)
                    ? ScheduleStatus.OnTime
                    : ScheduleStatus.Late;
            }

            TimeSpan left = deadline - AdminManager.Now;

            if (left <= TimeSpan.Zero) return ScheduleStatus.Late;
            if (left <= riskRange) return ScheduleStatus.InRisk;
            return ScheduleStatus.OnTime;
        }
    }

    private static TimeSpan CalculateTimeLeftToDeadline(DO.Order doOrder)
    {

        DateTime deadline = doOrder.OrderCreationTime + getMaxSupply();
        DateTime now = AdminManager.Now;


        if (CaculateIfIsClosed(CalculateOrderStatus(doOrder.Id)))
            return TimeSpan.Zero;

        var diff = deadline - now;
        return diff <= TimeSpan.Zero ? TimeSpan.Zero : diff;


    }


    private static async Task<DateTime?> CaculateExpectedArrivalTime(DO.Order doOrder)
    {
        DO.Delivery? last;
        lock (AdminManager.BlMutex)
        {
            last = GetLastDeliveryForOrder(doOrder.Id);
        }

        if (last == null || last.DeliveryEndType != null)
            return null;

        double lat, lon;

        lock (AdminManager.BlMutex)
        {
            var config = s_dal.Config;

            if (config.Latitude != null && config.Longitude != null)
            {
                lat = config.Latitude.Value;
                lon = config.Longitude.Value;
            }
            else
            {
                throw new InvalidDataException("Company location is not set in config.");
            }
        }

        double distanceKm = await Tools.GetTravelDistance(
            lat, lon,
            doOrder.Latitude, doOrder.Longitude, last.DeliveryType);

        double speedKmh = Tools.SpeedKmByType(last.DeliveryType);
        TimeSpan travel = TimeSpan.FromHours(distanceKm / speedKmh);

        return last.StartTime + travel;
    }

    private static double CalculateAirDistance(DO.Order doOrder)
    {
        lock (AdminManager.BlMutex)
        {
            var cfg = s_dal.Config;

            if (cfg.Latitude == null || cfg.Longitude == null)
                return 0;

            return Tools.CalculateAirDistance(
                cfg.Latitude.Value,
                cfg.Longitude.Value,
                doOrder.Latitude,
                doOrder.Longitude);
        }

    }


    #endregion
    // ============================================================


    // ============================================================
    #region DO → BO BUILDERS
    // ============================================================

    internal static TimeSpan getMaxSupply()
    {
        lock (AdminManager.BlMutex)
            return s_dal.Config.maxSupplayTime;
    }

    private static DO.Courier getCourier(int courierId)
    {
        lock (AdminManager.BlMutex)
            return s_dal.Courier.Read(courierId)!;
    }
    private static async Task<BO.Order> BuildBoOrder(DO.Order doOrder)
    {


        var deliveries = GetDeliveriesForOrder(doOrder.Id).ToList();
        var orderStatus = CalculateOrderStatus(doOrder.Id);
        var scheduleStatus = CalculateScheduleStatus(doOrder);

        var air = CalculateAirDistance(doOrder);
        var timeLeft = CalculateTimeLeftToDeadline(doOrder);
        DateTime deadline = doOrder.OrderCreationTime + getMaxSupply();

        var lastDelivery = GetLastDeliveryForOrder(doOrder.Id);
        // DateTime lastArrival = lastDelivery!.EndTime != null ? lastDelivery.EndTime.Value
        //: deadline;


        var courierList =
            from d in deliveries
            let courier = getCourier(d.Id)
            select new DeliveryPerOrderInList
            {
                DeliveryId = d.Id,
                CourierId = d.CourierId,
                CourierName = courier?.Name ?? "",
                ShipmentType = (BO.ShipmentType)d.DeliveryType,
                StartDeliveryTime = d.StartTime,
                FinishType = (BO.DeliveryEndType?)d.DeliveryEndType,
                FinishTime = d.EndTime
            };

        return new BO.Order
        {
            Id = doOrder.Id,
            OrderType = (BO.OrderType)doOrder.OrderType,
            Description = doOrder.Description,
            Address = doOrder.FullAddress,
            Latitude = doOrder.Latitude,
            Longitude = doOrder.Longitude,
            AirDistance = air,
            CustomerName = doOrder.CustomerName,
            CustomerPhone = doOrder.CustomerPhone,
            Notes = doOrder.Notes,
            Volume = doOrder.Volume,
            Weight = doOrder.Weight,
            OrderCreated = doOrder.OrderCreationTime,
            ExpectedArrivalTime = await CaculateExpectedArrivalTime(doOrder) ?? DateTime.MinValue,
            LastArrivalTime = deadline,
            OrderStatus = orderStatus,
            ScheduleStatus = scheduleStatus,
            TimeLeftToDeadline = timeLeft,
            CouriersForOrder = courierList.ToList()
        };
    }

    private static OrderInList BuildBoOrderInList(DO.Order doOrder)
    {
        var deliveries = GetDeliveriesForOrder(doOrder.Id).ToList();
        var last = GetLastDeliveryForOrder(doOrder.Id);

        var air = CalculateAirDistance(doOrder);
        var orderStatus = CalculateOrderStatus(doOrder.Id);
        var scheduleStatus = CalculateScheduleStatus(doOrder);
        var timeLeft = CalculateTimeLeftToDeadline(doOrder);



        TimeSpan finishTime = (last?.EndTime != null)
            ? last.EndTime.Value - last.StartTime
            : TimeSpan.Zero;

        return new OrderInList
        {
            DeliveryId = last?.Id,
            OrderId = doOrder.Id,
            OrderType = (BO.OrderType)doOrder.OrderType,
            AirDistance = air,
            OrderStatus = orderStatus,
            ScheduleStatus = scheduleStatus,
            TimeLeftToDeadline = timeLeft,
            TimeTookToFinish = finishTime,
            CouriersCount = deliveries.Count
        };
    }

    internal static (double, double) getConfigLatLon()
    {
        lock (AdminManager.BlMutex)
        {
            var config = s_dal.Config;
            return (config.Latitude ?? 0.0, config.Longitude ?? 0.0);
        }
    }
    private static async Task<OpenOrderInList> BuildBoOpenOrderInList(DO.Order doOrder, int courierId)
    {
        var last = GetLastDeliveryForOrder(doOrder.Id);

        var thisCourier = await CourierManager.GetCourier(courierId);

        var air = CalculateAirDistance(doOrder);
        var schedule = CalculateScheduleStatus(doOrder);
        var timeLeft = CalculateTimeLeftToDeadline(doOrder);


        var deadline = doOrder.OrderCreationTime + getMaxSupply();


        (double compLat, double compLon) = getConfigLatLon();



        double distanceKm = last?.DistanceKm ?? await Tools.GetTravelDistance(compLat, compLon, doOrder.Latitude, doOrder.Longitude,
        (DO.ShipmentType)thisCourier!.ShipmentType);


        double speedKmh = Tools.SpeedKmByType((DO.ShipmentType)thisCourier!.ShipmentType);

        TimeSpan travelTime = TimeSpan.FromHours(distanceKm / speedKmh);

        if (doOrder.OrderCreationTime + travelTime >= deadline)
            schedule = ScheduleStatus.Late;


        return new OpenOrderInList
        {
            CourierId = courierId,
            OrderId = doOrder.Id,
            OrderType = (BO.OrderType)doOrder.OrderType,
            Weight = doOrder.Weight,
            Volume = doOrder.Volume,
            AirDistance = air,
            ActualDistance = distanceKm,
            EstimatedArrivalTime = travelTime,
            ScheduleStatus = schedule,
            TimeLeftToDeadline = deadline - AdminManager.Now,
            MaxTimeToArrival = doOrder.OrderCreationTime + getMaxSupply()
        };
    }

    private static ClosedDeliveryInList BuildBoClosedDeliveryInList(DO.Delivery d, DO.Order o)
    {
        return new ClosedDeliveryInList
        {
            DeliveryId = d.Id,
            OrderId = o.Id,
            OrderType = (BO.OrderType)o.OrderType,
            Address = o.FullAddress,
            ShipmentType = (BO.ShipmentType)d.DeliveryType,
            ActualDistance = d.DistanceKm,
            ProcessingDuration = (d.EndTime ?? d.StartTime) - d.StartTime,
            FinishType = (BO.DeliveryEndType?)d.DeliveryEndType
        };
    }

    #endregion
    // ============================================================


    // ============================================================
    #region ACTIONS (ADD / UPDATE / DELETE / CANCEL / CHOOSE / COMPLETE)
    // ============================================================

    internal static async Task AddOrder(BO.Order bo)
    {
        double lat = 0, lon = 0;

        //If the address not used yet add it
        if (!AdminManager.Addresses.Contains(bo.Address))
        {
            (lat, lon) = await Helpers.Tools.GetLocationFromAddress(bo.Address);


            lock (AdminManager.BlMutex)
            {
                if (!AdminManager.Addresses.Contains(bo.Address))
                {
                    s_dal.Address.Create(new Address(0, bo.Address, lat, lon));
                    var adr = s_dal.Address.Read(d => d.FullAddress == bo.Address);

                    if (adr != null)
                        AdminManager.Addresses.Insert(adr.FullAddress, adr.Id);
                }
            }
        }

        else
        {
            int id = AdminManager.Addresses.GetPositions(bo.Address).FirstOrDefault();
            var adr = s_dal.Address.Read(id);
            if (adr != null)
            {
                lat = adr.Latitude; lon = adr.Longitude;
            }
        }

        var doOrder = new DO.Order(
            0,
            (DO.OrderType)bo.OrderType,
            bo.Description,
            bo.Address,
            lat,
            lon,
            bo.Volume,
            bo.Weight,
            bo.CustomerName,
            bo.CustomerPhone,
            bo.Notes,
            AdminManager.Now
        );

        lock (AdminManager.BlMutex)
            s_dal.Order.Create(doOrder);

        Observers.NotifyListUpdated();
    }

    internal static void UpdateOrder(BO.Order bo)
    {
        var old = LoadDoOrder(bo.Id);

        DO.Order updated = old with
        {
            OrderType = (DO.OrderType)bo.OrderType,
            Description = bo.Description,
            FullAddress = bo.Address,
            Latitude = bo.Latitude,
            Longitude = bo.Longitude,
            CustomerName = bo.CustomerName,
            CustomerPhone = bo.CustomerPhone,
            Notes = bo.Notes,
            Weight = bo.Weight,
            Volume = bo.Volume
        };

        lock (AdminManager.BlMutex)
            s_dal.Order.Update(updated);

        Observers.NotifyItemUpdated(bo.Id);
        Observers.NotifyListUpdated();
    }

    internal static void DeleteOrder(int orderId)
    {
        var deliveries = GetDeliveriesForOrder(orderId);
        if (deliveries.Any())
            throw new BlDoesNotExistException("Cannot delete order that already has deliveries.");

        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Order.Delete(orderId);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Order with ID={orderId} does not exist", ex);
        }

        Observers.NotifyListUpdated();
    }

    internal static void CancelOrder(int orderId)
    {
        var last = GetLastDeliveryForOrder(orderId);
        var orderStatus = CalculateOrderStatus(orderId);

        if (!(orderStatus == OrderStatus.Open || orderStatus == OrderStatus.InProgress))
            throw new BlInvalidOperationStateException(" cannot cancel order if the order is not open or in progress");

        if (orderStatus == OrderStatus.Open)
        {
            var dummy = new DO.Delivery(
                Id: 0,
                OrderId: orderId,
                CourierId: 0,
                DeliveryType: DO.ShipmentType.Foot,
                StartTime: AdminManager.Now,
                DistanceKm: 0,
                DeliveryEndType: DO.DeliveryEndType.Canceled,
                EndTime: AdminManager.Now
            );

            lock (AdminManager.BlMutex)
                s_dal.Delivery.Create(dummy);

            Observers.NotifyItemUpdated(orderId);
            Observers.NotifyListUpdated();

            return;
        }


        if (orderStatus == OrderStatus.InProgress)
        {
            var updated = last with
            {
                DeliveryEndType = DO.DeliveryEndType.Canceled,
                EndTime = AdminManager.Now
            };

            lock (AdminManager.BlMutex)
                s_dal.Delivery.Update(updated);

            Observers.NotifyItemUpdated(orderId);
            Observers.NotifyListUpdated();

            return;
        }
    }

    private static DO.Order getBoOrder(int id)
    {
        lock (AdminManager.BlMutex)
            return s_dal.Order.Read(id);
    }
    internal static async void ChooseOrderForHandling(int courierId, int orderId, double? distance = null)
    {
        var last = GetLastDeliveryForOrder(orderId);


        var order = getBoOrder(orderId)
            ?? throw new BlDoesNotExistException($"Order with ID={orderId} does not exist");

        var courier = await CourierManager.GetCourier(courierId)
            ?? throw new BlDoesNotExistException($"Courier with ID={courierId} does not exist");

        if (courier.IsActive == false)
            throw new BlDoesNotExistException("Cannot choose order — courier is not active.");

        if (courier.InProgress != null)
            throw new BlDoesNotExistException("Cannot choose order — courier is already handling another order.");

        if (last != null && CaculateIfIsClosed(CalculateOrderStatus(orderId)))
            throw new BlDoesNotExistException("Cannot choose order — it already has an active delivery.");

        (double lat, double lon) = getConfigLatLon();

        distance = (distance == null) ? await Helpers.Tools.GetTravelDistance(lat,
            lon, order.Latitude, order.Longitude,
            (DO.ShipmentType)courier.ShipmentType) : distance;

        var doOrder = LoadDoOrder(orderId);
        var shipment = (DO.ShipmentType)courier.ShipmentType;

        var delivery = new DO.Delivery(
            0,
            orderId,
            courierId,
            shipment,
            AdminManager.Now,
            distance,
            null,
            null
        );
        lock (AdminManager.BlMutex)
            s_dal.Delivery.Create(delivery);

        Observers.NotifyItemUpdated(orderId);
        Observers.NotifyListUpdated();
    }

    private static DO.Delivery? GetDelivery(int id)
    {
        lock (AdminManager.BlMutex)
            return s_dal.Delivery.Read(id);
    }
    internal static void CompleteOrderHandling(int courierId, int deliveryId)
    {
        var delivery = GetDelivery(deliveryId)
            ?? throw new BlDoesNotExistException($"Delivery {deliveryId} does not exist");

        if (delivery.CourierId != courierId)
            throw new BlDoesNotExistException("Courier mismatch");

        if (delivery.DeliveryEndType != null)
            throw new BlDoesNotExistException("Delivery already completed.");

        var updated = delivery with
        {
            DeliveryEndType = DO.DeliveryEndType.Provided,
            EndTime = AdminManager.Now,
            DistanceKm = delivery.DistanceKm ?? 0
        };
        lock (AdminManager.BlMutex)
            s_dal.Delivery.Update(updated);

        Observers.NotifyItemUpdated(updated.OrderId);
        Observers.NotifyListUpdated();

    }

    internal static void DeliveryEnded(int orderId, BO.DeliveryEndType deliveryEndType)
    {
        DO.Order tempOrder = getBoOrder(orderId)
            ?? throw new BlDoesNotExistException("Order not found");

        DO.Delivery last = GetLastDeliveryForOrder(orderId) ?? throw new BlDoesNotExistException("Delivery not found");

        if (last == null || last.DeliveryEndType != null)
            throw new BlInvalidOperationStateException("No active delivery to update.");
        DO.Delivery updated = last with
        {
            CourierId = last.CourierId,
            DeliveryType = last.DeliveryType,
            DistanceKm = last.DistanceKm,
            Id = last.Id,
            OrderId = last.OrderId,
            StartTime = last.StartTime,
            DeliveryEndType = (DO.DeliveryEndType)deliveryEndType,
            EndTime = AdminManager.Now
        };

        lock (AdminManager.BlMutex)
            s_dal.Delivery.Update(updated);
        Observers.NotifyItemUpdated(orderId);
        Observers.NotifyListUpdated();
    }

    internal static void PeriodicSimulationChecks(DateTime oldClock, DateTime newClock)
    {
        Observers.NotifyListUpdated();
    }

    #endregion
    // ============================================================


    // ============================================================
    #region QUERIES (SUMMARY / LIST / OPEN / CLOSED)
    // ============================================================

    internal static int[][] GetOrdersAmountSummary()
    {
        var orders = LoadAllOrders()
            .Select(BuildBoOrderInList);

        int statusCount = Enum.GetValues<OrderStatus>().Length;
        int scheduleCount = Enum.GetValues<ScheduleStatus>().Length;

        int[][] matrix = new int[statusCount][];
        for (int i = 0; i < statusCount; i++)
            matrix[i] = new int[scheduleCount];

        var grouped =
            orders.GroupBy(o => new { o.OrderStatus, o.ScheduleStatus })
                  .Select(g => new { g.Key.OrderStatus, g.Key.ScheduleStatus, Count = g.Count() });

        foreach (var g in grouped)
        {
            matrix[(int)g.OrderStatus][(int)g.ScheduleStatus] = g.Count;
        }

        return matrix;

    }

    internal static IEnumerable<OrderInList> GetOrdersList(
    OrderStatus? statusFilter,
    object? selector,
    ScheduleStatus? scheduleFilter)
    {
        var query = LoadAllOrders()
            .Select(order => BuildBoOrderInList(order))
            .AsEnumerable();

        // 1. סינון לפי סטטוס הזמנה
        if (statusFilter != null)
            query = query.Where(o => o.OrderStatus == statusFilter.Value);

        // 2. סינון לפי סטטוס זמן
        if (scheduleFilter != null)
            query = query.Where(o => o.ScheduleStatus == scheduleFilter.Value);

        // 3. selector logic
        if (selector != null)
        {
            switch (selector)
            {
                case int id:
                    query = query.Where(o => o.OrderId == id);
                    break;

                case double airDistance:
                    query = query.Where(o => o.AirDistance == airDistance);
                    break;

                case BO.OrderType boType:
                    query = query.Where(o => o.OrderType == boType);
                    break;

                case DO.OrderType doType:
                    query = query.Where(o => o.OrderType == (BO.OrderType)doType);
                    break;

                // אפשרויות נוספות (לפי המסמך)
                default:
                    break;
            }
        }


        return query;

    }

    internal static async Task<BO.Order> GetOrderDetails(int orderId)
        => await BuildBoOrder(LoadDoOrder(orderId));

    internal static IEnumerable<ClosedDeliveryInList> GetClosedOrders(
        int courierId,
        BO.OrderType? filterBy,
        ClosedDeliveryField? sortBy)
    {
        var deliveries = LoadAllDeliveries() // getting all deliveries per delivery id
            .Where(d => d.CourierId == courierId && d.DeliveryEndType != null);

        var query = // bulild a list of orders per this delivery id in list
            from d in deliveries
            let o = LoadDoOrder(d.OrderId)
            select BuildBoClosedDeliveryInList(d, o);

        if (filterBy != null)
            query = query.Where(o => o.OrderType == filterBy);

        if (sortBy != null)
        {
            query = sortBy.Value switch
            {
                ClosedDeliveryField.DeliveryId => query.OrderBy(o => o.DeliveryId),
                ClosedDeliveryField.OrderId => query.OrderBy(o => o.OrderId),
                ClosedDeliveryField.OrderType => query.OrderBy(o => o.OrderType),
                ClosedDeliveryField.Address => query.OrderBy(o => o.Address),
                ClosedDeliveryField.ShipmentType => query.OrderBy(o => o.ShipmentType),
                ClosedDeliveryField.ActualDistance => query.OrderBy(o => o.ActualDistance),
                ClosedDeliveryField.ProcessingDuration => query.OrderBy(o => o.ProcessingDuration),
                ClosedDeliveryField.FinishType => query.OrderBy(o => o.FinishType),
                _ => query
            };
        }
        else
            query = query.OrderBy(query => query.FinishType);

        return query;

    }

    internal static async Task<IEnumerable<OpenOrderInList>> GetOpenOrders(
    int courierId,
    BO.OrderType? filterBy,
    OpenDeliveryField? sortBy)
    {
        var orders = LoadAllOrders().ToList();

        var thisCourier = await CourierManager.GetCourier(courierId);

        var tasks =
            from o in orders
            where (CalculateOrderStatus(o.Id) == OrderStatus.Open) &&
                  CalculateAirDistance(o) < thisCourier!.MaxDistance
            select BuildBoOpenOrderInList(o, courierId);


        var result = await Task.WhenAll(tasks);

        IEnumerable<OpenOrderInList> query = result;


        if (filterBy != null)
            query = query.Where(o => o.OrderType == filterBy);


        if (sortBy != null)
        {
            query = sortBy.Value switch
            {
                OpenDeliveryField.CourierId => query.OrderBy(o => o.CourierId),
                OpenDeliveryField.OrderId => query.OrderBy(o => o.OrderId),
                OpenDeliveryField.OrderType => query.OrderBy(o => o.OrderType),
                OpenDeliveryField.Weight => query.OrderBy(o => o.Weight),
                OpenDeliveryField.Volume => query.OrderBy(o => o.Volume),
                OpenDeliveryField.AirDistance => query.OrderBy(o => o.AirDistance),
                OpenDeliveryField.ActualDistance => query.OrderBy(o => o.ActualDistance),
                OpenDeliveryField.ActualTravelTime => query.OrderBy(o => o.EstimatedArrivalTime),
                OpenDeliveryField.ScheduleStatus => query.OrderBy(o => o.ScheduleStatus),
                OpenDeliveryField.TimeLeftToDeadline => query.OrderBy(o => o.TimeLeftToDeadline),
                OpenDeliveryField.EstimatedArrivalTime => query.OrderBy(o => o.MaxTimeToArrival),
                _ => query
            };
        }
        else query = query.OrderBy(o => o.ScheduleStatus);


        return query;

    }

    #endregion
    // ============================================================
}
