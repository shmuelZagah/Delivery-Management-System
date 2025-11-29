using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class OrderInProgress
    {
        public int DeliveryId { get; set; }              // מספר מזהה של שיטת המשלוח
        public int OrderId { get; set; }                 // מספר מזהה של שיטת הזמנה
        public OrderType OrderType { get; set; }         // ENUM - סוג הזמנה
        public string? CustomerDescription { get; set; }  // תיאור מילולי
        public string CustomerAddress { get; set; }      // כתובת מלאה של ההזמנה

        public double AirDistance { get; set; }          // מרחק אווירי בק"מ
        public double? RealDistance { get; set; }         // מרחק בפועל בק"מ

        public string CustomerName { get; set; }         // שם מלא של המזמין
        public string CustomerPhone { get; set; }        // טלפון של המזמין

        public DateTime OrderCreated { get; set; }       // זמן פתיחת הזמנה
        public DateTime DeliveryStart { get; set; }      // זמן תחילת משלוח

        public DateTime ExpectedArrivalTime { get; set; }   // זמן אספקה צפוי
        public DateTime LastArrivalTime { get; set; }       // זמן אספקה מריבי (ללא התחשבות ברוט)

        public OrderStatus OrderStatus { get; set; }        // סטטוס הזמנה
        public ScheduleStatus ScheduleStatus { get; set; }  // סטטוס עמידה בזמנים

        public TimeSpan TimeGap { get; set; }               // פער זמן שנותר לסיום ההזמנה
    }
}
