using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class OpenOrderInList
    {

        public int? CourierId { get; }
        public int OrderId { get; }
        public OrderType OrderType { get; }
        public double Weight { get; }
        public double Volume { get; }
        public double AirDistance { get; }
        public double? ActualDistance { get; }
        public TimeSpan? ActualTravelTime { get; }
        public ScheduleStatus ScheduleStatus { get; }
        public TimeSpan TimeLeftToDeadline { get; }
        public DateTime EstimatedArrivalTime { get; }


    }

   
    
    }

