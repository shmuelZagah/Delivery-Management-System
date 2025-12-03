using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace BO
{
    public class Config
    {
        // --- COMPANY DETAILS ---
        public string? CompanyAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? MaxAirRange { get; set; }

        // --- AVERAGE SPEEDS (optional for ETA calculations on UI) ---
        public double AvgCarSpeed { get; set; }
        public double AvgMotocyclerSpeed { get; set; }
        public double AvgBicycleSpeed { get; set; }
        public double AvgWalkingSpeed { get; set; }

        public override string ToString() => this.ToStringProperty();
    }
}
