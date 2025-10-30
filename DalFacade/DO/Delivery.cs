namespace DO;

/// <summary>
/// Delivery Entity represents a delivery handled by a courier.
/// </summary>
/// <param name="Id">Unique ID of the delivery entity (running ID) (main key).</param>
/// <param name="OrderId">Unique ID of the order being delivered (same Id of the Order.id).</param>
/// <param name="CourierId">ID of the courier handling the delivery.</param>
/// <param name="DeliveryType">Type of delivery (enum).</param>
/// <param name="StartTime">Time when the delivery started.</param>
/// <param name="DistanceKm">Actual distance traveled in km (nullable).</param>
/// <param name="DeliveryEndType">(Null=not end yet,Cancelled,Finished) (enum, nullable).</param>
/// <param name="EndTime">Time when the delivery ended (nullable).</param>
public record Delivery
(
    int Id,
    int OrderId,
    int CourierId,
    DeliveryType DeliveryType,
    DateTime StartTime,
    double? DistanceKm,
    DeliveryEndType? DeliveryEndType,
    DateTime? EndTime
)
{
    /// <summary>
    /// Default constructor, initializes with neutral defaults.
    /// </summary>
    public Delivery()
        : this(
            0,                      // Id
            0,                      // OrderId
            0,                      // CourierId
            DeliveryType.Standard,  // DeliveryType
            DateTime.Now,           // StartTime
            null,                   // DistanceKm
            null,                   // DeliveryEndType
            null                    // EndTime
        )
    { }
}

