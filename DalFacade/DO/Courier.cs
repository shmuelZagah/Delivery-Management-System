// Module Courier.cs
#nullable enable
namespace DO;

/// <summary>
/// Courier Entity represents a delivery courier with all its properties.
/// </summary>
/// <param name="Id">Personal unique ID of the courier (internal numeric identifier).</param>
/// <param name="Name">Full name of the courier (first and last name).</param>
/// <param name="Phone">Mobile phone number of the courier.</param>
/// <param name="Email">Email address of the courier.</param>
/// <param name="Address">Residential (or primary) address of the courier.</param>
/// <param name="IsActive">Whether the courier is currently active at work (default true).</param>
/// <param name="PersonalMaxWeightKg">Courier's personal max allowed parcel weight in kilograms (nullable).</param>
/// <param name="PreferredShipmentType">Preferred shipment type for this courier (nullable).</param>
/// <param name="EmploymentStartTime">Employment start date/time in the company (nullable).</param>
public record Courier
(
    int Id,
    string Name,
    string Phone,
    string Email,
    string Address,
    bool IsActive = true,
    double? PersonalMaxWeightKg = null,
    ShipmentType? PreferredShipmentType = null,
    DateTime? EmploymentStartTime = null
)
{
    /// <summary>
    /// Default constructor for stage 3 (parameterless), initializes with neutral defaults.
    /// </summary>
    public Courier() : this(0, string.Empty, string.Empty, string.Empty, string.Empty) { }
}

/// <summary>
/// Shipment type enumeration for courier capabilities/preferences.
/// </summary>
public enum ShipmentType
{
    /// <summary>Standard delivery.</summary>
    Regular = 0,
    /// <summary>Express/priority delivery.</summary>
    Express = 1,
    /// <summary>Oversized/heavy items.</summary>
    Oversize = 2,
    /// <summary>Fragile items.</summary>
    Fragile = 3,
    /// <summary>Refrigerated/temperature-controlled items.</summary>
    Refrigerated = 4
}
