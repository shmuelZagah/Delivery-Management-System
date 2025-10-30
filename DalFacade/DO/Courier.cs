// Module Courier.cs
#nullable enable
namespace DO;

/// <summary>
/// Courier Entity represents a delivery courier with all its properties.
/// </summary>
/// <param name="Id">Personal unique ID of the courier (as in national id card) (main key).</param>
/// <param name="Name">Full name of the courier (first and last name) (second key).</param>
/// <param name="Phone">Mobile phone number of the courier.</param>
/// <param name="Email">Email address of the courier.</param>
/// <param name="Password">Password of the courier to the system </param>
/// <param name="Address">Residential (or primary) address of the courier.</param>
/// <param name="IsActive">Is the courier still working for the company?.</param>
/// <param name="PersonalMaxWeightKg">Courier's personal max allowed parcel weight in kilograms (nullable).</param>
/// <param name="PreferredShipmentType">Preferred shipment type for this courier.</param>
/// <param name="EmploymentStartTime">Employment start date/time in the company.</param>
public record Courier
(
    int Id,
    string Name,
    string Phone,
    string Email,
    string Password,
    string Address,
    bool IsActive,
    double? PersonalMaxWeightKg,
    ShipmentType PreferredShipmentType,
    DateTime EmploymentStartTime
)
{
    /// <summary>
    /// Default constructor , initializes with neutral defaults.
    /// </summary>
    public Courier() : this(
        0,                  //Id
        "",                 //Name
        "",                 //Phone
        "",                 //Email
        "",                 //Password
        "",                 //Address
        false,              //IsActive
        null,               //PersonalMaxWeightKg
        ShipmentType.Foot,  //ShipmentType
        DateTime.Now        //EmploymentStartTime
        )
    { }

}

