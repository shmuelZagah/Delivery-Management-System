using System;
using System.Runtime.CompilerServices;

namespace Dal;

internal static class Config
{
    // --- ID GENERATORS ---
    internal const int startDeliveryId = 0;

    internal const int startOrderId = 0;
    private static int s_nextOrderId = startOrderId;
    internal static int NextOrderId => s_nextOrderId++;

    private static int s_nextDeliveryId = startDeliveryId;
    internal static int NextDeliveryId => s_nextDeliveryId++;

    internal const int startAddressId = 0;
    private static int s_nextAddressId = startAddressId;
    internal static int NextAddressId =>  s_nextAddressId++;

    // --- SYSTEM DATA ---

    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    } = DateTime.Now;  // Current simulation or system time
   
    //private static int _managerId = 222222222;
    internal static int ManagerId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get;

        [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    } = 222222222;                // Manager identification

    //private static string _managerPass = "admin100";
    internal static string ManagerPass
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    } = "admin100" ;         // Manager password or passcode

    // --- COMPANY DETAILS ---
    internal static string? CompanyAddress
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    } = null;  // Company main address
    internal static double? Latitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    } = null;        // Latitude coordinate
    internal static double? Longitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    } = null;       // Longitude coordinate
    internal static double? MaxAirRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    } = null;     // Max allowed air distance for deliveries

    // --- AVERAGE SPEEDS ---
    internal static double AvgCarSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    }              // Average car speed (km/h)
    internal static double AvgMotocyclerSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    }       // Average motorcycle speed (km/h)
    internal static double AvgBicycleSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    }          // Average Bicycle speed (km/h)
    internal static double AvgWalkingSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    }          // Average walking speed (km/h)

    // --- TIME RANGES ---
    internal static TimeSpan maxSupplayTime
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    }         // Maximum allowed delivery time
    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    }              // Time range for potential delivery risks
    internal static TimeSpan UnactivityRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    }        // Time range for inactivity detection


    [MethodImpl(MethodImplOptions.Synchronized)]
    // --- RESET FUNCTION ---
    /// <summary>
    /// Resets all configuration values to their default state.
    /// </summary>
    /// 
    internal static void Reset()
    {
        // Reset IDs
        s_nextOrderId = startOrderId;
        s_nextDeliveryId = startDeliveryId;

        // Reset system time
        Clock = DateTime.Now;

        // Reset company details
        CompanyAddress = null;
        Latitude = null;
        Longitude = null;
        MaxAirRange = null;

        // Reset average speeds
        AvgCarSpeed = 0;
        AvgMotocyclerSpeed = 0;
        AvgWalkingSpeed = 0;

        // Reset time ranges
        maxSupplayTime = TimeSpan.Zero;
        RiskRange = TimeSpan.Zero;
        UnactivityRange = TimeSpan.Zero;

        // Reset manager data
        ManagerId = 222222222; //for testing purposes
        ManagerPass = "admin100";
    }
}
