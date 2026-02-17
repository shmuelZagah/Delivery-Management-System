using BlApi;
using BO;
using System;
using System.Collections.Generic;

namespace PL;

internal class Program
{
    // גישה לשכבת הלוגיקה
    private static readonly IBl s_bl = Factory.Get();
    

    // user info;
    private static int user_id = 0;
    private static BO.UserType type;

    static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==================================================");
        Console.WriteLine("      Pizza Delivery System - Console Test");
        Console.WriteLine("==================================================");
        Console.ResetColor();

        // 1. אתחול ראשוני
        try
        {
            s_bl.Admin.InitializeDB();
        }
        catch (Exception ex)
        {
            PrintError("Initialization failed", ex);
            return;
        }

        // 2. "התחברות" כמנהל לצורך ביצוע פעולות
        Console.WriteLine("\n--- Login Simulation ---");

        bool flag = true;
        while (flag)
        {
            try
            {
                Console.WriteLine("Please enter the Manager ID defined in your Config (Manager = 222222222) (Courier = 234567890):");
                string idT;
                string password;

                idT = Console.ReadLine()!;
                

                Console.WriteLine("Enter Password: (Manager = admin100) (Courier = 1234567890)");
                password = Console.ReadLine()!;

                type = s_bl.Courier.Login(idT, password);
                flag = false;

                if (!int.TryParse(idT, out user_id))
                {
                    Console.WriteLine("Invalid Inpute. Try again:");
                    continue;
                }


            }
            catch (Exception ex)
            {
                PrintError("Login failed", ex);
                flag = true;
            }

        }

        Console.WriteLine($"Logged in with ID: {user_id}\n");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=============================");
        Console.WriteLine($"\tWelcome {type.ToString()}");
        Console.WriteLine("=============================");
        Console.ResetColor();

        // 3. לולאת התפריט הראשי
        bool exit = false;
        while (!exit)
        {
            PrintHeader($"Main Menu | System Time: {s_bl.Admin.GetClock()}");
            Console.WriteLine("1. Admin & Simulation (Clock)");
            Console.WriteLine("2. Couriers Management");
            Console.WriteLine("3. Orders Management");
            Console.WriteLine("0. Exit");
            Console.Write(">> ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1: AdminMenu(); break;
                    case 2: CourierMenu(); break;
                    case 3: OrderMenu(); break;
                    case 0: exit = true; break;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid number!"); 
            }
        }
    }

    #region Admin & Simulation
    private static void AdminMenu()
    {
        bool back = false;
        while (!back)
        {
            PrintHeader($"Admin & Simulation | Time: {s_bl.Admin.GetClock()}");
            Console.WriteLine("1. Show Configuration");
            Console.WriteLine("2. Forward Clock (Second)");
            Console.WriteLine("3. Forward Clock (Minute)");
            Console.WriteLine("4. Forward Clock (Hour)");
            Console.WriteLine("5. Forward Clock (Day)");
            Console.WriteLine("6. Forward Clock (Month)");
            Console.WriteLine("7. Forward Clock (Year)");
            Console.WriteLine("0. Back");
            Console.Write(">> ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                try
                {
                    switch (choice)
                    {
                        case 1:
                            var config = s_bl.Admin.GetConfig();
                            if (config != null)
                                Console.WriteLine(config.ToString());
                            break;
                        case 2:
                            s_bl.Admin.ForwardClock(TimeUnit.Second);
                            Console.WriteLine("Time advanced by 1 Second.");
                            break;
                        case 3:
                            s_bl.Admin.ForwardClock(TimeUnit.Minute);
                            Console.WriteLine("Time advanced by 1 Minute.");
                            break;
                        case 4:
                            s_bl.Admin.ForwardClock(TimeUnit.Hour);
                            Console.WriteLine("Time advanced by 1 Hour.");
                            break;
                        case 5:
                            s_bl.Admin.ForwardClock(TimeUnit.Day);
                            Console.WriteLine("Time advanced by 1 Day.");
                            break;
                        case 6:
                            s_bl.Admin.ForwardClock(TimeUnit.Month);
                            Console.WriteLine("Time advanced by 1 Month.");
                            break;
                        case 7:
                            s_bl.Admin.ForwardClock(TimeUnit.Year);
                            Console.WriteLine("Time advanced by 1 Year.");
                            break;
                        case 0:
                            back = true;
                            break;
                    }
                }
                catch (Exception ex) { PrintError("Admin Error", ex); }
            }
        }
    }
    #endregion

    #region Courier Management
    private static void CourierMenu()
    {
        bool back = false;
        while (!back)
        {
            PrintHeader("Courier Management");
            Console.WriteLine("1. Add Courier");
            Console.WriteLine("2. Get Courier Details");
            Console.WriteLine("3. Get All Couriers");
            Console.WriteLine("4. Update Courier");
            Console.WriteLine("5. Delete Courier");
            Console.WriteLine("0. Back");
            Console.Write(">> ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                try
                {
                    switch (choice)
                    {
                        case 1: AddCourier(); break;
                        case 2: GetCourier(); break;
                        case 3: ShowAllCouriers(); break;
                        case 4: UpdateCourier(); break;
                        case 5: DeleteCourier(); break;
                        case 0: back = true; break;
                    }
                }
                catch (Exception ex) { PrintError("Courier Operation Failed", ex); }
            }
            else
            {
                Console.WriteLine("Please enter a valid number!"); 
            }
        }
    }

    private static void AddCourier()
    {
        BO.Courier c = new BO.Courier();
        Console.Write("ID (9 digits): "); c.Id = int.Parse(Console.ReadLine()!);
        Console.Write("Name: "); c.Name = Console.ReadLine()!;
        Console.Write("Phone: "); c.Phone = Console.ReadLine()!;
        Console.Write("Email: "); c.Email = Console.ReadLine()!;
        Console.Write("Password: "); c.Password = Console.ReadLine()!;
        Console.Write("Max Distance: "); c.MaxDistance = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Shipment Type (0-Foot, 1-Car, 2-Motorcycle, 3-Bicycle): ");
        c.ShipmentType = (ShipmentType)int.Parse(Console.ReadLine()!);

        c.IsActive = true;

        s_bl.Courier.AddCourier(user_id, c);
        Console.WriteLine("Courier added successfully.");
    }

    private static void GetCourier()
    {
        Console.Write("Enter Courier ID: ");
        int id = int.Parse(Console.ReadLine()!);
        var courier = s_bl.Courier.GetCourierDetails(user_id, id);
        Console.WriteLine(courier.ToString());
    }

    private static void ShowAllCouriers()
    {
        // קריאה לפונקציה עם null כדי לקבל הכל
        var list = s_bl.Courier.GetCouriers(user_id, null, null);
        Console.WriteLine(
      $"\n{"ID",-10} {"Name",-22} {"Active",-8} {"Type",-12} {"OnTime",-8} {"Late",-6} {"CurrentOrder",-14}");

        Console.WriteLine(new string('-', 90));

        foreach (var item in list)
        {
            Console.WriteLine(
                $"{item.Id,-10} {item.FullName,-22} {item.IsActive,-8} {item.ShipmentType,-12} {item.DeliveredOnTimeCount,-8} {item.DeliveredLateCount,-6} {item.CurrentOrderId,-14}");
        }

    }

    private static void UpdateCourier()
    {
        try
        {

        Console.Write("Enter Courier ID to update: ");
        int id = int.Parse(Console.ReadLine()!);
        var courier = s_bl.Courier.GetCourierDetails(user_id, id);

        Console.Write($"New Name ({courier.Name}): ");
        string input = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(input)) courier.Name = input;

        Console.Write($"New Phone ({courier.Phone}): ");
        input = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(input)) courier.Phone = input;

        Console.Write($"New Email ({courier.Email}): ");
        input = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(input)) courier.Email = input;

        Console.Write($"New Password ({courier.Password}): ");
        input = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(input)) courier.Password = input;

        Console.Write($"Is Active ({courier.IsActive})? (Press 'Y' for Yes, 'N' for No, or Enter to skip): ");
        input = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(input))
        {
            if (input.Trim().ToUpper() == "Y") courier.IsActive = true;
            else if (input.Trim().ToUpper() == "N") courier.IsActive = false;
        }
       
            s_bl.Courier.UpdateCourier(user_id, courier);
            Console.WriteLine("Courier updated.");
        }
        catch (Exception ex) { Console.WriteLine(ex); }
    }


    private static void DeleteCourier()
    {
        Console.Write("Enter Courier ID to delete: ");
        int id = int.Parse(Console.ReadLine()!);
        s_bl.Courier.DeleteCourier(user_id, id);
        Console.WriteLine("Courier deleted.");
    }
    #endregion

    #region Order Management
    private static void OrderMenu()
    {
        bool back = false;
        while (!back)
        {
            PrintHeader("Order Management");
            Console.WriteLine("1. Add Order");
            Console.WriteLine("2. Get Order Details");
            Console.WriteLine("3. Get All Orders (List)");
            Console.WriteLine("4. Choose Order For Handling (Assign Courier)");
            Console.WriteLine("5. Complete Order Handling (Finish Delivery)");
            Console.WriteLine("6. Cancel Order");
            Console.WriteLine("0. Back");
            Console.Write(">> ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                try
                {
                    switch (choice)
                    {
                        case 1: AddOrder(); break;
                        case 2: GetOrder(); break;
                        case 3: ShowAllOrders(); break;
                        case 4: ChooseOrderForHandling(); break;
                        case 5: CompleteOrderHandling(); break;
                        case 6: CancelOrder(); break;
                        case 0: back = true; break;
                    }
                }
                catch (Exception ex) { PrintError("Order Operation Failed", ex); }
            }
            else
            {
                Console.WriteLine("Please enter a valid number!"); 
            }
        }
    }

    private static void AddOrder()
    {
        BO.Order o = new BO.Order();
        Console.Write("Customer Name: "); o.CustomerName = Console.ReadLine()!;
        Console.Write("Customer Phone: "); o.CustomerPhone = Console.ReadLine()!;
        Console.Write("Address (City, Street): "); o.Address = Console.ReadLine()!;

        Console.WriteLine("Order Type (0-Standard, 1-Express...): ");
        o.OrderType = (OrderType)int.Parse(Console.ReadLine()!);

        o.OrderCreated = s_bl.Admin.GetClock(); // שימוש בשעון המערכת

        s_bl.Order.AddOrder(user_id, o);
        Console.WriteLine("Order added. Coordinates calculated automatically by BL.");
    }

    private static void GetOrder()
    {
        Console.Write("Enter Order ID: ");
        int id = int.Parse(Console.ReadLine()!);
        var order = s_bl.Order.GetOrderDetails(user_id, id);

        if (order != null)
            Console.WriteLine(order.ToString());
        else
            Console.WriteLine("Order not found.");
    }

    private static void ShowAllOrders()
    {
        // שימוש בפונקציה הגנרית GetOrdersList
        var list = s_bl.Order.GetOrdersList(user_id, null, null, null);

        Console.WriteLine("\nID | Type | Status | Schedule");
        Console.WriteLine(new string('-', 50));
        foreach (var item in list)
        {
            Console.WriteLine($"{item.OrderId} | {item.OrderType} | {item.OrderStatus} | {item.ScheduleStatus}");
        }
    }

    private static void ChooseOrderForHandling()
    {
        Console.WriteLine("--- Assign Courier to Order ---");
        Console.Write("Courier ID: ");
        int cid = int.Parse(Console.ReadLine()!);
        Console.Write("Order ID: ");
        int oid = int.Parse(Console.ReadLine()!);

        s_bl.Order.ChooseOrderForHandling(user_id, cid, oid);
        Console.WriteLine("Order assigned successfully! Status changed to InProgress.");
    }

    private static void CompleteOrderHandling()
    {
        int cid;

        Console.WriteLine("--- Finish Delivery ---");
        if (type.ToString() == "Courier")
        {
            cid = user_id;
        }
        else {
            Console.Write("Courier ID: ");
            cid = int.Parse(Console.ReadLine()!);
        }
        
        var courier = s_bl.Courier.GetCourierDetails(user_id, cid);

        if (courier.InProgress == null)
        {
            Console.WriteLine("This courier has no order in progress.");
            return;
        }

        int did = courier.InProgress.DeliveryId;

        s_bl.Order.CompleteOrderHandling(user_id, cid, did);

        
        Console.WriteLine("Order completed! Status changed to Completed/Provided.");
    }

    private static void CancelOrder()
    {
        Console.Write("Enter Order ID to cancel: ");
        int oid = int.Parse(Console.ReadLine()!);
        s_bl.Order.CancelOrder(user_id, oid);
        Console.WriteLine("Order canceled.");
    }
    #endregion

    #region Helpers
    private static void PrintHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n--- {title} ---");
        Console.ResetColor();
    }

    private static void PrintError(string msg, Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERROR] {msg}: {ex.Message}");
        if (ex.InnerException != null)
            Console.WriteLine($"Inner: {ex.InnerException.Message}");
        Console.ResetColor();
    }
    #endregion
}
