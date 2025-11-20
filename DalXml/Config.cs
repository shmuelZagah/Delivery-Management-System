using System;
using System.Collections.Generic;
using System.Linq;
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

        internal const int startDeliveryId = 0;

        internal const int startOrderId = 0;
        //...  

        internal static int NextOrderId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextOrderId");
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextOrderId", value);
        }

        internal static int NextDeliveryId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextDeliveryId");
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextDeliveryId", value);
        }



        // --- SYSTEM DATA ---
        internal static DateTime Clock
        {
            get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
            set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
        }

        internal static int ManagerId
        {
            get => XMLTools.GetConfigIntVal(s_data_config_xml, "ManagerId");
            set => XMLTools.SetConfigIntVal(s_data_config_xml, "ManagerId", value);
        }

        internal static string ManagerPass
        {
            get => XMLTools.GetConfigStringVal(s_data_config_xml, "ManagerPass") ?? "";
            set => XMLTools.SetConfigStringVal(s_data_config_xml, "ManagerPass", value);
        }


        // --- COMPANY DETAILS ---
        internal static string? CompanyAddress
        {
            get => XMLTools.GetConfigStringVal(s_data_config_xml, "CompanyAddress");
            set => XMLTools.SetConfigStringVal(s_data_config_xml, "CompanyAddress", value);
        }

        internal static double? Latitude
        {
            get => XMLTools.GetConfigDoubleNullable(s_data_config_xml, "Latitude");
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Latitude", value);
        }

        internal static double? Longitude
        {
            get => XMLTools.GetConfigDoubleNullable(s_data_config_xml, "Longitude");
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Longitude", value);
        }

        internal static double? MaxAirRange
        {
            get => XMLTools.GetConfigDoubleNullable(s_data_config_xml, "MaxAirRange");
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "MaxAirRange", value);
        }


        // --- AVERAGE SPEEDS ---
        internal static double AvgCarSpeed
        {
            get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed");
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed", value);
        }

        internal static double AvgMotocyclerSpeed
        {
            get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgMotocyclerSpeed");
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgMotocyclerSpeed", value);
        }

        internal static double AvgBicycleSpeed
        {
            get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed");
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed", value);
        }

        internal static double AvgWalkingSpeed
        {
            get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed");
            set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed", value);
        }


        // --- TIME RANGES ---
        internal static TimeSpan maxSupplayTime
        {
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "maxSupplayTime");
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "maxSupplayTime", value);
        }

        internal static TimeSpan RiskRange
        {
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
        }

        internal static TimeSpan UnactivityRange
        {
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "UnactivityRange");
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "UnactivityRange", value);
        }

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
