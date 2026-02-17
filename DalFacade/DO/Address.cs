namespace DO;

/// <summary>
/// address for auto-complit
/// </summary>
/// <param name="address">address in the format: name of street int num, country</param>
/// <param name="Latitude">Latitude of the order address.</param>
/// <param name="Longitude">Longitude of the order address.</param>
public record Address
(
    int Id,
    string FullAddress,
    double Latitude,
    double Longitude
);

