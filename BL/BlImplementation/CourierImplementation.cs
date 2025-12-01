using BIApi;
using BO;
using System.Collections.Generic;
using System.Reflection;

namespace BlImplementation;

internal class CourierImplementation : BIApi.ICourier
{

    // Login method for courier authentication
    public UserType Login(string username, string password)
    {
        var courier = Helpers.CourierManager.GetCourier(int.TryParse(username, out var idTemp) ? idTemp
            : throw new BlDoesNotExistException("Invalid input, need ID")) 
            ?? throw new BlInvalidInputException("Courier not found");

        if (courier.Password != password)
            throw new BlInvalidInputException("Incorrect password");

        return Helpers.CourierManager.GetUserType(username);
    }


    // Retrieve a list of couriers
    public IEnumerable<Courier> getCouriers(int requesterId, bool? isActive, CourierField? courierField)
    {
        // Get all couriers (if isActive is null get all)
        IEnumerable<Courier> couriers = Helpers.CourierManager.GetAllCouriers(p=> p.IsActive == isActive);

        if (courierField != null)
        {
            // Apply additional filtering or selection based on the courierField
            couriers = Helpers.Tools.SortWithGroupBy(couriers, c =>
         c.GetType().GetProperty(courierField.ToString()!)?.GetValue(c) ?? 0);
        }

        return couriers;
    }


    public void addCourier(int requesterId, Courier courier)
    {
        throw new NotImplementedException();
    }

    public void deleteCourier(int requesterId, int courierId)
    {
        throw new NotImplementedException();
    }

    public Courier GetCourierDetails(int requesterId, int courierId)
    {
        throw new NotImplementedException();
    }

    public void updateCourier(int requesterId, Courier courier)
    {
        throw new NotImplementedException();
    }
}