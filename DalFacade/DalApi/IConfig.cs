namespace DalApi;
public interface IConfig
{
    // --- SYSTEM DATA ---
    DateTime Clock { get; set; }
    int ManagerId { get; set; }
    string ManagerPass { get; set; }


    // --- COMPANY DETAILS ---
    string? CompanyAddress { get; set; }
    double? Latitude {  get; set; }
    double? Longitude { get; set; }
    double? MaxAirRange { get; set; }

    // --- AVERAGE SPEEDS ---
    double AvgCarSpeed { get; set; }
    double AvgMotorcycleSpeed { get; set; }
    double AvgBicycleSpeed {  get; set; }
    double AvgWalkingSpeed { get; set; }

    // --- TIME RANGES ---
    TimeSpan maxSupplayTime { get; set; }
    TimeSpan RiskRange { get; set; }
    TimeSpan UnactivityRange { get; set; }


    // --- FUNCTION ---
    void Reset();
}
