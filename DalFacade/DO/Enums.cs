namespace DO;

/// <summary>
/// Type of transportation required for delivery
/// </summary>
public enum ShipmentType
{
    Foot =0,
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