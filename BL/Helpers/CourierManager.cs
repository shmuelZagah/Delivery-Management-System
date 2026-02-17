using BlApi;
using BO;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;


namespace Helpers;

internal static class CourierManager
{
    private static DalApi.IDal s_dal = DalApi.Factory.Get;

    internal static ObserverManager Observers = new();

    #region Getters
    internal static async Task <BO.Courier?> GetCourier(int id)
    {
        DO.Courier? doCourier = null;

        lock (AdminManager.BlMutex)
            doCourier = s_dal.Courier.Read(id) ??
            throw new BO.BlDoesNotExistException($"Courier with ID={id} does Not exist");

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
            OrdersDeliveredInTime = await DeliveriesInTime(id),
            OrdersDeliveredAfterTime = await DeliveredLate(id),
            InProgress = await GetOrderInProgress(id),
        };

        return temp;

    }

    internal static IEnumerable<BO.CourierInList> GetAllCouriers(Func<DO.Courier, bool>? predicate = null)
    {
        IEnumerable<DO.Courier> doCouriers;
        IEnumerable<DO.Delivery> allDeliveries;
        IEnumerable<DO.Order> allOrders;
        lock (AdminManager.BlMutex)
        {
             doCouriers = s_dal.Courier.ReadAll(predicate);
             allDeliveries = s_dal.Delivery.ReadAll();
             allOrders = s_dal.Order.ReadAll();
        }

  

        TimeSpan maxSupplyTime = OrderManager.getMaxSupply();

        var boCouriers = from courier in doCouriers
                         let myDeliveries = allDeliveries.Where(d => d.CourierId == courier.Id)

                         let activeDelivery = myDeliveries.FirstOrDefault(d => d.EndTime == null)

                         let inTimeCount = myDeliveries.Count(d =>
                         {
                             if (d.EndTime == null || d.DeliveryEndType != DO.DeliveryEndType.Provided)
                                 return false;

                             var order = allOrders.FirstOrDefault(o => o.Id == d.OrderId);
                             if (order == null) return false;

                             return d.EndTime <= order.OrderCreationTime.Add(maxSupplyTime);
                         })

                         let lateCount = myDeliveries.Count(d =>
                         {
                             if (d.EndTime == null || d.DeliveryEndType != DO.DeliveryEndType.Provided)
                                 return false;

                             var order = allOrders.FirstOrDefault(o => o.Id == d.OrderId);
                             if (order == null) return false;

                             return d.EndTime > order.OrderCreationTime.Add(maxSupplyTime);
                         })

                         select new BO.CourierInList
                         {
                             Id = courier.Id,
                             FullName = courier.Name,
                             ShipmentType = (BO.ShipmentType)courier.PreferredShipmentType,
                             IsActive = courier.IsActive,
                             StartTime = courier.EmploymentStartTime,
                             CurrentOrderId = activeDelivery?.Id,
                             DeliveredOnTimeCount = inTimeCount,
                             DeliveredLateCount = lateCount
                         };

        return boCouriers.ToList();
    }


    internal static BO.UserType GetUserType(string id)
    {
        DO.Courier courier;
        int managerId;

    
        lock (AdminManager.BlMutex)
        {
            courier =  s_dal.Courier.Read(p => p.Id == int.Parse(id));
            managerId = s_dal.Config.ManagerId;
        }

        if (courier == null)
        {
            throw new BlDoesNotExistException($"User with Name={id} does not exist");
        }

        return (managerId == courier.Id) ? BO.UserType.Manager : BO.UserType.Courier;
    }

    #endregion

    #region Setters and Modifiers
    internal static void AddCourier(BO.Courier courier)
    {
        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Courier.Create(new DO.Courier()
            {
                Id = courier.Id,
                Name = courier.Name,
                Phone = courier.Phone,
                Email = courier.Email,
                Password = Helpers.Tools.HashPassword(courier.Password), 
                IsActive = courier.IsActive,
                PersonalMaxDistance = courier.MaxDistance,
                PreferredShipmentType = (DO.ShipmentType)courier.ShipmentType,
                EmploymentStartTime = Helpers.AdminManager.Now,
            });
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Courier with ID={courier.Id} already exists", ex);
        }

        Observers.NotifyListUpdated();
    }

    internal static async Task DeleteCourier(int id)
    {
        try
        {
            var courier = await Helpers.CourierManager.GetCourier(id);
            if (courier!.InProgress != null)
                throw new BO.BlInvalidOperationStateException("Cannot delete a courier with an order in progress");
            else if ((courier.OrdersDeliveredAfterTime + courier.OrdersDeliveredInTime) != 0)
                  throw new BO.BlInvalidOperationStateException("Cannot delete a courier with delivery history");
            lock (AdminManager.BlMutex)
                s_dal.Courier.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Courier with ID={id} does not exist", ex);
        }
        Observers.NotifyListUpdated();
    }

    internal static void UpdateCourier(BO.Courier boCourier)
    {
        // Ensure Validity of Input
        ValidationOfCarrier(boCourier);

        IEnumerable<DO.Delivery> allDeliverys;

        lock (AdminManager.BlMutex)
            allDeliverys = s_dal.Delivery.ReadAll();

        // Check if courier in the middle of a delivery when trying to deactivate
        var activeDeliveriesCheck =
            (from delivery in allDeliverys
             where delivery.CourierId == boCourier.Id && delivery.EndTime == null
             let isCourierBeingDeactivated = boCourier.IsActive == false
             where isCourierBeingDeactivated
             select delivery).ToList();

        if (activeDeliveriesCheck.Any())
        {
            throw new BO.BlInvalidOperationStateException($"Cannot deactivate courier {boCourier.Id} while they have active deliveries in progress.");
        }
        DO.Courier? courier;
        lock (AdminManager.BlMutex)
            courier = s_dal.Courier.Read(boCourier.Id);

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
            EmploymentStartTime = courier.EmploymentStartTime == boCourier.StartTime ? boCourier.StartTime
            : throw new BO.BlInvalidInputException("Cannot change start time"),
        };

        // Update in DAL
        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Courier.Update(doCourier);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Courier with ID={boCourier.Id} does not exist", ex);
        }

        Observers.NotifyItemUpdated(doCourier.Id);
        Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Updates courier activity status by deactivating couriers who have been inactive
    /// for longer than the configured inactivity range.
    /// </summary>
    internal static void UpdateCourierActivity()
    {
        DateTime cutoffTime;

        // Snapshot lists: we will read all required DAL data once (inside lock),
        // convert to in-memory Lists, and then work only on these snapshots.
        List<DO.Courier> activeCouriers;
        List<DO.Delivery> allDeliveries;

        // Read everything we need from DAL under one lock (stage 7 requirement)
        lock (AdminManager.BlMutex)
        {
            // Calculate the inactivity cutoff time using current "Now" and config
            cutoffTime = AdminManager.Now - s_dal.Config.UnactivityRange;

            // Snapshot of active couriers
            activeCouriers = s_dal.Courier
                .ReadAll(c => c.IsActive)
                .ToList();

            // Snapshot of deliveries (all deliveries once)
            // Note: if your DAL requires a predicate, adjust accordingly.
            allDeliveries = s_dal.Delivery
                .ReadAll()
                .ToList();
        }

        // Build an in-memory lookup: CourierId -> List of deliveries
        // This prevents repeated scans / repeated DAL calls.
        var deliveriesByCourier =
            allDeliveries
                .GroupBy(d => d.CourierId)
                .ToDictionary(g => g.Key, g => g.ToList());

        // Identify couriers to deactivate using ONLY the snapshot lists
        var couriersToDeactivate =
            (from courier in activeCouriers
                 // Get this courier's deliveries from the dictionary (or empty list if none)
             let deliveries = deliveriesByCourier.TryGetValue(courier.Id, out var list)
                                ? list
                                : new List<DO.Delivery>()
             // Must have no active deliveries (EndTime == null means still active)
             where !deliveries.Any(d => d.EndTime == null)
             // Compute last activity time based on last completed delivery,
             // or fallback to EmploymentStartTime if no completed deliveries exist
             let lastActivity = deliveries
                                    .Where(d => d.EndTime != null)
                                    .OrderByDescending(d => d.EndTime)
                                    .Select(d => d.EndTime)
                                    .FirstOrDefault() ?? courier.EmploymentStartTime
             // Inactive longer than allowed
             where lastActivity < cutoffTime
             select courier)
            .ToList(); // Materialize result as a list before updates

        // Deactivate each courier
        couriersToDeactivate.ForEach(c =>
        {
            // Create a new courier object with IsActive = false (same fields copied)
            DO.Courier updatedCourier = new DO.Courier
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Password = c.Password,
                IsActive = false,
                PersonalMaxDistance = c.PersonalMaxDistance,
                PreferredShipmentType = c.PreferredShipmentType,
                EmploymentStartTime = c.EmploymentStartTime
            };

            try
            {
                // DAL update must be protected by lock (stage 7 requirement)
                lock (AdminManager.BlMutex)
                    s_dal.Courier.Update(updatedCourier);

                // Notifications must be OUTSIDE the lock (per instructions)
                Observers.NotifyItemUpdated(c.Id);
            }
            catch (Exception ex)
            {
                // Keep your original behavior: wrap and throw
                throw new BO.BlDoesNotExistException($"Courier with ID={c.Id} does not exist", ex);
            }

            // Notify list update after each courier (same logic as original)
            Observers.NotifyListUpdated();
        });
    }




    /// <summary>
    /// Simulation logic: Updates delivery status based on clock time and cleans up inactive couriers.
    /// Called by AdminManager.UpdateClock.
    /// </summary>
    internal static void PeriodicSimulationChecks(DateTime oldClock, DateTime newClock)
    {
        UpdateCourierActivity(); 
        Observers.NotifyListUpdated();
    }
    #endregion

    private static readonly Random s_rand = new();
    private static readonly AsyncMutex s_simulationMutex = new(); // stage 7

    internal static async Task CourierSimulatorAsync()
    {
        if (s_simulationMutex.CheckAndSetInProgress())
            return;

        try
        {
            // 1) Query active couriers and IMMEDIATELY materialize with ToList()
            List<DO.Courier> activeCouriers;
            lock (AdminManager.BlMutex)
                activeCouriers = s_dal.Courier.ReadAll(c => c.IsActive).ToList();

            foreach (var courier in activeCouriers)
            {
                // 2) Tight lock: check if courier has an active delivery
                DO.Delivery? activeDelivery;
                lock (AdminManager.BlMutex)
                    activeDelivery = s_dal.Delivery
                        .ReadAll(d => d.CourierId == courier.Id && d.EndTime == null)
                        .OrderByDescending(d => d.Id)
                        .FirstOrDefault();

                // ==========================================================
                // CASE A: No active delivery => maybe pick a new order
                // ==========================================================
                if (activeDelivery == null)
                {
                    // 0.15 probability: courier is "available" now
                    if (s_rand.NextDouble() > 0.15)
                        continue;

                    // Heavy operation (network distance inside your GetOpenOrders):
                    // do it for ~1 courier per tick => already controlled by 0.15 probability above.
                    List<BO.OpenOrderInList> openOrders;
                    try
                    {
                        openOrders = (await OrderManager.GetOpenOrders(
                            courier.Id,
                            filterBy: null,
                            sortBy: null)).ToList();
                    }
                    catch
                    {
                        continue;
                    }

                    if (openOrders.Count == 0)
                        continue;

                    // 50% probability: he actually chooses an order
                    if (s_rand.NextDouble() > 0.50)
                        continue;

                    var chosen = openOrders[s_rand.Next(openOrders.Count)];
                    int orderId = chosen.OrderId;

                    bool created = false;

                    // Tight lock: DAL transaction (validate + create delivery)
                    lock (AdminManager.BlMutex)
                    {
                        // Re-check courier still active
                        var freshCourier = s_dal.Courier.Read(courier.Id);
                        if (freshCourier == null || !freshCourier.IsActive)
                        {
                            created = false;
                        }
                        else
                        {
                            // Ensure courier still has no active delivery
                            bool courierFree = !s_dal.Delivery
                                .ReadAll(d => d.CourierId == courier.Id && d.EndTime == null)
                                .Any();

                            // Ensure order not already in progress
                            var lastForOrder = s_dal.Delivery
                                .ReadAll(d => d.OrderId == orderId)
                                .OrderByDescending(d => d.Id)
                                .FirstOrDefault();

                            bool orderFree = (lastForOrder == null) || (lastForOrder.DeliveryEndType != null);

                            if (courierFree && orderFree)
                            {
                                // Use the already computed ActualDistance (no recalculation here)
                                var delivery = new DO.Delivery(
                                    Id: 0,
                                    OrderId: orderId,
                                    CourierId: courier.Id,
                                    DeliveryType: freshCourier.PreferredShipmentType,
                                    StartTime: AdminManager.Now,
                                    DistanceKm: chosen.ActualDistance,
                                    DeliveryEndType: null,
                                    EndTime: null
                                );

                                s_dal.Delivery.Create(delivery);
                                created = true;
                            }
                        }
                    }

                    // Notifications OUTSIDE lock
                    if (created)
                    {
                        OrderManager.Observers.NotifyItemUpdated(orderId);
                        OrderManager.Observers.NotifyListUpdated();

                        Observers.NotifyItemUpdated(courier.Id);
                        Observers.NotifyListUpdated();
                    }

                    continue;
                }

                // ==========================================================
                // CASE B: Has active delivery => maybe finish / cancel
                // ==========================================================

                // Decide if "enough time" passed based on distance + speed + random buffer
                double distanceKm = activeDelivery.DistanceKm ?? 0.0;
                double speedKmh = Tools.SpeedKmByType(activeDelivery.DeliveryType);

                var travel = TimeSpan.FromHours(distanceKm / Math.Max(speedKmh, 0.1));
                var buffer = TimeSpan.FromMinutes(s_rand.Next(2, 12)); // tune as you like
                var required = travel + buffer;

                var elapsed = AdminManager.Now - activeDelivery.StartTime;

                if (elapsed >= required)
                {
                    // Enough time: finish the delivery.
                    // We want variety in end types:
                    // - Mostly "Provided" (like CompleteOrderHandling),
                    // - Sometimes other end types using DeliveryEnded.
                    bool finished = false;
                    int orderId = activeDelivery.OrderId;

                    // Pick random end type (variety)
                    var endType = RandomSimEndType();

                    try
                    {
                        if (endType == BO.DeliveryEndType.Provided)
                        {
                            // "Courier pressed finish handling" (your existing logic)
                            OrderManager.CompleteOrderHandling(activeDelivery.CourierId, activeDelivery.Id);
                        }
                        else
                        {
                            // Variety: end with a different finish type
                            OrderManager.DeliveryEnded(orderId, endType);
                        }

                        finished = true;
                    }
                    catch
                    {
                        finished = false;
                    }

                    if (finished)
                    {
                        // Notifications already happen inside OrderManager methods,
                        // but if you want to ensure courier list refresh too:
                        Observers.NotifyItemUpdated(courier.Id);
                        Observers.NotifyListUpdated();
                    }
                }
                else
                {
                    // Not enough time: 10% admin cancels handling
                    if (s_rand.NextDouble() <= 0.10)
                    {
                        bool canceled = false;
                        int orderId = activeDelivery.OrderId;

                        try
                        {
                            // Closest to "admin cancels handling" in your current API:
                            // if order is InProgress, your CancelOrder updates the last delivery to Canceled.
                            OrderManager.CancelOrder(orderId);
                            canceled = true;
                        }
                        catch
                        {
                            canceled = false;
                        }

                        if (canceled)
                        {
                            Observers.NotifyItemUpdated(courier.Id);
                            Observers.NotifyListUpdated();
                        }
                    }
                }
            }
        }
        finally
        {
            s_simulationMutex.UnsetInProgress();
        }
    }

    /// <summary>
    /// Choose a random end type with variety.
    /// Adjust probabilities as you like.
    /// </summary>
    private static BO.DeliveryEndType RandomSimEndType()
    {
        double r = s_rand.NextDouble();

        // Mostly successful delivery
        if (r < 0.80) return BO.DeliveryEndType.Provided;

        // Some variety
        if (r < 0.90) return BO.DeliveryEndType.ClientNotFound;
        if (r < 0.97) return BO.DeliveryEndType.Failed;

        return BO.DeliveryEndType.ClientRefusedAccept;
    }



    #region Validation Methods

    internal static bool EnsureIsManager(string id)
    {
        lock (AdminManager.BlMutex)
            return s_dal.Config.ManagerId == int.Parse(id);
    }

    internal static bool EnsureIsManager(int id)
    {
        lock (AdminManager.BlMutex)
            return s_dal.Config.ManagerId == id;
    }

    /// <summary>
    /// Input Validity Check
    /// </summary>
    /// <returns>true if all valid</returns>
    internal static bool ValidationOfCarrier(BO.Courier courier)
    {
        Helpers.Tools.IdValidtion(courier.Id);
        if (courier.Phone.Length != 10 || courier.Phone[0] != '0' || courier.Phone[1] != '5')
            throw new BlInvalidInputException("Phone number mast at format 05XXXXXXXX");
        Helpers.Tools.IsEmailValidManual(courier.Email);
        if (courier.MaxDistance > AdminManager.GetConfig().MaxAirRange)
            throw new BlInvalidInputException($"Max distance cannot exceed {AdminManager.GetConfig().MaxAirRange} km");

        return true;
    }

    #endregion



    #region private
    private static async Task<BO.OrderInProgress?> GetOrderInProgress(int courierId)
    {
        // 1. Find the courier's active delivery (delivery that hasn't ended yet)
        DO.Delivery? activeDoDelivery;

        lock (AdminManager.BlMutex)
            activeDoDelivery = s_dal.Delivery.ReadAll()
            .FirstOrDefault(d => d.CourierId == courierId && d.EndTime == null);

        if (activeDoDelivery == null) return null;

        // 2. Retrieve Order and Courier details from DAL
        DO.Order? orderDo;
        lock (AdminManager.BlMutex)
            orderDo = s_dal.Order.Read(activeDoDelivery.OrderId);
        if (orderDo == null) return null;

        DO.Courier courier;
        lock (AdminManager.BlMutex)
            courier = s_dal.Courier.Read(courierId)!;

        // --- Safe Calculations (Prevent Crash on Null Distance) ---
        double distance = Tools.CalculateAirDistance(orderDo.Latitude, orderDo.Longitude);
        double speed = Tools.SpeedKmByType(courier.PreferredShipmentType);

        double travelHours = (speed > 0) ? distance / speed : 0;


        (double compLat, double compLon) = OrderManager.getConfigLatLon();
        var actualDistance = activeDoDelivery.DistanceKm ??
                      await Tools.GetTravelDistance(compLat, compLon, orderDo.Latitude, orderDo.Longitude, courier.PreferredShipmentType);

        var timeGap = orderDo.OrderCreationTime.Add(OrderManager.getMaxSupply()) - AdminManager.Now;


        ScheduleStatus scheduleStatus;
        lock (AdminManager.BlMutex)
            scheduleStatus = timeGap.TotalHours switch
        {
            var h when h < 0 => BO.ScheduleStatus.Late,
            var h when h > s_dal.Config.RiskRange.TotalHours => BO.ScheduleStatus.OnTime,
            _ => BO.ScheduleStatus.InRisk
        };


        return new BO.OrderInProgress
        {
            DeliveryId = activeDoDelivery.Id,
            OrderId = orderDo.Id,
            OrderType = (BO.OrderType)orderDo.OrderType,
            CustomerDescription = orderDo.Description,
            CustomerAddress = orderDo.FullAddress,

            AirDistance = distance,
            RealDistance = actualDistance,

            CustomerName = orderDo.CustomerName,
            CustomerPhone = orderDo.CustomerPhone,

            OrderCreated = orderDo.OrderCreationTime,
            DeliveryStart = activeDoDelivery.StartTime,

            // Use the safely calculated travelHours
            ExpectedArrivalTime = activeDoDelivery.StartTime.AddHours(travelHours),

            
            LastArrivalTime = activeDoDelivery.StartTime.Add(OrderManager.getMaxSupply()),

            OrderStatus = BO.OrderStatus.InProgress,
            ScheduleStatus = scheduleStatus,
            TimeGap = timeGap,
        };
    }


    private static async Task<int> DeliveriesInTime(int id)
    {
        List<DO.Delivery>? deliveries;

        lock (AdminManager.BlMutex)
            deliveries =
            s_dal.Delivery.ReadAll(p => p.CourierId == id)
            .Where(item => item.EndTime != null && item.DeliveryEndType == DO.DeliveryEndType.Provided)
            .ToList();

        var orders = await Task.WhenAll(deliveries.Select(d => Helpers.OrderManager.GetOrderDetails(d.OrderId)));

        return orders.Count(o => o != null && o.ScheduleStatus == BO.ScheduleStatus.OnTime);
    }


    private static async Task<int> DeliveredLate(int id)
    {
        List<DO.Delivery>? deliveries;
        lock (AdminManager.BlMutex)
            deliveries =
            s_dal.Delivery.ReadAll(p => p.CourierId == id)
            .Where(item => item.EndTime != null && item.DeliveryEndType == DO.DeliveryEndType.Provided)
            .ToList();

        var orders = await Task.WhenAll(deliveries.Select(d => Helpers.OrderManager.GetOrderDetails(d.OrderId)));

        return orders.Count(o => o != null && o.ScheduleStatus == BO.ScheduleStatus.Late);
    }


    internal static bool EnsureIsCourierOrManager(int requesterId)
    {
        lock (AdminManager.BlMutex)
            if (EnsureIsManager(requesterId.ToString()) || s_dal.Courier.Read(requesterId) != null)
            return true;

        return false;
    }

    #endregion
}

















