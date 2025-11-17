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
        //...  

        internal static int NextCourseId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCourseId");
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCourseId", value);
        }

        //...  

        internal static DateTime Clock
        {
            get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
            set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
        }
    }


    }
