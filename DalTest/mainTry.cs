using System;
using DalTest;    // Intialization.Do(...)
using Dal;        // CourierImplemention, DeliveryImplemention, ...
using DalApi;
using DO;

#region enums

// main menu for the whole DAL test
enum MainMenuOption
{
    EXIT = 0,
    INIT_DB = 1,
    MANAGE_ENTITY = 2,
    SHOW_ALL_DB = 3,
    CONFIG_MENU = 4,
    RESET_ALL = 5
}

// choose which entity to manage
enum EntityMenuOption
{
    BACK = 0,
    COURIER = 1,
    DELIVERY = 2,
    ORDER = 3
}

// CRUD menu for a single entity
enum CrudMenuOption
{
    BACK = 0,
    CREATE = 1,
    READ_ALL = 2,
    READ_BY_ID = 3,
    UPDATE = 4,
    DELETE = 5,
    DELETE_ALL = 6
}

// config submenu
enum ConfigMenuOption
{
    EXIT = 0,
    ADVANCE_MINUTES = 1,
    ADVANCE_HOURS = 2,
    ADVANCE_DAYS = 3,
    SHOW_CLOCK = 4,
    SET_NEW_CONFIG = 5,
    SHOW_CONFIG_VALUES = 6,
    RESET_CONFIG = 7
}

#endregion

internal class MainTry
{
    // DAL objects
    private static ICourier courierDal = new CourierImplemention();
    private static IDelivery deliveryDal = new DeliveryImplemention();
    private static IOrder orderDal = new OrderImplementation();
    private static IConfig configDal = new ConfigImplementation();

    static void Main(string[] args)
    {
        Console.WriteLine("=== DAL Test Start ===");

        while (true)
        {
            PrintMainMenu();
            int choice = ReadIntInRange(0, 5, "Enter your choice (0-5): ");

            if ((MainMenuOption)choice == MainMenuOption.EXIT)
                break;

            HandleMainMenu(choice);
        }

        Console.WriteLine("Bye 👋");
    }

    // ================== MAIN MENU ==================

    static void HandleMainMenu(int choice)
    {
        MainMenuOption option = (MainMenuOption)choice;

        switch (option)
        {
            case MainMenuOption.INIT_DB:
                Console.WriteLine("Initializing database (random data)...");
                Intialization.Do(courierDal, deliveryDal, orderDal, configDal);
                Console.WriteLine("Initialization done.");
                break;

            case MainMenuOption.MANAGE_ENTITY:
                HandleEntityMenu();
                break;

            case MainMenuOption.SHOW_ALL_DB:
                ShowAllData();
                break;

            case MainMenuOption.CONFIG_MENU:
                HandleConfigMenu();
                break;

            case MainMenuOption.RESET_ALL:
                Console.WriteLine("Resetting all data and config...");
                configDal.Reset();
                courierDal.DeleteAll();
                deliveryDal.DeleteAll();
                orderDal.DeleteAll();
                Console.WriteLine("All data cleared.");
                break;
        }
    }

    static void PrintMainMenu()
    {
        Console.WriteLine("\n===== DAL MAIN MENU =====");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Initialize database");
        Console.WriteLine("2. Manage entities (CRUD)");
        Console.WriteLine("3. Show all data");
        Console.WriteLine("4. Configuration menu");
        Console.WriteLine("5. Reset ALL (config + lists)");
        Console.WriteLine("=========================\n");
    }

    // ================== ENTITY MENU ==================

    static void HandleEntityMenu()
    {
        while (true)
        {
            PrintEntityMenu();
            int choice = ReadIntInRange(0, 3, "Choose entity (0-3): ");

            EntityMenuOption option = (EntityMenuOption)choice;

            if (option == EntityMenuOption.BACK)
                break;

            switch (option)
            {
                case EntityMenuOption.COURIER:
                    HandleCrudMenu("Courier");
                    break;

                case EntityMenuOption.DELIVERY:
                    HandleCrudMenu("Delivery");
                    break;

                case EntityMenuOption.ORDER:
                    HandleCrudMenu("Order");
                    break;
            }
        }
    }

    static void PrintEntityMenu()
    {
        Console.WriteLine("\n===== ENTITY MENU =====");
        Console.WriteLine("0. Back");
        Console.WriteLine("1. Couriers");
        Console.WriteLine("2. Deliveries");
        Console.WriteLine("3. Orders");
        Console.WriteLine("=======================\n");
    }

    // ================== CRUD MENU ==================

    static void HandleCrudMenu(string entityName)
    {
        while (true)
        {
            PrintCrudMenu(entityName);
            int choice = ReadIntInRange(0, 6, "Choose CRUD action (0-6): ");

            CrudMenuOption option = (CrudMenuOption)choice;
            if (option == CrudMenuOption.BACK)
                break;

            switch (option)
            {
                case CrudMenuOption.CREATE:
                    CreateEntity(entityName);
                    break;

                case CrudMenuOption.READ_ALL:
                    ReadAllEntities(entityName);
                    break;

                case CrudMenuOption.READ_BY_ID:
                    ReadEntityById(entityName);
                    break;

                case CrudMenuOption.UPDATE:
                    UpdateEntity(entityName);
                    break;

                case CrudMenuOption.DELETE:
                    DeleteEntity(entityName);
                    break;

                case CrudMenuOption.DELETE_ALL:
                    DeleteAllEntities(entityName);
                    break;
            }
        }
    }

    static void PrintCrudMenu(string entityName)
    {
        Console.WriteLine($"\n===== {entityName.ToUpper()} CRUD MENU =====");
        Console.WriteLine("0. Back");
        Console.WriteLine("1. Create");
        Console.WriteLine("2. Read all");
        Console.WriteLine("3. Read by ID");
        Console.WriteLine("4. Update");
        Console.WriteLine("5. Delete");
        Console.WriteLine("6. Delete ALL");
        Console.WriteLine("======================================\n");
    }

    // ================== CONFIG MENU ==================

    static void HandleConfigMenu()
    {
        while (true)
        {
            PrintConfigMenu();
            int choice = ReadIntInRange(0, 7, "Choose config option (0-7): ");

            ConfigMenuOption option = (ConfigMenuOption)choice;
            if (option == ConfigMenuOption.EXIT)
                break;

            switch (option)
            {
                case ConfigMenuOption.ADVANCE_MINUTES:
                    int m = ReadIntInRange(0, 10000, "Minutes to advance: ");
                    configDal.Clock = configDal.Clock.AddMinutes(m);
                    Console.WriteLine($"Clock: {configDal.Clock}");
                    break;

                case ConfigMenuOption.ADVANCE_HOURS:
                    int h = ReadIntInRange(0, 10000, "Hours to advance: ");
                    configDal.Clock = configDal.Clock.AddHours(h);
                    Console.WriteLine($"Clock: {configDal.Clock}");
                    break;

                case ConfigMenuOption.ADVANCE_DAYS:
                    int d = ReadIntInRange(0, 10000, "Days to advance: ");
                    configDal.Clock = configDal.Clock.AddDays(d);
                    Console.WriteLine($"Clock: {configDal.Clock}");
                    break;

                case ConfigMenuOption.SHOW_CLOCK:
                    Console.WriteLine($"Current clock: {configDal.Clock}");
                    break;

                case ConfigMenuOption.SET_NEW_CONFIG:
                    Console.Write("Enter new company address: ");
                    configDal.CompanyAddress = Console.ReadLine();
                    Console.WriteLine("Updated.");
                    break;

                case ConfigMenuOption.SHOW_CONFIG_VALUES:
                    Console.WriteLine($"Clock: {configDal.Clock}");
                    Console.WriteLine($"Company: {configDal.CompanyAddress}");
                    Console.WriteLine($"Lat/Lon: {configDal.Latitude},{configDal.Longitude}");
                    Console.WriteLine($"Speeds: car={configDal.AvgCarSpeed}, moto={configDal.AvgMotocyclerSpeed}, walk={configDal.AvgWalkingSpeed}");
                    break;

                case ConfigMenuOption.RESET_CONFIG:
                    configDal.Reset();
                    Console.WriteLine("Config reset.");
                    break;
            }
        }
    }

    static void PrintConfigMenu()
    {
        Console.WriteLine("\n===== CONFIG MENU =====");
        Console.WriteLine("0. Back");
        Console.WriteLine("1. Advance clock (minutes)");
        Console.WriteLine("2. Advance clock (hours)");
        Console.WriteLine("3. Advance clock (days)");
        Console.WriteLine("4. Show clock");
        Console.WriteLine("5. Set config value");
        Console.WriteLine("6. Show all config values");
        Console.WriteLine("7. Reset config");
        Console.WriteLine("=======================\n");
    }

    // ================== ENTITY OPERATIONS (STUBS) ==================

    static void CreateEntity(string entityName)
    {
        Console.WriteLine($"[stub] Create {entityName} here...");
        // כאן תוכל לקרוא ל-courierDal.Create(...) וכן הלאה
    }

    static void ReadAllEntities(string entityName)
    {
        Console.WriteLine($"[stub] Read ALL {entityName}s:");

        if (entityName == "Courier")
        {
            foreach (var c in courierDal.ReadAll())
                Console.WriteLine($"{c.Id} | {c.Name} | {c.Phone}");
        }
        else if (entityName == "Delivery")
        {
            foreach (var d in deliveryDal.ReadAll())
                Console.WriteLine($"{d.Id} | order={d.OrderId} | courier={d.CourierId}");
        }
        else if (entityName == "Order")
        {
            foreach (var o in orderDal.ReadAll())
                Console.WriteLine($"{o.Id} | {o.CustomerName} | {o.FullAddress}");
        }
    }

    static void ReadEntityById(string entityName)
    {
        int id = ReadIntInRange(1, int.MaxValue, $"Enter {entityName} ID: ");

        if (entityName == "Courier")
        {
            var c = courierDal.Read(id);
            Console.WriteLine(c == null ? "Not found" : $"{c.Id} | {c.Name} | {c.Phone}");
        }
        else if (entityName == "Delivery")
        {
            var d = deliveryDal.Read(id);
            Console.WriteLine(d == null ? "Not found" : $"{d.Id} | {d.OrderId} | {d.CourierId}");
        }
        else if (entityName == "Order")
        {
            var o = orderDal.Read(id);
            Console.WriteLine(o == null ? "Not found" : $"{o.Id} | {o.CustomerName} | {o.FullAddress}");
        }
    }

    static void UpdateEntity(string entityName)
    {
        Console.WriteLine($"[stub] Update {entityName}...");
        // תוכל להשלים לפי הדרישה
    }

    static void DeleteEntity(string entityName)
    {
        int id = ReadIntInRange(1, int.MaxValue, $"Enter {entityName} ID to delete: ");

        if (entityName == "Courier")
            courierDal.Delete(id);
        else if (entityName == "Delivery")
            deliveryDal.Delete(id);
        else if (entityName == "Order")
            orderDal.Delete(id);

        Console.WriteLine("Deleted.");
    }

    static void DeleteAllEntities(string entityName)
    {
        if (entityName == "Courier")
            courierDal.DeleteAll();
        else if (entityName == "Delivery")
            deliveryDal.DeleteAll();
        else if (entityName == "Order")
            orderDal.DeleteAll();

        Console.WriteLine("All records deleted.");
    }

    // ================== SHOW ALL ==================

    static void ShowAllData()
    {
        Console.WriteLine("\n--- Couriers ---");
        foreach (var c in courierDal.ReadAll())
            Console.WriteLine($"{c.Id} | {c.Name}");

        Console.WriteLine("\n--- Deliveries ---");
        foreach (var d in deliveryDal.ReadAll())
            Console.WriteLine($"{d.Id} | order={d.OrderId} | courier={d.CourierId}");

        Console.WriteLine("\n--- Orders ---");
        foreach (var o in orderDal.ReadAll())
            Console.WriteLine($"{o.Id} | {o.CustomerName} | {o.FullAddress}");
    }

    // ================== INPUT HELPER ==================

    static int ReadIntInRange(int min, int max, string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (int.TryParse(s, out int num) && num >= min && num <= max)
                return num;
            Console.WriteLine("Invalid input, try again.");
        }
    }
}
