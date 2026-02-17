using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Helpers;

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
        if (s_dal == null)
            throw new BO.BlInvalidOperationStateException("DAL is not initialized.");
        lock (AdminManager.BlMutex)
        {
            return shipmentType switch
            {
                DO.ShipmentType.Foot => s_dal.Config.AvgWalkingSpeed,
                DO.ShipmentType.Bicycle => s_dal.Config.AvgBicycleSpeed,
                DO.ShipmentType.Motorcycle => s_dal.Config.AvgMotorcycleSpeed,
                DO.ShipmentType.Car => s_dal.Config.AvgCarSpeed,
            };
        }
       
    }


    #region Password Handling
    public static string HashPassword(string password)
    {
        const int SaltSize = 16;
        const int HashSize = 32;
        const int Iterations = 10000;

        byte[] salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash)
    {
        const int SaltSize = 16;
        const int HashSize = 32;
        const int Iterations = 10000;

        try
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] generatedHash = pbkdf2.GetBytes(HashSize);

                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != generatedHash[i])
                    {
                        return false;
                    }
                }
            }
            return true; 
        }
        catch
        {
            return false; 
        }
    }

    public static string GenerateRandomPassword(int length = 8)
    {
        const string validChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%^&*?";
        StringBuilder password = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] uintBuffer = new byte[4];
            while (password.Length < length)
            {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                password.Append(validChars[(int)(num % (uint)validChars.Length)]);
            }
        }
        return password.ToString();
    }

    public static string PasswordStrengthChecker(string password)
    {
        int score = 0;

        if (password.Length < 8)
            return "Weak";

        if (password.Any(char.IsUpper))
            score++;
        if (password.Any(char.IsLower))
            score++;
        if (password.Any(char.IsDigit))
            score++;
        if (password.Any(ch => "!@#$%^&*?".Contains(ch)))
            score++;

        return score switch
        {
            1 => "Weak",
            2 => "Moderate",
            3 => "Strong",
            4 => "Very Strong",
            _ => "Unknown"
        };
    }

    #endregion

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

        lock (AdminManager.BlMutex)
        {
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
    internal static async Task<(double Lat, double Lon)> GetLocationFromAddress(string address)
    {
        // בדיקה ראשונית
        if (string.IsNullOrWhiteSpace(address))
            throw new BO.BlDoesNotExistException("Address cannot be empty.");


        string encodedAddress = Uri.EscapeDataString(address);
        string url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1";

        try
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("User-Agent", "PizzaDeliveryProject");
            client.Timeout = TimeSpan.FromSeconds(5); // טיימאאוט למקרה שאין אינטרנט

   
            var response = await client.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

    
            if (root.GetArrayLength() > 0)
            {
                var element = root[0];
                string latStr = element.GetProperty("lat").GetString()!;
                string lonStr = element.GetProperty("lon").GetString()!;

       
                double lat = double.Parse(latStr, CultureInfo.InvariantCulture);
                double lon = double.Parse(lonStr, CultureInfo.InvariantCulture);

                return (lat, lon);
            }
            else
            {
    
                throw new BO.BlDoesNotExistException($"Address not found: {address}");
            }
        }
        catch (Exception ex) when (ex is not BO.BlDoesNotExistException)
        {

            throw new BO.BlDoesNotExistException($"Failed to retrieve location for: {address}", ex);
        }
    }

    /// <summary>
    /// Calculates the actual travel distance (driving/walking) between two points using OSRM API.
    /// </summary>
    internal static async Task<double> GetTravelDistance(double lat1, double lon1, double lat2, double lon2, DO.ShipmentType type)
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
                var response = await client.GetStringAsync(url);

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
        if (id.ToString().Length !=9)
            throw new BlInvalidInputException("Id must be 9 characters long");
        return true;
    }

    internal static bool IsEmailValidManual(string? email)
    {

        bool thorwE = false;

        // 1. בדיקה בסיסית: האם המחרוזת ריקה או מכילה רווחים?
        if (string.IsNullOrWhiteSpace(email) || email.Contains(' '))
        {
            thorwE = true;
        }

        // 2. בדיקת קיום סימן '@' אחד ויחיד
        int atIndex = email.IndexOf('@');
        int lastAtIndex = email.LastIndexOf('@');

        // חייב להכיל @, והוא חייב להיות הסימן היחיד
        if (atIndex == -1 || atIndex != lastAtIndex)
        {
            thorwE = true;
        }

        // 3. בדיקת מיקום הסימן '@'
        // אסור שיהיה תו ראשון או אחרון
        if (atIndex == 0 || atIndex == email.Length - 1)
        {
            thorwE = true;
        }

        // 4. בדיקת קיום סימן נקודה ('.') בדומיין (החלק שאחרי ה-@)
        string domainPart = email.Substring(atIndex + 1);
        int dotIndex = domainPart.IndexOf('.');

        // חייב להכיל נקודה אחת לפחות בדומיין
        if (dotIndex == -1)
        {
            thorwE = true;
        }

        // 5. בדיקת מיקום הנקודה בדומיין
        // אסור שהנקודה תהיה התו הראשון או האחרון בדומיין
        if (dotIndex == 0 || dotIndex == domainPart.Length - 1)
        {
            thorwE = true;
        }


        if (thorwE) throw new BlInvalidInputException("Inviled email");

        return true;
    }

}
#endregion
