
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
    public string? CompanyAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double? Latitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double? Longitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double? MaxAirRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgCarSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgMotocyclerSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgBicycleSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgWalkingSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TimeSpan maxSupplayTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TimeSpan RiskRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TimeSpan UnactivityRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}
