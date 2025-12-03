using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class Tools
    {

        private static IDal? s_dal = Factory.Get;

        public static string ToStringProperty<T>(this T t)
        {
            if (t == null)
                return "";

            PropertyInfo[] properties = typeof(T).GetProperties();
            List<string> lines = new();

            foreach (var prop in properties)
            {
                object? value = prop.GetValue(t);

                // הדפסה יפה לרשימות
                if (value is System.Collections.IEnumerable list &&
                    value is not string)
                {
                    List<string> items = new();
                    foreach (var item in list)
                        items.Add(item?.ToString() ?? "null");

                    lines.Add($"{prop.Name}: [ {string.Join(", ", items)} ]");
                }
                else
                {
                    lines.Add($"{prop.Name}: {value}");
                }
            }

            return string.Join("\n", lines);
        }

        /// <summary>
        /// Sorts a collection of items and groups them by a specified key.
        /// </summary>
        /// T = The type of items in the collection.
        /// TKey = The type of the key used for grouping.
        public static IEnumerable<T> SortWithGroupBy<T, TKey>(IEnumerable<T> items,
            Func<T, TKey> keySelector)
        {
            return items
                .GroupBy(keySelector)      // קיבוץ לפי הקריטריון
                .OrderBy(g => g.Key)       // מיון הקבוצות לפי המפתח
                .SelectMany(g => g);       // החזרת כל הפריטים כסדרה ממוינת
        }
        internal static double SpeedKmByType(DO.ShipmentType shipmentType)
        {
            return shipmentType switch
            {
                DO.ShipmentType.Foot => s_dal.Config.AvgWalkingSpeed,
                DO.ShipmentType.Bicycle => s_dal.Config.AvgBicycleSpeed,
                DO.ShipmentType.Motorcycle => s_dal.Config.AvgMotorcycleSpeed,
                DO.ShipmentType.Car => s_dal.Config.AvgCarSpeed,
            };
        }

        #region Calculations

        #region Distances

        // Radius of the Earth in kilometers
        private const double EarthRadiusKm = 6371;

        /// <summary>
        /// Calculates the air distance between two geographic points using the Haversine formula.
        /// </summary>
        internal static double CalculateAirDistance(double lat2, double lon2, double? lat = null, double? lon = null)
        {
            double lat1;
            double lon1;
            if (s_dal.Config.Latitude != null && s_dal.Config.Longitude != null)
            {
                lat1 = (lat == null) ? s_dal.Config.Latitude.Value : lat.Value;
                lon1 = (lon == null) ? s_dal.Config.Longitude.Value : lon.Value;
            }
            else
            {
                lat1 = (lat == null) ? 0 : lat.Value;
                lon1 = (lon == null) ? 0 : lon.Value;
            }

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private static double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        /// <summary>
        /// Get coordinates from an address (Geocoding)
        /// Accesses the internet (OSM Nominatim) synchronously and returns latitude and longitude.
        /// </summary>
        /// <param name="address">Full address (for example: "HaNesi'im 7, Petah Tikva")</param>
        /// <returns>Tuple of (Latitude, Longitude) or null if failed</returns>
        internal static (double Lat, double Lon)? GetLocationFromAddress(string address)
        {
            string url = $"https://nominatim.openstreetmap.org/search?q={address}&format=json&limit=1";

            try
            {
                using var client = new HttpClient();
                // Nominatim requires a User-Agent header
                client.DefaultRequestHeaders.Add("User-Agent", "PizzaDeliveryProject");

                var response = client.GetStringAsync(url).Result;

                using JsonDocument doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                if (root.GetArrayLength() > 0)
                {
                    var element = root[0];
                    string latStr = element.GetProperty("lat").GetString()!;
                    string lonStr = element.GetProperty("lon").GetString()!;

                    return (double.Parse(latStr), double.Parse(lonStr));
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Calculates the actual travel distance (driving/walking) between two points using OSRM API.
        /// </summary>
        internal static double GetTravelDistance(double lat1, double lon1, double lat2, double lon2, DO.ShipmentType type)
        {
            // We select the correct profile according to the shipment type
            string profile = type switch
            {
                DO.ShipmentType.Foot => "walking",
                DO.ShipmentType.Bicycle => "biking",
                _ => "driving" 
            };

            // Note: OSRM URL format expects {Longitude},{Latitude} (opposite of Google Maps)
            string url = $"http://router.project-osrm.org/route/v1/{profile}/{lon1},{lat1};{lon2},{lat2}?overview=false";

            try
            {
                using (var client = new HttpClient())
                {
                    // Sending a synchronous (blocking) request as required in stage 4
                    var response = client.GetStringAsync(url).Result;

                    using (JsonDocument doc = JsonDocument.Parse(response))
                    {
                        var routes = doc.RootElement.GetProperty("routes");

                        if (routes.GetArrayLength() > 0)
                        {
                            // The distance is returned in meters
                            double distanceInMeters = routes[0].GetProperty("distance").GetDouble();

                            // Convert meters to kilometers
                            return distanceInMeters / 1000.0;
                        }
                    }
                }
            }
            catch
            {
                // In case of a network or server failure, return the air (straight-line) distance
                // This is a common fallback to prevent crashes when the internet is unavailable
                return CalculateAirDistance(lat2, lon2, lat1, lon1);
            }

            return 0;
        }

        #endregion

        #endregion Calculations

        #region Validtions

        //--------------
        //  Validtions
        //--------------
        internal static bool IdValidtion(int id)
        {
            return id.ToString().Length == 9;
        }

        internal static bool IsEmailValidManual(string? email)
        {
            // 1. בדיקה בסיסית: האם המחרוזת ריקה או מכילה רווחים?
            if (string.IsNullOrWhiteSpace(email) || email.Contains(' '))
            {
                return false;
            }

            // 2. בדיקת קיום סימן '@' אחד ויחיד
            int atIndex = email.IndexOf('@');
            int lastAtIndex = email.LastIndexOf('@');

            // חייב להכיל @, והוא חייב להיות הסימן היחיד
            if (atIndex == -1 || atIndex != lastAtIndex)
            {
                return false;
            }

            // 3. בדיקת מיקום הסימן '@'
            // אסור שיהיה תו ראשון או אחרון
            if (atIndex == 0 || atIndex == email.Length - 1)
            {
                return false;
            }

            // 4. בדיקת קיום סימן נקודה ('.') בדומיין (החלק שאחרי ה-@)
            string domainPart = email.Substring(atIndex + 1);
            int dotIndex = domainPart.IndexOf('.');

            // חייב להכיל נקודה אחת לפחות בדומיין
            if (dotIndex == -1)
            {
                return false;
            }

            // 5. בדיקת מיקום הנקודה בדומיין
            // אסור שהנקודה תהיה התו הראשון או האחרון בדומיין
            if (dotIndex == 0 || dotIndex == domainPart.Length - 1)
            {
                return false;
            }

            return true;
        }

    }
    #endregion


}
