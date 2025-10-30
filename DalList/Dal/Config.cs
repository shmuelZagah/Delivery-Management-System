using System;

namespace Dal.Dal
{
    internal static class Config
    {
        // --- ID GENERATORS ---
        internal const int startOrderId = 0;
        private static int s_nextOrderId = startOrderId;
        internal static int NextOrderId => s_nextOrderId++;

        internal const int startDeliveryId = 0;
        private static int s_nextDeliveryId = startDeliveryId;
        internal static int NextDeliveryId => s_nextDeliveryId++;

        // --- SYSTEM DATA ---
        internal static DateTime Clock { get; set; } = DateTime.Now;  // Current simulation or system time
        internal static int ManagerId { get; set; }                   // Manager identification
        internal static int ManagerPass { get; set; }                 // Manager password or passcode

        // --- COMPANY DETAILS ---
        internal static string? CompanyAddress { get; set; } = null;  // Company main address
        internal static double? Latitude { get; set; } = null;        // Latitude coordinate
        internal static double? Longitude { get; set; } = null;       // Longitude coordinate
        internal static double? MaxAirRange { get; set; } = null;     // Max allowed air distance for deliveries

        // --- AVERAGE SPEEDS ---
        internal static double AvgCarSpeed { get; set; }              // Average car speed (km/h)
        internal static double AvgMotocyclerSpeed { get; set; }       // Average motorcycle speed (km/h)
        internal static double AvgWalkingSpeed { get; set; }          // Average walking speed (km/h)

        // --- TIME RANGES ---
        internal static TimeSpan maxSupplayTime { get; set; }         // Maximum allowed delivery time
        internal static TimeSpan RiskRange { get; set; }              // Time range for potential delivery risks
        internal static TimeSpan UnactivityRange { get; set; }        // Time range for inactivity detection

        // --- RESET FUNCTION ---
        /// <summary>
        /// Resets all configuration values to their default state.
        /// Used for testing or restarting the system with clean settings.
        /// </summary>
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
            ManagerId = 0;
            ManagerPass = 0;
        }
    }
}
