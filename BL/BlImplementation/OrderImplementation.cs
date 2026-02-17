using BlApi;
using BO;
using DO;
using Helpers;
using System.Collections.Generic;

namespace BlImplementation;

/// <summary>
/// Thin BL service implementation for Orders.
/// All logic is delegated to Helpers.OrderManager.
/// This class handles only authorization and IOrder interface wiring.
/// </summary>
internal class OrderImplementation : IOrder
{
    // -----------------------------
    #region access functions
    // -----------------------------

    private static void EnsureManager(int requesterId)
    {
        if (!CourierManager.EnsureIsManager(requesterId.ToString()))
            throw new BLUnauthorizedAccessException("Only manager can perform this action.");
    }

    private static void EnsureCourierOrManager(int requesterId)
    {
        if (!CourierManager.EnsureIsCourierOrManager(requesterId))
            throw new BLUnauthorizedAccessException("Only the courier or manager can perform this action.");
    }

    #endregion

    // -----------------------------
    #region Observer functions
    // -----------------------------

    public void AddObserver(Action listObserver) =>
    OrderManager.Observers.AddListObserver(listObserver);
    public void AddObserver(int id, Action observer) =>
   OrderManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(Action listObserver) =>
   OrderManager.Observers.RemoveListObserver(listObserver);
    public void RemoveObserver(int id, Action observer) =>
   OrderManager.Observers.RemoveObserver(id, observer);


    #endregion
    // -----------------------------


    //------------------------------
    #region IOrder functions
    //------------------------------

    public int[][] GetOrdersAmountSummary(int requesterId)
    {

        EnsureManager(requesterId);
        return OrderManager.GetOrdersAmountSummary();
    }

    public IEnumerable<OrderInList> GetOrdersList(
         int requesterId,
         OrderStatus? statusFilter,
         object? selector,
         ScheduleStatus? scheduleFilter)
    {
        EnsureManager(requesterId);
        return OrderManager.GetOrdersList(statusFilter, selector, scheduleFilter);
    }

    public async Task <BO.Order?> GetOrderDetails(int requesterId, int orderId)
    {
        EnsureManager(requesterId);
        return await OrderManager.GetOrderDetails(orderId);
    }

    public void UpdateOrder(int requesterId, BO.Order order)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        EnsureManager(requesterId);
        OrderManager.UpdateOrder(order);
    }

    public void CancelOrder(int requesterId, int orderId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        EnsureManager(requesterId);
        OrderManager.CancelOrder(orderId);
    }

    public void DeleteOrder(int requesterId, int orderId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        EnsureManager(requesterId);
        OrderManager.DeleteOrder(orderId);
    }

    public void AddOrder(int requesterId, BO.Order order)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        EnsureManager(requesterId);
        OrderManager.AddOrder(order);
    }

    public void CompleteOrderHandling(int requesterId, int courierId, int deliveryId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        EnsureCourierOrManager(requesterId);
        OrderManager.CompleteOrderHandling(courierId, deliveryId);
    }

    public void ChooseOrderForHandling(int requesterId, int courierId, int orderId, double? distance = null)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        EnsureCourierOrManager(requesterId);
        OrderManager.ChooseOrderForHandling(courierId, orderId, distance);
    }

    public IEnumerable<ClosedDeliveryInList> GetClosedOrders(
        int requesterId,
        int courierId,
        BO.OrderType? filterBy,
        BO.ClosedDeliveryField? sortBy)
    {
        EnsureCourierOrManager(requesterId);
        return OrderManager.GetClosedOrders(courierId, filterBy, sortBy);
    }

    public async Task<IEnumerable<OpenOrderInList>> GetOpenOrders(
        int requesterId,
        int courierId,
        BO.OrderType? filterBy,
        OpenDeliveryField? sortBy)
    {
        EnsureCourierOrManager(requesterId);
        return await OrderManager.GetOpenOrders(courierId, filterBy, sortBy);
    }

    public void DeliveryEnded(int requesterId, int orderId, BO.DeliveryEndType deliveryEndType)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        EnsureCourierOrManager(requesterId);
        OrderManager.DeliveryEnded(orderId, deliveryEndType);
    }

    public IEnumerable<string> GetSuggestions(string str)
    {
        return AdminManager.Addresses.GetAllWithPrefix(str);
    }

    #endregion
    //------------------------------
}

