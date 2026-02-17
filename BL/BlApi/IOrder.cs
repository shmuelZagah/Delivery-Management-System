using BO;

namespace BlApi;

public interface IOrder : BlApi.IObservable
{


    int[][] GetOrdersAmountSummary(int requesterId);

    public IEnumerable<BO.OrderInList> GetOrdersList(
         int requesterId,
         BO.OrderStatus? statusFilter,      // Nullable filter for OrderStatus
         object? selector,               // Optional filter by dynamic type (ID, phone, date, etc.)
         BO.ScheduleStatus? scheduleFilter);
    Task<BO.Order?> GetOrderDetails(int requesterId, int orderId);

    void UpdateOrder(int requesterId, BO.Order order);

    void CancelOrder(int requesterId, int orderId);

    void DeleteOrder(int requesterId, int orderId);

    void AddOrder(int requesterId, BO.Order order);

    void CompleteOrderHandling(int requesterId, int courierId, int deliveryId);

    void ChooseOrderForHandling(int requesterId, int courierId, int orderId, double? distance = null);

    IEnumerable<BO.ClosedDeliveryInList> GetClosedOrders(int requesterId, int courierId, OrderType? filterBy, ClosedDeliveryField? sortBy);

    Task<IEnumerable<BO.OpenOrderInList>> GetOpenOrders(int requesterId, int courierId, OrderType? filterBy, OpenDeliveryField? sortBy);


    void DeliveryEnded(int requesterId, int orderId, BO.DeliveryEndType deliveryEndType);

    IEnumerable<string> GetSuggestions(string str);
}
