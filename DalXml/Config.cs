using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    internal static class Config
    {
        internal const string s_data_config_xml = "data-config.xml"; 
        internal const string s_couriers_xml = "couriers.xml";
        internal const string s_deliverys_xml = "deliverys.xml";
        internal const string s_orders_xml = "orders.xml";
        internal const string s_address_xml = "address.xml";

        internal const int startDeliveryId = 0;

        internal const int startOrderId = 0;
        internal const int startAddressId = 0;
        //...  

        internal static int NextOrderId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextOrderId");
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextOrderId", value);
        }

        internal static int NextDeliveryId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextDeliveryId");
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextDeliveryId", value);
        }

        internal static int NextAddressId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAddressId");

            [MethodImpl(MethodImplOptions.Synchronized)]
            private set=> XMLTools.SetConfigIntVal(s_data_config_xml, "NextAddressId", value);

        }



        // --- SYSTEM DATA ---
        internal static DateTime Clock
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
        }

        internal static int ManagerId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigIntVal(s_data_config_xml, "ManagerId");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigIntVal(s_data_config_xml, "ManagerId", value);
        }

        internal static string ManagerPass
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigStringVal(s_data_config_xml, "ManagerPass") ?? "";
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigStringVal(s_data_config_xml, "ManagerPass", value);
        }


        // --- COMPANY DETAILS ---
        internal static string? CompanyAddress
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigStringVal(s_data_config_xml, "CompanyAddress");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigStringVal(s_data_config_xml, "CompanyAddress", value);
        }

        internal static double? Latitude
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDoubleNullable(s_data_config_xml, "Latitude");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Latitude", value);
        }

        internal static double? Longitude
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDoubleNullable(s_data_config_xml, "Longitude");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Longitude", value);
        }

        internal static double? MaxAirRange
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDoubleNullable(s_data_config_xml, "MaxAirRange");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "MaxAirRange", value);
        }


        // --- AVERAGE SPEEDS ---
        internal static double AvgCarSpeed
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed", value);
        }

        internal static double AvgMotocyclerSpeed
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgMotocyclerSpeed");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgMotocyclerSpeed", value);
        }

        internal static double AvgBicycleSpeed
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed", value);
        }

        internal static double AvgWalkingSpeed
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed", value);
        }


        // --- TIME RANGES ---
        internal static TimeSpan maxSupplayTime
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "maxSupplayTime");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "maxSupplayTime", value);
        }

        internal static TimeSpan RiskRange
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
        }

        internal static TimeSpan UnactivityRange
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "UnactivityRange");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "UnactivityRange", value);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void Reset()
        {
            // Reset IDs
            NextOrderId = startOrderId;
            NextDeliveryId = startDeliveryId;

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
            ManagerPass = string.Empty;
        }

    }


}
