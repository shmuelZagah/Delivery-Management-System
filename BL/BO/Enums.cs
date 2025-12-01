namespace BO;

/// <summary>
/// Type of transportation required for delivery
/// </summary>
public enum ShipmentType
{
    Foot = 0,
    Car,
    Motorcycle,
    Bicycle
}

/// <summary>
/// Enum representing order types.
/// </summary>
public enum OrderType
{
    Standard = 0,
    Express,
    Fragile,
    Heavy
}

/// <summary>
/// Enum representing end type of delivery.
/// </summary>
public enum DeliveryEndType
{
    Provided = 0,
    ClientRefusedAccept,
    Cancelled,
    ClientNotFound,
    Failed
}

public enum OrderStatus
{
    Created,
    Assigned,
    InProgress,
    Completed,
    Cancelled
}

public enum ScheduleStatus
{
    OnTime,
    Late,
    VeryLate,
    Unknown
}


public enum DeliveryField
{
    DeliveryId,
    CourierId,
    CourierName,
    ShipmentType,
    StartDeliveryTime,
    FinishType,
    FinishTime
}
public enum CourierField
{
    Id,
    Name,
    Phone,
    Email,
    Password,
    IsActive,
    MaxDistance,
    ShipmentType,
    StartTime,
    OrdersDeliveredInTime,
    OrdersDeliveredAfterTime,
    InProgress
}

public enum OrderField
{
    OrderStatus,
    ScheduleStatus,
    OrderType,
    IsFragile,
    Volume,
    Weight,
    AirDistance,
    OrderCreated,
    ExpectedArrivalTime,
    LastArrivalTime,
    TimeLeftToDeadline,
    CustomerName,
    CustomerPhone
}
