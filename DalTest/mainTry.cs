using System;
using DalTest;        // for Initialization
using Dal;            // for *Implementation classes (DalList project)
using DalApi;         // for the interfaces
using DO;             // for the data objects

internal class mainTry
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== DAL TEST START ===");

        // 1. create DAL implementations (from DalList project)
        ICourier courierDal = new CourierImplemention();
        IDelivery deliveryDal = new DeliveryImplemention();
        IOrder orderDal = new OrderImplementation();
        IConfig? configDal = new ConfigImplementation();

        // 2. run initialization
        Intialization.Do(
               courierDal,
               deliveryDal,
               orderDal,
               configDal
         );

        Console.WriteLine();
        Console.WriteLine("=== PRINTING DATA AFTER INITIALIZATION ===");

        // 3. read & print couriers
        Console.WriteLine("\n--- Couriers ---");
        foreach (var c in courierDal.ReadAll())
        {
            Console.WriteLine(
                $"Id={c.Id}, Name={c.Name}, Phone={c.Phone}, Active={c.IsActive}, Start={c.EmploymentStartTime}");
        }

        // 4. read & print deliveries
        Console.WriteLine("\n--- Deliveries ---");
        foreach (var d in deliveryDal.ReadAll())
        {
            Console.WriteLine(
                $"Id={d.Id}, OrderId={d.OrderId}, CourierId={d.CourierId}, Type={d.DeliveryType}, Start={d.StartTime}, End={d.EndTime}");
        }

        // 5. read & print orders
        Console.WriteLine("\n--- Orders ---");
        foreach (var o in orderDal.ReadAll())
        {
            Console.WriteLine(
                $"Id={o.Id}, Type={o.OrderType}, Customer={o.CustomerName}, Phone={o.CustomerPhone}, Address={o.FullAddress}, Created={o.OrderCreationTime}");
        }


        Console.WriteLine("\n=== DAL TEST END ===");
        Console.ReadKey();
    }
}
