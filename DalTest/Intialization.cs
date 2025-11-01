namespace DalTest;
using DalApi;
using DO;

public static class Intialization
{

    private static IConfig? s_DalConfig;
    private static ICourier? s_DalCourier;
    private static IDelivery? s_DalDelivery;
    private static IOrder? s_DalOrder;


    const int MINID = 100_000_000;  
    const int MAXID = 999_999_999;   

    private static readonly Random s_rand = new();

    public static void Do(
    ICourier? dalCourier,
    IDelivery? dalDelivery,
    IOrder? dalOrder,
    IConfig? dalConfig)
    {
        // store refs
        s_DalCourier = dalCourier ?? throw new NullReferenceException("DAL courier cannot be null!");
        s_DalDelivery = dalDelivery ?? throw new NullReferenceException("DAL delivery cannot be null!");
        s_DalOrder = dalOrder ?? throw new NullReferenceException("DAL order cannot be null!");
        s_DalConfig = dalConfig ?? throw new NullReferenceException("DAL config cannot be null!");

        Console.WriteLine("Reset configuration and lists...");

        // stage 1 reset
        s_DalConfig.Reset();
        s_DalCourier.DeleteAll();
        s_DalDelivery.DeleteAll();
        s_DalOrder.DeleteAll();

        Console.WriteLine("Create initial data...");

        createConfig();
        createCourier();
        createDelivery();
        createOrders();

        Console.WriteLine("Init done.");
    }

    private static void createConfig()
    {
        
        s_DalConfig!.Clock = new DateTime(2000, 1, 1);


        s_DalConfig.CompanyAddress = "משרד ראשי " + s_rand.Next(1, 999);
        s_DalConfig.Latitude = 31.7 + s_rand.NextDouble();   // טווח בארץ
        s_DalConfig.Longitude = 35.1 + s_rand.NextDouble();

        s_DalConfig.MaxAirRange = s_rand.Next(5, 50);
        s_DalConfig.AvgCarSpeed = s_rand.Next(60, 120);
        s_DalConfig.AvgMotocyclerSpeed = s_rand.Next(40, 90);
        s_DalConfig.AvgWalkingSpeed = s_rand.Next(3, 10);

        s_DalConfig.maxSupplayTime = TimeSpan.FromMinutes(s_rand.Next(30, 300));
        s_DalConfig.RiskRange = TimeSpan.FromMinutes(s_rand.Next(5, 30));
        s_DalConfig.UnactivityRange = TimeSpan.FromHours(s_rand.Next(1, 5));

        s_DalConfig.ManagerId = s_rand.Next(1000, 9999);
        s_DalConfig.ManagerPass = "admin" + s_rand.Next(100, 999);

    }

    private static void createCourier()
    {

        string[] names = new string[]
{
    "Bob TheBuilder",
    "Megan Toast",
    "Sir Wigglebottom",
    "Clara Byte",
    "John Lemon",
    "Patty O’Furniture",
    "Elon Dusk",
    "Linda Logic",
    "Max Powers",
    "Captain Debug"
};

        string[] cities = { "Jerusalem", "Tel Aviv", "Haifa", "Beer Sheva", "Eilat", "Ashdod", "Ramat Gan" };

        foreach (string name in names)
        {
            int id = s_rand.Next(MINID, MAXID);

            do
            {
                id = s_rand.Next(MINID, MAXID);
            } while (s_DalCourier!.Read(id) != null);

            string phone = "05" + s_rand.Next(0, 9) + "-" + s_rand.Next(1000000, 9999999);

            string email = name.ToLower().Replace(" ", "").Replace("’", "") + "@example.com";

            string password = "pw" + s_rand.Next(1000, 9999) + "!" + (char)s_rand.Next('A', 'Z' + 1);


            string address = $"{cities[s_rand.Next(cities.Length)]}, St {s_rand.Next(1, 200)}";

            bool isActive = s_rand.Next(0, 100) < 80; 

            double? personalMaxWeight = s_rand.Next(5, 25); 

            DO.ShipmentType[] types = Enum.GetValues<DO.ShipmentType>();
            DO.ShipmentType preferredType = types[s_rand.Next(types.Length)];

            DateTime startBase = new DateTime(s_DalConfig!.Clock.Year - 2, 1, 1);
            int range = (s_DalConfig.Clock - startBase).Days;
            DateTime employmentStartTime = startBase.AddDays(s_rand.Next(range));

            s_DalCourier!.Create(new DO.Courier
            {
                Id = id,
                Name = name,
                Phone = phone,
                Email = email,
                Password = password,
                Address = address,
                IsActive = isActive,
                PersonalMaxWeightKg = personalMaxWeight,
                PreferredShipmentType = preferredType,
                EmploymentStartTime = employmentStartTime
            });
        }

    }

    

    private static void createDelivery()
    {



        // Get all possible enum values
        DO.DeliveryType[] deliveryTypes = Enum.GetValues<DO.DeliveryType>();
        DO.DeliveryEndType[] endTypes = Enum.GetValues<DO.DeliveryEndType>();

        List<DO.Courier> couriers = s_DalCourier!.ReadAll();


        for (int i = 0; i < 20; i++)
        {
            // --- Generate unique ID ---
            int id;
            do
            {
                id = s_rand.Next(MINID, MAXID);
            } while (s_DalDelivery!.Read(id) != null);

            // --- Assign related order and courier ---
            int orderId = s_rand.Next(100000, 999999);
            var courier = couriers[s_rand.Next(couriers.Count)];
            


            // --- Random delivery properties ---
            DO.DeliveryType deliveryType = deliveryTypes[s_rand.Next(deliveryTypes.Length)];
            double? distanceKm =  Math.Round(s_rand.NextDouble() * 20 + 1, 2);

            // --- Random start time based on system clock ---
            DateTime startBase = new DateTime(s_DalConfig!.Clock.Year - 1, 1, 1);
            int range = (s_DalConfig.Clock - startBase).Days;
            DateTime startTime = startBase.AddDays(s_rand.Next(range));

            // --- Randomly decide if delivery has ended ---
            bool finished = s_rand.Next(0, 100) < 70; // 70% finished
            DateTime? endTime = finished
                ? startTime.AddMinutes(s_rand.Next(20, 300))
                : null;

            DO.DeliveryEndType? deliveryEndType = finished
                ? endTypes[s_rand.Next(endTypes.Length)]
                : null;

            // --- Create and add the new delivery ---
            s_DalDelivery!.Create(new DO.Delivery
            {
                Id = id,
                OrderId = orderId ,
                CourierId = courier.Id,
                DeliveryType = deliveryType,
                StartTime = startTime,
                DistanceKm = distanceKm,
                DeliveryEndType = deliveryEndType,
                EndTime = endTime
            });
        }
    }



    private static void createOrders()
    {
        // Possible enum values for order type
        DO.OrderType[] orderTypes = Enum.GetValues<DO.OrderType>();

        // Data pools for random generation
        string[] streets = { "Ben Gurion", "Herzl", "Dizengoff", "Weizmann", "Rothschild", "Jabotinsky", "Allenby", "King George" };
        string[] cities = { "Jerusalem", "Tel Aviv", "Haifa", "Beer Sheva", "Eilat", "Ashdod", "Ramat Gan" };
        string[] customers = { "Alice Cohen", "David Levi", "Sarah Mizrahi", "Yossi Green", "Noa Azulay", "Mike Stone", "Tom Harel", "Rachel Bloom" };
        string[] descriptions = { "Electronics", "Groceries", "Furniture", "Documents", "Clothes", "Books", "Toys", "Flowers" };
        string[] notesList = { "Leave at the door", "Fragile!", "Deliver ASAP", "Customer not home", "Call before arriving", null, null };

        // Get all existing deliveries
        List<DO.Delivery> deliveries = s_DalDelivery!.ReadAll();

        foreach (var delivery in deliveries)
        {
            // Skip if the order already exists
            if (s_DalOrder!.Read(delivery.OrderId) != null)
                continue;

            // --- Use the SAME Id as in Delivery ---
            int id = delivery.OrderId;

            // --- Random order details ---
            DO.OrderType orderType = orderTypes[s_rand.Next(orderTypes.Length)];
            string? description = descriptions[s_rand.Next(descriptions.Length)];
            string? notes = notesList[s_rand.Next(notesList.Length)];

            string city = cities[s_rand.Next(cities.Length)];
            string street = streets[s_rand.Next(streets.Length)];
            int houseNumber = s_rand.Next(1, 200);
            string fullAddress = $"{street} St {houseNumber}, {city}";

            double latitude = 31.0 + s_rand.NextDouble() * 2.0;   // ~31–33
            double longitude = 34.5 + s_rand.NextDouble() * 2.0;  // ~34.5–36.5

            string customerName = customers[s_rand.Next(customers.Length)];
            string customerPhone = "05" + s_rand.Next(0, 9) + "-" + s_rand.Next(1000000, 9999999);

            // --- Order creation time = same as delivery start ---
            DateTime orderCreationTime = delivery.StartTime;

            // --- Create and add the new Order ---
            s_DalOrder!.Create(new DO.Order
            {
                Id = id,
                OrderType = orderType,
                Description = description,
                FullAddress = fullAddress,
                Latitude = latitude,
                Longitude = longitude,
                CustomerName = customerName,
                CustomerPhone = customerPhone,
                Notes = notes,
                OrderCreationTime = orderCreationTime
            });
        }
    }



}
