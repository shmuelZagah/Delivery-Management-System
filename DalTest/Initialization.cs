namespace DalTest;
using DalApi;
using DO;
using System;
using System.Diagnostics.Metrics;
using System.Linq;

public static class Initialization
{


    private static IDal? s_dal;


    const int MINID = 200000000;
    const int MAXID = 400000000;

    private static readonly Random s_rand = new();
    private static DateTime s_clock = new DateTime(2000, 1, 1);

    public static void Do(

        
        )
    {

        s_dal = DalApi.Factory.Get;

        Console.WriteLine("Reset configuration and lists...");

        // stage 1 reset
        s_dal.ResetDB();

        Console.WriteLine("Create initial data...");

        createConfig();
        createCouriers();
        createOrders();
        createDeliveries();
    }

    private static void createConfig()
    {

        s_dal!.Config.Clock = s_clock;

        // company address roughly in central Israel
        s_dal!.Config.CompanyAddress = "Company HQ, Tel Aviv";

        // realistic coords for company (Tel Aviv)
        s_dal!.Config.Latitude = 32.0853;
        s_dal!.Config.Longitude = 34.7818;


        // ranges and speeds
        s_dal!.Config.MaxAirRange = 40; // km - company committed max delivery radius
        s_dal!.Config.AvgCarSpeed = 60;           // km/h
        s_dal!.Config.AvgMotorcycleSpeed = 45;    // km/h
        s_dal!.Config.AvgWalkingSpeed = 5;        // km/h
        s_dal!.Config.AvgBicycleSpeed = 15;       // km/h


        s_dal!.Config.maxSupplayTime = TimeSpan.FromMinutes(180);
        s_dal!.Config.RiskRange = TimeSpan.FromMinutes(15);
        s_dal!.Config.UnactivityRange = TimeSpan.FromHours(30);

        s_dal!.Config.ManagerId = 222222222;
        s_dal!.Config.ManagerPass = "admin" + 100;

        s_dal.Config.RiskRange = new TimeSpan(0, 45, 0);

    }


    private static void createCouriers()
    {

        string[] names = new string[]
        {
              "Bob TheBuilder","Megan Toast","Sir Wigglebottom","Clara Byte","John Lemon",
            "Patty O’Furniture","Elon Dusk","Linda Logic","Max Powers","Captain Debug",
            "Anna Singer","Omer Cohen","Yael Azulay","David Levi","Noa Rabin",
            "Tomer Azulay","Maya Green","Amit Bar","Rina Kaplan","Eli Shahar"
        };

        string[] cities = { "Tel Aviv", "Jerusalem", "Haifa", "Beer Sheva", "Ashdod", "Ramat Gan", "Petah Tikva" };

        foreach (string name in names)
        {
            int id;

            bool isActive = s_rand.Next(0, 100) < 80;

            do
            {
                id = s_rand.Next(MINID, MAXID);
            } while (s_dal!.Courier!.Read(id) != null);

            if (s_dal.Courier.Read(234567890) == null)
            {
                id = 234567890;
                isActive = true;
            }

            string phone = "05" + s_rand.Next(0, 9) + s_rand.Next(1000000, 9999999);

            string email = name.ToLower().Replace(" ", "").Replace("’", "") + "@example.com";

            string password = "1234567890";


            string address = $"{cities[s_rand.Next(cities.Length)]}, St {s_rand.Next(1, 200)}";

          

            double? personalMaxWeight = s_rand.Next(5, 25);

            DO.ShipmentType[] types = Enum.GetValues<DO.ShipmentType>();
            DO.ShipmentType preferredType = types[s_rand.Next(types.Length)];

            DateTime employmentStartTime = s_dal.Config.Clock.AddDays(-s_rand.Next(0, 730));

            s_dal!.Courier!.Create(new DO.Courier
            {
                Id = id,
                Name = name,
                Phone = phone,
                Email = email,
                Password = password,
                IsActive = isActive,
                PersonalMaxDistance = personalMaxWeight,
                PreferredShipmentType = preferredType,
                EmploymentStartTime = employmentStartTime
            });
        }

    }

    private static void createOrders()
    {
        string[] customers = {
    "Alice Cohen", "David Levi", "Sarah Mizrahi", "Yossi Green", "Noa Azulay",
    "Mike Stone", "Tom Harel", "Rachel Bloom", "Ori Shalom", "Liat Azulay",
    "Eli Ben-David", "Dana Peretz", "Shir Avraham", "Guy Malka", "Roni Cohen",
    "Nadav Bar", "Gal Sharon", "Eden Refael", "Maya Hadad", "Nir Tal",
    "Hila Dayan", "Yarden Azulay", "Daniel Cohen", "Ruth Levi", "Amir Segal",
    "Dikla Moyal", "Ben Shani", "Mor Chen", "Tal Azulay", "Idan Kaplan",
    "Omer Levy", "Neta Blum", "Lior Gold", "Shani Peleg", "Yair Azulay",
    "Adi Ronen", "Dana Goren", "Noam Erez", "Moran Shalem", "Itay Rosen",
    "Tamar Koren", "Galit Hadar", "Rafi Dahan", "Einav Mor", "Sivan Regev",
    "Erez Azulay", "Lina Weiss", "Bar Oren", "Roy Ashkenazi", "Naomi Cohen",
    "Talia Katz"
};

        string[] descriptions = {
   "Electronics", "Groceries", "Furniture", "Documents", "Clothes", "Books"
};


        var addressPool = new List<(string address, double lat, double lon)>
        {
            ("Tel Aviv, Rothschild Blvd 33", 32.0646, 34.7756),
            ("Tel Aviv, Dizengoff St 100", 32.0799, 34.7865),
            ("Tel Aviv, Allenby St 40", 32.0717, 34.7717),
            ("Tel Aviv, Ibn Gabirol St 10", 32.0734, 34.7890),
            ("Tel Aviv, HaYarkon St 30", 32.0809, 34.7713),
            ("Tel Aviv, Ben Yehuda St 120", 32.0865, 34.7886),
            ("Tel Aviv, King George St 20", 32.0643, 34.7780),
            ("Tel Aviv, Arlozorov St 16", 32.0675, 34.7890),
            ("Tel Aviv, Sderot Sha'ul Hamelech 50", 32.0778, 34.7796),
            ("Tel Aviv, Jaffa St 12", 32.0540, 34.7510),
            ("Jaffa, Yefet St 10", 32.0520, 34.7506),
            ("Ramat Gan, Ahuza St 1", 32.0712, 34.8123),
            ("Givatayim, Jabotinsky St 5", 32.0618, 34.8079),
            ("Ramat Gan, Avnei Chen 3", 32.0730, 34.8190),
            ("Givatayim, Aluf Sade 8", 32.0635, 34.8065),
            ("Herzliya, Herzl St 2", 32.1668, 34.8400),
            ("Herzliya Pituach, Hayam St 5", 32.1730, 34.8320),
            ("Kfar Saba, Yitzhak Rabin Blvd 3", 32.1689, 34.9065),
            ("Petah Tikva, Haim Ozer St 6", 32.0858, 34.8878),
            ("Rishon LeZion, Hativat Hanegev 4", 31.9736, 34.7898),
            ("Bat Yam, Balfour 2", 32.0285, 34.7483),
            ("Holon, Derech Ayalon 10", 32.0123, 34.7870),
            ("Holon, Ben Gurion 33", 32.0190, 34.7794),
            ("Bnei Brak, Jabotinsky 12", 32.0826, 34.8286),
            ("Or Yehuda, Yad Harutzim 7", 32.0310, 34.8420),
            ("Kiryat Ono, Hahoresh 9", 32.0460, 34.8350),
            ("Or Akiva, HaSharon St 15", 32.3920, 34.9180),
            ("Netanya, Herzl St 9", 32.3329, 34.8596),
            ("Ra'anana, Hama'apilim 11", 32.1830, 34.8720),
            ("Ramat HaSharon, Hahagana 2", 32.1590, 34.8250),
            ("Modi'in, HaPalmach 4", 31.9000, 35.0078),
            ("Lod, Yitzhak Rabin St 8", 31.9454, 34.8868),
            ("Ramla, Herzl St 21", 31.9343, 34.8632),
            ("Ashdod, Yefet St 5", 31.8014, 34.6436),
            ("Rehovot, Katznelson 10", 31.8939, 34.8110),
            ("Kfar Saba, Haim Weizman 6", 32.1696, 34.9076),
            ("Petah Tikva, Weizmann St 22", 32.0860, 34.8980),
            ("Ness Ziona, Harel St 3", 31.9281, 34.8000),
            ("Shoham, Emek Dotan 2", 31.9920, 34.9870),
            ("Yavne, Hagalil 14", 31.8340, 34.7430),
            ("Hod Hasharon, Sokolov 7", 32.1510, 34.9020),
            ("Kefar Sava, Em Hamoshavot 9", 32.1640, 34.9050),
            ("Tnufa, Derech Hachashmonaim 2", 32.0110, 34.8400),
            ("Kfar Malal, HaShaked 1", 32.0550, 34.9300),
            ("Rosh HaAyin, Hamoshava 5", 32.1060, 34.9510),
            ("Hadera, Bilu 3", 32.4290, 34.9120),
            ("Sderot, Hagefen 8", 31.5195, 34.5996),
             ("Rosh HaAyin, Hamoshava 5", 32.1060, 34.9510),
             ("Hod Hasharon, Sokolov 7", 32.1510, 34.9020)
        };

        DO.OrderType[] orderTypes = Enum.GetValues<DO.OrderType>();

        foreach (var customer in customers)
        {
            if (addressPool.Count == 0) break;
            int addrIndex = s_rand.Next(addressPool.Count);
            var chosenAddress = addressPool[addrIndex];
            addressPool.RemoveAt(addrIndex);

            DateTime orderCreationTime = s_dal!.Config!.Clock.AddHours(-s_rand.Next(0, 5));
            string customerPhone = "05" + s_rand.Next(0, 9)  + s_rand.Next(1000000, 9999999);
            string? notes = s_rand.Next(0, 100) < 20 ? "Leave at the door" : null;
            DO.OrderType type = orderTypes[s_rand.Next(orderTypes.Length)];

            s_dal!.Order!.Create(new DO.Order
            {
                Id = 0,
                OrderType = type,
                Description = descriptions[s_rand.Next(descriptions.Length)],
                FullAddress = chosenAddress.address,
                Latitude = chosenAddress.lat,
                Longitude = chosenAddress.lon,
                Weight = s_rand.Next(10, 20),
                Volume = s_rand.Next(3,30),
                CustomerName = customer,
                CustomerPhone = customerPhone,
                Notes = notes,
                OrderCreationTime = orderCreationTime
            });
        }
    }
    private static void createDeliveries()
    {
        // Get enums
        DO.DeliveryEndType[] endTypes = Enum.GetValues<DO.DeliveryEndType>();

        // 1. Fetch ALL couriers
        List<DO.Courier> allCouriers = s_dal!.Courier!.ReadAll().ToList();

        // 2. Fetch all orders (Sorted by time)
        List<DO.Order> allOrders = s_dal!.Order!.ReadAll().OrderBy(o => o.OrderCreationTime).ToList();

        // 3. Track couriers currently busy with an ACTIVE delivery
        HashSet<int> busyCouriers = new HashSet<int>();

        foreach (var order in allOrders)
        {
            // Skip small percentage (randomly leave as Pending)
            if (s_rand.NextDouble() > 0.9) continue;

            // --- Find a suitable courier (Local distance check) ---
            var suitableCouriers = allCouriers.Where(c =>
            {
                double dist = HaversineDistanceKm(order.Latitude, order.Longitude,
                              s_dal!.Config!.Latitude!.Value, s_dal!.Config.Longitude!.Value);
                return dist <= c.PersonalMaxDistance;
            }).ToList();

            if (!suitableCouriers.Any()) continue;

            // Pick a random courier
            var courier = suitableCouriers[s_rand.Next(suitableCouriers.Count)];

            // --- Determine Status: Active or History? ---
            bool isOldOrder = (s_dal.Config.Clock - order.OrderCreationTime).TotalHours > 24;
            bool isActive = !isOldOrder && (s_rand.NextDouble() < 0.8);

            // --- CONSTRAINTS ---

            // 1. Inactive Courier Check: Must be history if courier is inactive
            if (!courier.IsActive)
            {
                isActive = false;
            }

            // 2. Busy Check: If courier is already busy, this delivery must be history
            if (isActive && busyCouriers.Contains(courier.Id))
            {
                isActive = false;
            }

            // --- Calculate Data ---
            double distanceKm = HaversineDistanceKm(order.Latitude, order.Longitude,
                              s_dal.Config.Latitude!.Value, s_dal.Config.Longitude!.Value);

            // Safe Speed Calculation (PREVENT XML CRASH: Fallback if speed is 0)
            double speed = 60;
            switch (courier.PreferredShipmentType)
            {
                case DO.ShipmentType.Foot:
                    speed = s_dal.Config.AvgWalkingSpeed > 0 ? s_dal.Config.AvgWalkingSpeed : 5;
                    break;
                case DO.ShipmentType.Bicycle:
                    speed = s_dal.Config.AvgBicycleSpeed > 0 ? s_dal.Config.AvgBicycleSpeed : 15;
                    break;
                case DO.ShipmentType.Motorcycle:
                    speed = s_dal.Config.AvgMotorcycleSpeed > 0 ? s_dal.Config.AvgMotorcycleSpeed : 45;
                    break;
                case DO.ShipmentType.Car:
                    speed = s_dal.Config.AvgCarSpeed > 0 ? s_dal.Config.AvgCarSpeed : 60;
                    break;
            }

            // Calculate Duration (Do NOT modify distanceKm itself!)
            double durationHours = distanceKm / speed;
            int durationMinutes = (int)(durationHours * 60) + s_rand.Next(10, 30); // Add buffer

            // Times
            DateTime startTime = order.OrderCreationTime.AddMinutes(s_rand.Next(20, 120));
            DateTime? endTime = null;
            DO.DeliveryEndType? endType = null;

            if (!isActive)
            {
                // -- History (Finished) --
                endTime = startTime.AddMinutes(durationMinutes);

                // Safety: Don't end in the future
                if (endTime > s_dal.Config.Clock)
                    endTime = s_dal.Config.Clock.AddMinutes(-5);

                endType = DO.DeliveryEndType.Provided;
            }
            else
            {
                // -- Active (Open) --
                endTime = null;
                endType = null;

                // Mark courier as busy
                busyCouriers.Add(courier.Id);
            }

            // --- Create Delivery ---
            s_dal!.Delivery!.Create(new DO.Delivery
            {
                Id = 0, // Auto ID
                OrderId = order.Id,
                CourierId = courier.Id,
                DeliveryType = courier.PreferredShipmentType,
                StartTime = startTime,
                DistanceKm = distanceKm, // Save the actual distance!
                DeliveryEndType = endType,
                EndTime = endTime
            });
        }
    }

    // Air distance(Haversine)
    private static double HaversineDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        //Using polar projection calculation
        double R = 6371;
        double dLat = DegreeToRad(lat2 - lat1);
        double dLon = DegreeToRad(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreeToRad(lat1)) * Math.Cos(DegreeToRad(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double DegreeToRad(double deg) => deg * (Math.PI / 180.0);

}


