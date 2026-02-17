
namespace Dal;
using DalApi;
using DO;
using System;

internal class ConfigImplementation : IConfig
{
    // --- SYSTEM DATA ---
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public int ManagerId
    {
        get => Config.ManagerId;
        set => Config.ManagerId = value;
    }
    public string ManagerPass
    {
        get => Config.ManagerPass;
        set => Config.ManagerPass = value;
    }
    public string? CompanyAddress
    {
        get => Config.CompanyAddress;
        set => Config.CompanyAddress = value;
    }
    public double? Latitude
    {
        get => Config.Latitude;
        set => Config.Latitude = value;
    }
    public double? Longitude
    {
        get => Config.Longitude;
        set => Config.Longitude = value;
    }
    public double? MaxAirRange
    {
        get => Config.MaxAirRange;
        set => Config.MaxAirRange = value;
    }
    public double AvgCarSpeed
    {
        get => Config.AvgCarSpeed;
        set => Config.AvgCarSpeed = value;
    }
    public double AvgMotorcycleSpeed
    {
        get => Config.AvgMotocyclerSpeed;
        set => Config.AvgMotocyclerSpeed = value;
    }
    public double AvgBicycleSpeed
    {
        get => Config.AvgBicycleSpeed;
        set => Config.AvgBicycleSpeed = value;
    }
    public double AvgWalkingSpeed
    {
        get => Config.AvgWalkingSpeed;
        set => Config.AvgWalkingSpeed = value;
    }
    public TimeSpan maxSupplayTime
    {
        get => Config.maxSupplayTime;
        set => Config.maxSupplayTime = value;
    }
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    public TimeSpan UnactivityRange
    {
        get => Config.UnactivityRange;
        set => Config.UnactivityRange = value;
    }

    public void Reset()
    {
        Config.Reset();
    }
}
