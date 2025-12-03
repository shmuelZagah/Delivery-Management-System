using BIApi;
using BO;

namespace BlImplementation;

//--------------------------
// CourierImplementation
//--------------------------
internal class CourierImplementation : BIApi.ICourier
{

    // Login method for courier authentication
    public UserType Login(string username, string password)
    {
        //Input validation
        if(!Helpers.Tools.IdValidtion(int.Parse(username))) 
            throw new BlInvalidInputException("Username must be 9 characters long");

        var courier = Helpers.CourierManager.GetCourier(int.TryParse(username, out var idTemp) ? idTemp
            : throw new BlInvalidInputException("Invalid input, Username input is invalid"));

        if (courier!.Password != password)
            throw new BlInvalidInputException("Incorrect password");

        return Helpers.CourierManager.GetUserType(username);
    }


    // Retrieve a list of couriers
    public IEnumerable<CourierInList> GetCouriers(int requesterId, bool? isActive, CourierField? courierField)
    {

        // Authorization check: only managers can access this method
        Helpers.CourierManager.EnsureIsManager(requesterId.ToString());

        // Get all couriers (if isActive is null get all)
        IEnumerable<CourierInList> couriers = Helpers.CourierManager.GetAllCouriers(p=> p.IsActive == isActive);

        if (courierField != null)
        {
            // Apply additional filtering or selection based on the courierField
            couriers = Helpers.Tools.SortWithGroupBy(couriers, c =>
         c.GetType().GetProperty(courierField.ToString()!)?.GetValue(c) ?? 0);
        }

        return couriers;
    }


    public void AddCourier(int requesterId, Courier courier)
    {
        // Authorization check: only managers can access this method
        Helpers.CourierManager.EnsureIsManager(requesterId.ToString());

        // Validate courier details
        Helpers.CourierManager.ValidationOfCarrier(courier);

        //If all validations passed, add the courier to the system
        Helpers.CourierManager.AddCourier(courier);
    }

    public void DeleteCourier(int requesterId, int courierId)
    {
        // Authorization check: only managers can access this method
        Helpers.CourierManager.EnsureIsManager(requesterId.ToString());

        // Delete the courier from the system
        Helpers.CourierManager.DeleteCourier(courierId);

    }

    public Courier GetCourierDetails(int requesterId, int courierId)
    {
        var courier = Helpers.CourierManager.GetCourier(courierId);

        if(requesterId == courier!.Id || Helpers.CourierManager.EnsureIsManager(requesterId.ToString()))
        {
            return courier;
        }
        else throw new BLUnauthorizedAccessException("Unauthorized access attempt detected");
    }

    public void UpdateCourier(int requesterId, Courier courier)
    {

        var courierToUpdate = Helpers.CourierManager.GetCourier(courier.Id);

        // Authorization check: only managers can change isActive status
        if ( courierToUpdate!.IsActive != courier.IsActive)
            try { 
            Helpers.CourierManager.EnsureIsManager(requesterId.ToString());
            }
            catch (BLUnauthorizedAccessException ex)
            {
                throw new BLUnauthorizedAccessException("Cannot change the activeness status",ex);
            }

        // Update courier details
        Helpers.CourierManager.UpdateCourier(courier);

    }
}