using BIApi;
using BO;

namespace BITest;

class Program
{
    static readonly IBi s_bl = Factory.Get();

    static void Main(string[] args)
    {
        Console.WriteLine("BL Test Program - Stage 4");
        Console.WriteLine("==========================");

        int choice;
        do
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Admin Operations");
            Console.WriteLine("2. Courier Operations");
            Console.WriteLine("3. Order Operations");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        AdminMenu();
                        break;
                    case 2:
                        CourierMenu();
                        break;
                    case 3:
                        OrderMenu();
                        break;
                    case 0:
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            catch (BO.BlAlreadyExistsException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            catch (BO.BlInvalidInputException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (BO.BLUnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (BO.BlInvalidOperationStateException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        } while (choice != 0);
    }

    #region Admin Menu

    static void AdminMenu()
    {
        int choice;
        do
        {
            Console.WriteLine("\nAdmin Menu:");
            Console.WriteLine("1. Reset Database");
            Console.WriteLine("2. Initialize Database");
            Console.WriteLine("3. Get System Clock");
            Console.WriteLine("4. Forward Clock");
            Console.WriteLine("5. Get Configuration");
            Console.WriteLine("6. Update Configuration");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Resetting database...");
                        s_bl.Admin.ResetDB();
                        Console.WriteLine("Database reset successfully.");
                        break;

                    case 2:
                        Console.WriteLine("Initializing database...");
                        s_bl.Admin.InitializeDB();
                        Console.WriteLine("Database initialized successfully.");
                        break;

                    case 3:
                        var clock = s_bl.Admin.GetClock();
                        Console.WriteLine($"Current System Clock: {clock}");
                        break;

                    case 4:
                        Console.WriteLine("Select time unit to forward:");
                        Console.WriteLine("1. Minute");
                        Console.WriteLine("2. Hour");
                        Console.WriteLine("3. Month");
                        Console.WriteLine("4. Year");
                        Console.Write("Enter choice: ");
                        if (int.TryParse(Console.ReadLine(), out int timeChoice))
                        {
                            TimeUnit unit = timeChoice switch
                            {
                                1 => TimeUnit.Minute,
                                2 => TimeUnit.Hour,
                                3 => TimeUnit.Month,
                                4 => TimeUnit.Year,
                                _ => TimeUnit.Minute
                            };
                            s_bl.Admin.ForwardClock(unit);
                            Console.WriteLine($"Clock forwarded. New time: {s_bl.Admin.GetClock()}");
                        }
                        break;

                    case 5:
                        var config = s_bl.Admin.GetConfig();
                        if (config != null)
                        {
                            Console.WriteLine("Configuration:");
                            Console.WriteLine($"  Company Address: {config.CompanyAddress}");
                            Console.WriteLine($"  Latitude: {config.Latitude}");
                            Console.WriteLine($"  Longitude: {config.Longitude}");
                            Console.WriteLine($"  Max Air Range: {config.MaxAirRange}");
                            Console.WriteLine($"  Avg Car Speed: {config.AvgCarSpeed}");
                            Console.WriteLine($"  Avg Motorcycle Speed: {config.AvgMotocyclerSpeed}");
                            Console.WriteLine($"  Avg Bicycle Speed: {config.AvgBicycleSpeed}");
                            Console.WriteLine($"  Avg Walking Speed: {config.AvgWalkingSpeed}");
                        }
                        break;

                    case 6:
                        Console.Write("Enter Max Air Range: ");
                        if (double.TryParse(Console.ReadLine(), out double maxRange))
                        {
                            var currentConfig = s_bl.Admin.GetConfig();
                            if (currentConfig != null)
                            {
                                currentConfig.MaxAirRange = maxRange;
                                s_bl.Admin.SetConfig(currentConfig);
                                Console.WriteLine("Configuration updated successfully.");
                            }
                        }
                        break;

                    case 0:
                        Console.WriteLine("Returning to main menu...");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        } while (choice != 0);
    }

    #endregion

    #region Courier Menu

    static void CourierMenu()
    {
        int choice;
        do
        {
            Console.WriteLine("\nCourier Menu:");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Get All Couriers");
            Console.WriteLine("3. Get Courier Details");
            Console.WriteLine("4. Add Courier");
            Console.WriteLine("5. Update Courier");
            Console.WriteLine("6. Delete Courier");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Enter Username (ID): ");
                        string? username = Console.ReadLine();
                        Console.Write("Enter Password: ");
                        string? password = Console.ReadLine();
                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                        {
                            var userType = s_bl.Courier.Login(username, password);
                            Console.WriteLine($"Login successful. User Type: {userType}");
                        }
                        break;

                    case 2:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int requesterId))
                        {
                            Console.Write("Filter by active status? (y/n/Enter for all): ");
                            string? activeInput = Console.ReadLine();
                            bool? isActive = activeInput?.ToLower() switch
                            {
                                "y" => true,
                                "n" => false,
                                _ => null
                            };

                            var couriers = s_bl.Courier.GetCouriers(requesterId, isActive, null);
                            Console.WriteLine("\nCouriers List:");
                            foreach (var courier in couriers)
                            {
                                Console.WriteLine(courier);
                            }
                        }
                        break;

                    case 3:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int reqId))
                        {
                            Console.Write("Enter Courier ID to view: ");
                            if (int.TryParse(Console.ReadLine(), out int courierId))
                            {
                                var courier = s_bl.Courier.GetCourierDetails(reqId, courierId);
                                Console.WriteLine(courier);
                            }
                        }
                        break;

                    case 4:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int addReqId))
                        {
                            var newCourier = CreateCourierFromInput();
                            s_bl.Courier.AddCourier(addReqId, newCourier);
                            Console.WriteLine("Courier added successfully.");
                        }
                        break;

                    case 5:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int updateReqId))
                        {
                            var updateCourier = CreateCourierFromInput();
                            s_bl.Courier.UpdateCourier(updateReqId, updateCourier);
                            Console.WriteLine("Courier updated successfully.");
                        }
                        break;

                    case 6:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int delReqId))
                        {
                            Console.Write("Enter Courier ID to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int delCourierId))
                            {
                                s_bl.Courier.DeleteCourier(delReqId, delCourierId);
                                Console.WriteLine("Courier deleted successfully.");
                            }
                        }
                        break;

                    case 0:
                        Console.WriteLine("Returning to main menu...");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            catch (BO.BLUnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (BO.BlInvalidInputException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        } while (choice != 0);
    }

    static Courier CreateCourierFromInput()
    {
        Console.Write("Enter Courier ID: ");
        int.TryParse(Console.ReadLine(), out int id);
        
        Console.Write("Enter Name: ");
        string name = Console.ReadLine() ?? "";
        
        Console.Write("Enter Phone: ");
        string phone = Console.ReadLine() ?? "";
        
        Console.Write("Enter Email: ");
        string email = Console.ReadLine() ?? "";
        
        Console.Write("Enter Password: ");
        string pwd = Console.ReadLine() ?? "";
        
        Console.Write("Is Active? (y/n): ");
        bool isActive = Console.ReadLine()?.ToLower() == "y";
        
        Console.Write("Enter Max Distance (or press Enter for null): ");
        double? maxDist = null;
        if (double.TryParse(Console.ReadLine(), out double dist))
            maxDist = dist;
        
        Console.WriteLine("Shipment Type: 0=Foot, 1=Car, 2=Motorcycle, 3=Bicycle");
        Console.Write("Enter Shipment Type: ");
        ShipmentType shipment = ShipmentType.Foot;
        if (int.TryParse(Console.ReadLine(), out int shipVal))
            shipment = (ShipmentType)shipVal;

        return new Courier
        {
            Id = id,
            Name = name,
            Phone = phone,
            Email = email,
            Password = pwd,
            IsActive = isActive,
            MaxDistance = maxDist,
            ShipmentType = shipment,
            StartTime = DateTime.Now
        };
    }

    #endregion

    #region Order Menu

    static void OrderMenu()
    {
        int choice;
        do
        {
            Console.WriteLine("\nOrder Menu:");
            Console.WriteLine("1. Get Orders List");
            Console.WriteLine("2. Get Order Details");
            Console.WriteLine("3. Add Order");
            Console.WriteLine("4. Update Order");
            Console.WriteLine("5. Cancel Order");
            Console.WriteLine("6. Delete Order");
            Console.WriteLine("7. Get Orders Summary (Matrix)");
            Console.WriteLine("8. Choose Order for Handling");
            Console.WriteLine("9. Complete Order Handling");
            Console.WriteLine("10. Get Closed Orders");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int listReqId))
                        {
                            var orders = s_bl.Order.GetOrdersList(listReqId, null, null, null);
                            Console.WriteLine("\nOrders List:");
                            foreach (var order in orders)
                            {
                                Console.WriteLine(order);
                            }
                        }
                        break;

                    case 2:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int detReqId))
                        {
                            Console.Write("Enter Order ID: ");
                            if (int.TryParse(Console.ReadLine(), out int orderId))
                            {
                                var order = s_bl.Order.GetOrderDetails(detReqId, orderId);
                                Console.WriteLine(order);
                            }
                        }
                        break;

                    case 3:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int addReqId))
                        {
                            var newOrder = CreateOrderFromInput();
                            s_bl.Order.addOrder(addReqId, newOrder);
                            Console.WriteLine("Order added successfully.");
                        }
                        break;

                    case 4:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int updReqId))
                        {
                            var updateOrder = CreateOrderFromInput();
                            s_bl.Order.UpdateOrder(updReqId, updateOrder);
                            Console.WriteLine("Order updated successfully.");
                        }
                        break;

                    case 5:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int canReqId))
                        {
                            Console.Write("Enter Order ID to cancel: ");
                            if (int.TryParse(Console.ReadLine(), out int canOrderId))
                            {
                                s_bl.Order.CancelOrder(canReqId, canOrderId);
                                Console.WriteLine("Order cancelled successfully.");
                            }
                        }
                        break;

                    case 6:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int delReqId))
                        {
                            Console.Write("Enter Order ID to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int delOrderId))
                            {
                                s_bl.Order.deleteOrder(delReqId, delOrderId);
                                Console.WriteLine("Order deleted successfully.");
                            }
                        }
                        break;

                    case 7:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int sumReqId))
                        {
                            var summary = s_bl.Order.GetOrdersAmountSummary(sumReqId);
                            Console.WriteLine("\nOrders Summary Matrix:");
                            Console.WriteLine("(Rows: OrderStatus, Columns: ScheduleStatus)");
                            for (int i = 0; i < summary.Length; i++)
                            {
                                Console.Write($"{(OrderStatus)i}: ");
                                for (int j = 0; j < summary[i].Length; j++)
                                {
                                    Console.Write($"{summary[i][j]} ");
                                }
                                Console.WriteLine();
                            }
                        }
                        break;

                    case 8:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int chooseReqId))
                        {
                            Console.Write("Enter Courier ID: ");
                            if (int.TryParse(Console.ReadLine(), out int chooseCourierId))
                            {
                                Console.Write("Enter Order ID: ");
                                if (int.TryParse(Console.ReadLine(), out int chooseOrderId))
                                {
                                    s_bl.Order.ChooseOrderForHandling(chooseReqId, chooseCourierId, chooseOrderId);
                                    Console.WriteLine("Order assigned to courier successfully.");
                                }
                            }
                        }
                        break;

                    case 9:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int compReqId))
                        {
                            Console.Write("Enter Courier ID: ");
                            if (int.TryParse(Console.ReadLine(), out int compCourierId))
                            {
                                Console.Write("Enter Delivery ID: ");
                                if (int.TryParse(Console.ReadLine(), out int compDeliveryId))
                                {
                                    s_bl.Order.CompleteOrderHandling(compReqId, compCourierId, compDeliveryId);
                                    Console.WriteLine("Order handling completed successfully.");
                                }
                            }
                        }
                        break;

                    case 10:
                        Console.Write("Enter Requester ID: ");
                        if (int.TryParse(Console.ReadLine(), out int closedReqId))
                        {
                            Console.Write("Enter Courier ID: ");
                            if (int.TryParse(Console.ReadLine(), out int closedCourierId))
                            {
                                var closedOrders = s_bl.Order.GetClosedOrders(closedReqId, closedCourierId, null, null);
                                Console.WriteLine("\nClosed Orders:");
                                foreach (var order in closedOrders)
                                {
                                    Console.WriteLine(order);
                                }
                            }
                        }
                        break;

                    case 0:
                        Console.WriteLine("Returning to main menu...");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            catch (BO.BlInvalidOperationStateException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        } while (choice != 0);
    }

    static Order CreateOrderFromInput()
    {
        Console.Write("Enter Order ID: ");
        int.TryParse(Console.ReadLine(), out int id);
        
        Console.WriteLine("Order Type: 0=Standard, 1=Express, 2=Fragile, 3=Heavy");
        Console.Write("Enter Order Type: ");
        OrderType orderType = OrderType.Standard;
        if (int.TryParse(Console.ReadLine(), out int typeVal))
            orderType = (OrderType)typeVal;
        
        Console.Write("Enter Description: ");
        string description = Console.ReadLine() ?? "";
        
        Console.Write("Enter Address: ");
        string address = Console.ReadLine() ?? "";
        
        Console.Write("Enter Latitude: ");
        double.TryParse(Console.ReadLine(), out double lat);
        
        Console.Write("Enter Longitude: ");
        double.TryParse(Console.ReadLine(), out double lon);
        
        Console.Write("Enter Customer Name: ");
        string custName = Console.ReadLine() ?? "";
        
        Console.Write("Enter Customer Phone: ");
        string custPhone = Console.ReadLine() ?? "";
        
        Console.Write("Enter Notes (or press Enter for none): ");
        string? notes = Console.ReadLine();
        if (string.IsNullOrEmpty(notes)) notes = null;

        return new Order
        {
            Id = id,
            OrderType = orderType,
            orderType = orderType,
            Description = description,
            Address = address,
            Latitude = lat,
            Longitude = lon,
            CustomerName = custName,
            CustomerPhone = custPhone,
            Notes = notes,
            OrderCreated = DateTime.Now
        };
    }

    #endregion
}

