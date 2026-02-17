using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class OpenOrderInList
    {

        public int? CourierId { get; init; }
        public int OrderId { get; init; }
        public OrderType OrderType { get; init; }
        public double Weight { get; init; }
        public double Volume { get; init; }
        public double AirDistance { get; init; }
        public double? ActualDistance { get; init; }
        public TimeSpan? EstimatedArrivalTime { get; init; }
        public ScheduleStatus ScheduleStatus { get; init; }
        public TimeSpan TimeLeftToDeadline { get;   init; }
        public DateTime MaxTimeToArrival { get; init; }


    }

   
    
    }

