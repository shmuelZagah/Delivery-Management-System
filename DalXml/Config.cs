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
        internal const string s_students_xml = "students.xml";
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

        internal static void Reset()
        {
            NextCourseId = 1000
             //... 
  Clock = DateTime.Now;
            דומע 7 ךותמ 16
       
  //... 

}
}
