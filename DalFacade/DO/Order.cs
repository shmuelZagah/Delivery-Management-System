namespace DO;

/// <summary>
/// Order Entity represents a delivery order with all its properties.
/// </summary>
/// <param name="Id">Personal unique ID of the order (running ID start from 0,main key).</param>
/// <param name="OrderType">Type of the order (enum).</param>
/// <param name="Description">Short textual description of the order.</param>
/// <param name="FullAddress">Full address of the order destination.</param>
/// <param name="Latitude">Latitude of the order address.</param>
/// <param name="Longitude">Longitude of the order address.</param>
/// <param name="CustomerName">Full name of the customer.</param>
/// <param name="CustomerPhone">Mobile phone number of the customer.</param>
/// <param name="Notes">Additional details about the order.</param>
/// <param name="OrderCreationTime">Time when the order was created.</param>
public record Order
(
    int Id,
    OrderType OrderType,
    string? Description,
    string FullAddress,
    double Latitude,
    double Longitude,
    string CustomerName,
    string CustomerPhone,
    string? Notes,
    DateTime OrderCreationTime
)
{
    /// <summary>
    /// Default constructor, initializes with neutral defaults.
    /// </summary>
    public Order()
        : this(
            0,                   // Id
            OrderType.Standard,  // OrderType
            null,                // Description
            "",                  // FullAddress
            0.0,                 // Latitude
            0.0,                 // Longitude
            "",                  // CustomerName
            "",                  // CustomerPhone
            null,                // Details
            DateTime.Now         // OrderCreationTime
        )
    { }
}


